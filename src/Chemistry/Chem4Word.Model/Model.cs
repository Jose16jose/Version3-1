﻿// ---------------------------------------------------------------------------
//  Copyright (c) 2019, The .NET Foundation.
//  This software is released under the Apache License, Version 2.0.
//  The license and further copyright text can be found in the file LICENSE.md
//  at the root directory of the distribution.
// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace Chem4Word.Model
{
    /// <summary>
    /// Overall container for Atoms, Bonds and other objects.
    /// Please limit rendering-specific code in these classes.
    /// Sometimes it will be unavoidable, but the less, the better
    /// </summary>
    public class Model : ChemistryContainer, INotifyPropertyChanged
    {
        public string CustomXmlPartGuid { get; set; }

        public Model()
        {
            GeneralErrors = new List<string>();
        }

        public string ConciseFormula
        {
            get
            {
                string result = "";
                Dictionary<string, int> f = new Dictionary<string, int>();
                foreach (var mol in Molecules)
                {
                    if (string.IsNullOrEmpty(mol.ConciseFormula))
                    {
                        mol.ConciseFormula = mol.CalculatedFormula();
                    }

                    if (f.ContainsKey(mol.ConciseFormula))
                    {
                        f[mol.ConciseFormula]++;
                    }
                    else
                    {
                        f.Add(mol.ConciseFormula, 1);
                    }
                }

                foreach (KeyValuePair<string, int> kvp in f)
                {
                    if (kvp.Value == 1)
                    {
                        result += $"{kvp.Key} . ";
                    }
                    else
                    {
                        result += $"{kvp.Value} {kvp.Key} . ";
                    }
                }

                if (result.EndsWith(" . "))
                {
                    result = result.Substring(0, result.Length - 3);
                }

                return result;
            }
        }

        public List<string> GeneralErrors { get; set; }

        public List<string> AllWarnings
        {
            get
            {
                return Molecules.SelectMany(m => m.Warnings).ToList();
            }
        }

        public List<string> AllErrors
        {
            get
            {
                var result = new List<string>();
                result.AddRange(GeneralErrors);
                result.AddRange(Molecules.SelectMany(m => m.Errors));
                return result;
            }
        }

        /// <summary>
        /// Rolls up all those objects exposed to the view model so they can be displayed
        /// This is only Atoms and Bonds for now
        /// </summary>
        public CompositeCollection AllObjects
        {
            get
            {
                CompositeCollection theLot = new CompositeCollection();
                CollectionContainer cc1 = new CollectionContainer();
                cc1.Collection = AllAtoms;

                CollectionContainer cc2 = new CollectionContainer();
                cc2.Collection = AllBonds;

                theLot.Add(cc2);
                //Atoms MUST be added after Bonds to ensure they get z-indexed properly.
                theLot.Add(cc1);
                return theLot;
            }
        }

        public List<Ring> Rings
        {
            get { return Molecules.SelectMany(m => m.Rings).ToList(); }
        }

        /// <summary>
        /// Adding molecules to  or removing from the model also adds the Atoms and Bonds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Molecules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    foreach (Molecule m in e.NewItems)
                    {
                        foreach (Atom atom in m.Atoms)
                        {
                            if (!AllAtoms.Contains(atom))
                            {
                                AllAtoms.Add(atom);
                            }
                        }

                        foreach (Bond bond in m.Bonds)
                        {
                            if (!AllBonds.Contains(bond))
                            {
                                AllBonds.Add(bond);
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Molecule m in e.OldItems)
                    {
                        foreach (Atom atom in m.Atoms.ToList())
                        {
                            AllAtoms.Remove(atom);
                        }

                        foreach (Bond bond in m.Bonds.ToList())
                        {
                            AllBonds.Remove(bond);
                        }
                    }
                    break;
            }
            OnPropertyChanged(nameof(BoundingBox));
        }

        public void Relabel(bool includeNames)
        {
            int iBondcount = 0, iAtomCount = 0, iMolcount = 0;

            foreach (Molecule m in Molecules)
            {
                m.ReLabel(includeNames, ref iMolcount, ref iAtomCount, ref iBondcount);
            }
        }

        /// <summary>
        /// Re-labels the model from scratch.  Molecules, Atoms and Bonds all get a new label.
        /// </summary>
        public void Relabel()
        {
            Relabel(false);
        }

        /// <summary>
        /// Regenerates molecule collections from the bottom up
        /// WARNING ** THIS IS VERY DESTRUCTIVE CALL AT YOUR PERIL **
        /// ToDo: Get rid of this routine
        /// HACK: This routine should never exist it's too destructive
        /// </summary>
        public void RebuildMolecules()
        {
            foreach (Atom atom in AllAtoms)
            {
                atom.Parent = null;
            }

            foreach (Bond bond in AllBonds)
            {
                bond.Parent = null;
            }

            Molecules.Clear();

            AddNewMols();
        }

        private void AddNewMols()
        {
            while (AllAtoms.Count(a => a.Parent == null) > 0)
            {
                Atom seed = AllAtoms.First(a => a.Parent == null);
                Molecule m = new Molecule(seed);
                Molecules.Add(m);
            }
            OnPropertyChanged(nameof(BoundingBox));
        }

        /// <summary>
        /// Refereshes molecules, leaving those already assigned intact
        /// </summary>
        public void RefreshMolecules()
        {
            foreach (Molecule molecule in Molecules.ToList())
            {
                molecule.Refresh();
            }
            AddNewMols();
        }

        public Model Clone()
        {
            Model clone = new Model();

            // Strictly speaking this is modifying the original object.
            //  but it is required to allow re-connecting of atoms with the correct bonds.
            Relabel();

            clone.CustomXmlPartGuid = CustomXmlPartGuid;
            clone.ScaledForXaml = ScaledForXaml;

            foreach (var molecule in Molecules)
            {
                Molecule m = molecule.Clone();
                m.Id = molecule.Id;
                m.ConciseFormula = m.CalculatedFormula();
                m.RebuildRings();
                clone.Molecules.Add(m);
            }

            clone.Relabel();
            clone.RefreshMolecules();

            return clone;
        }

        #region Layout

        public double ActualWidth
        {
            get { return BoundingBox.Width; }
        }

        public double ActualHeight
        {
            get { return BoundingBox.Height; }
        }

        //used to calculate the bounds of the atom
        public double FontSize
        {
            get
            {
                double fontSize = Globals.DefaultFontSize * Globals.ScaleFactorForXaml;

                if (AllBonds.Any())
                {
                    fontSize = XamlBondLength * Globals.FontSizePercentageBond;
                }

                return fontSize;
            }
        }

        public double XamlBondLength;

        private Rect _boundingBox = Rect.Empty;

        public Rect BoundingBox
        {
            get
            {
                if (_boundingBox == Rect.Empty)
                {
                    var modelRect = AllAtoms[0].BoundingBox(FontSize);
                    for (int i = 1; i < AllAtoms.Count; i++)
                    {
                        var atom = AllAtoms[i];
                        modelRect.Union(atom.BoundingBox(FontSize));
                    }

                    Point topleft = new Point(modelRect.TopLeft.X - FontSize, modelRect.TopLeft.Y - FontSize); // modelRect.TopLeft;
                    Point bottomRight = new Point(modelRect.BottomRight.X + FontSize, modelRect.BottomRight.Y + FontSize); //modelRect.BottomRight;
                    var bb = new Rect(topleft, bottomRight);

                    _boundingBox = modelRect;
                }

                return _boundingBox;
            }
        }

        public double MinX => BoundingBox.Left;
        public double MaxX => BoundingBox.Right;
        public double MinY => BoundingBox.Top;
        public double MaxY => BoundingBox.Bottom;

        public double MeanBondLength
        {
            get
            {
                double result = 0;
                if (AllBonds.Any())
                {
                    result = AllBonds.Average(b => b.BondVector.Length);
                }
                return result;
            }
        }

        /// <summary>
        /// Centres the model in a canvas
        /// </summary>
        /// <param name="canvas"></param>
        public void CentreInCanvas(Size canvas)
        {
            // Re-Centre scaled drawing on Canvas, does not need to be undone
            double desiredLeft = (canvas.Width - BoundingBox.Width) / 2.0;
            double desiredTop = (canvas.Height - BoundingBox.Height) / 2.0;
            double offsetLeft = BoundingBox.Left - desiredLeft;
            double offsetTop = BoundingBox.Top - desiredTop;

            RepositionAll(offsetLeft, offsetTop);
        }

        /// <summary>
        /// Drags all Atoms back to the origin by the specified offset
        /// </summary>
        /// <param name="x">X offset</param>
        /// <param name="y">Y offset</param>
        public void RepositionAll(double x, double y)
        {
            foreach (Molecule molecule in Molecules)
            {
                molecule.RepositionAll(x, y);
            }
        }

        /// <summary>
        /// Rescale to new preferred length
        /// </summary>
        /// <param name="newLength"></param>
        public void ScaleToAverageBondLength(double newLength)
        {
            if (MeanBondLength > 0)
            {
                double scale = newLength / MeanBondLength;
                foreach (var atom in AllAtoms)
                {
                    atom.Position = new Point(atom.Position.X * scale, atom.Position.Y * scale);
                }
            }

            XamlBondLength = newLength;
            OnPropertyChanged(nameof(BoundingBox));
            OnPropertyChanged(nameof(XamlBondLength));
        }

        public bool ScaledForXaml { get; set; }

        public void RescaleForCml()
        {
            if (ScaledForXaml)
            {
                if (MeanBondLength > 0)
                {
                    ScaleToAverageBondLength(MeanBondLength / Globals.ScaleFactorForXaml);
                }
                else
                {
                    ScaleToAverageBondLength(Globals.SingleAtomPseudoBondLength / Globals.ScaleFactorForXaml);
                }
                ScaledForXaml = false;
            }
        }

        /// <summary>
        /// Rescale to new preferred length, to be used in xaml code behind, not normal cs
        /// </summary>
        public void RescaleForXaml(bool forDisplay)
        {
            if (!ScaledForXaml)
            {
                if (MeanBondLength > 0)
                {
                    ScaleToAverageBondLength(MeanBondLength * Globals.ScaleFactorForXaml);
                }
                else
                {
                    ScaleToAverageBondLength(Globals.SingleAtomPseudoBondLength * Globals.ScaleFactorForXaml);
                }
                ScaledForXaml = true;

                if (forDisplay)
                {
                    RepositionAll(MinX, MinY);
                }
                OnPropertyChanged(nameof(BoundingBox));
            }
        }

        #endregion Layout

        #region Interface implementations

        public new event PropertyChangedEventHandler PropertyChanged;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Interface implementations

        #region Diagnostics

        public void DumpModel(string comment)
        {
            Debug.WriteLine(comment);
            Debug.WriteLine($"Model.Molecules.Count: {Molecules.Count}");
            Debug.WriteLine($"Model.Atoms.Count: {AllAtoms.Count}");
            Debug.WriteLine($"Model.Bonds.Count: {AllBonds.Count}");
            Debug.WriteLine($"Model.Rings.Count: {Rings.Count}");

            foreach (var molecule in Molecules)
            {
                Debug.WriteLine($" Molecule.Id: {molecule.Id}");
                Debug.WriteLine($" Molecule.Atoms.Count: {molecule.Atoms.Count}");
                Debug.WriteLine($" Molecule.AllAtoms.Count: {molecule.AllAtoms.Count}");
                Debug.WriteLine($" Molecule.Bonds.Count: {molecule.Bonds.Count}");
                Debug.WriteLine($" Molecule.AllBonds.Count: {molecule.AllBonds.Count}");
                Debug.WriteLine($" Molecule.Rings.Count: {molecule.Rings.Count}");

                foreach (var atom in molecule.Atoms)
                {
                    Debug.WriteLine($"  Atom.Id: {atom.Id}");
                }
                foreach (var atom in molecule.AllAtoms)
                {
                    Debug.WriteLine($"  AllAtom.Id: {atom.Id}");
                }
                foreach (var bond in molecule.Bonds)
                {
                    Debug.WriteLine($"  Bond.Id: {bond.Id} Rings: {bond.Rings.Count}");
                }
                foreach (var bond in molecule.AllBonds)
                {
                    Debug.WriteLine($"  AllBond.Id: {bond.Id} Rings: {bond.Rings.Count}");
                }
            }
        }

        #endregion Diagnostics
    }
}