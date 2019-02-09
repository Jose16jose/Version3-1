﻿// ---------------------------------------------------------------------------
//  Copyright (c) 2019, The .NET Foundation.
//  This software is released under the Apache License, Version 2.0.
//  The license and further copyright text can be found in the file LICENSE.md
//  at the root directory of the distribution.
// ---------------------------------------------------------------------------

using Chem4Word.Model2.Helpers;
using Chem4Word.Model2.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Chem4Word.Model2
{
    public class Model : IChemistryContainer, INotifyPropertyChanged
    {
        #region Fields

        public event NotifyCollectionChangedEventHandler AtomsChanged;

        public event NotifyCollectionChangedEventHandler BondsChanged;

        public event NotifyCollectionChangedEventHandler MoleculesChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Fields

        #region Event handlers

        private void UpdateMoleculeEventHandlers(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var mol = ((Molecule)oldItem);
                    mol.AtomsChanged -= Atoms_CollectionChanged;
                    mol.BondsChanged -= Bonds_CollectionChanged;
                    mol.PropertyChanged -= ChemObject_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    var mol = ((Molecule)newItem);
                    mol.AtomsChanged += Atoms_CollectionChanged;
                    mol.BondsChanged += Bonds_CollectionChanged;
                    mol.PropertyChanged += ChemObject_PropertyChanged;
                }
            }
        }

        private void OnMoleculesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var temp = MoleculesChanged;
            if (temp != null)
            {
                temp.Invoke(sender, e);
            }
        }

        private void ChemObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender, e);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var temp = PropertyChanged;
            if (temp != null)
            {
                temp.Invoke(sender, e);
            }
        }

        private void Bonds_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnBondsChanged(sender, e);
        }

        private void OnBondsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var temp = BondsChanged;
            if (temp != null)
            {
                temp.Invoke(sender, e);
            }
        }

        private void Atoms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnAtomsChanged(sender, e);
        }

        private void OnAtomsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var temp = AtomsChanged;
            if (temp != null)
            {
                temp.Invoke(sender, e);
            }
        }

        #endregion Event handlers

        #region Properties

        public int TotalAtomsCount
        {
            get
            {
                int count = 0;

                foreach (var molecule in Molecules.Values)
                {
                    count += molecule.AtomCount;
                }

                return count;
            }
        }

        public int TotalBondsCount
        {
            get
            {
                int count = 0;

                foreach (var molecule in Molecules.Values)
                {
                    count += molecule.BondCount;
                }

                return count;
            }
        }

        public double MeanBondLength
        {
            get
            {
                List<double> lengths = new List<double>();

                foreach (var mol in Molecules.Values)
                {
                    lengths.AddRange(mol.BondLengths);
                }

                if (lengths.Any())
                {
                    return lengths.Average();
                }

                if (ScaledForXaml)
                {
                    return XamlBondLength;
                }

                return 0;
            }
        }

        public double XamlBondLength { get; set; }

        public Rect OverallBoundingBox
        {
            get
            {
                bool isNew = true;
                Rect boundingBox = new Rect();

                foreach (var mol in Molecules.Values)
                {
                    if (isNew)
                    {
                        boundingBox = mol.BoundingBox;
                        isNew = false;
                    }
                    else
                    {
                        boundingBox.Union(mol.BoundingBox);
                    }
                }

                return boundingBox;
            }
        }

        public string Path => "/";
        public IChemistryContainer Root => null;

        public bool ScaledForXaml { get; set; }

        private Rect _boundingBox = Rect.Empty;

        public double MinX => BoundingBox.Left;
        public double MaxX => BoundingBox.Right;
        public double MinY => BoundingBox.Top;
        public double MaxY => BoundingBox.Bottom;

        public Rect BoundingBox
        {
            get
            {
                var allAtoms = GetAllAtoms();
                if (_boundingBox == Rect.Empty)
                {
                    var modelRect = allAtoms[0].BoundingBox(FontSize);
                    for (int i = 1; i < allAtoms.Count; i++)
                    {
                        var atom = allAtoms[i];
                        modelRect.Union(atom.BoundingBox(FontSize));
                    }

                    Point topleft =
                        new Point(modelRect.TopLeft.X - FontSize, modelRect.TopLeft.Y - FontSize); // modelRect.TopLeft;
                    Point bottomRight = new Point(modelRect.BottomRight.X + FontSize,
                        modelRect.BottomRight.Y + FontSize); //modelRect.BottomRight;
                    var bb = new Rect(topleft, bottomRight);

                    _boundingBox = modelRect;
                }

                return _boundingBox;
            }
        }

        //used to calculate the bounds of the atom
        public double FontSize
        {
            get
            {
                var allBonds = GetAllBonds();
                double fontSize = Globals.DefaultFontSize * Globals.ScaleFactorForXaml;

                if (allBonds.Any())
                {
                    fontSize = XamlBondLength * Globals.FontSizePercentageBond;
                }

                return fontSize;
            }
        }

        /// <summary>
        /// Drags all Atoms back to the origin by the specified offset
        /// </summary>
        /// <param name="x">X offset</param>
        /// <param name="y">Y offset</param>

        private Dictionary<string, Molecule> _molecules { get; }

        //wraps up the above Molecules collection
        public ReadOnlyDictionary<string, Molecule> Molecules;

        public string CustomXmlPartGuid { get; set; }

        public List<string> GeneralErrors { get; set; }

        public List<string> AllWarnings
        {
            get
            {
                var list = new List<string>();
                foreach (var molecule in Molecules.Values)
                {
                    list.AddRange(molecule.Warnings);
                }

                return list;
            }
        }

        public List<string> AllErrors
        {
            get
            {
                var list = new List<string>();
                foreach (var molecule in Molecules.Values)
                {
                    list.AddRange(molecule.Errors);
                }

                return list;
            }
        }

        public string ConciseFormula
        {
            get
            {
                string result = "";
                Dictionary<string, int> f = new Dictionary<string, int>();
                foreach (var mol in Molecules.Values)
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

        #endregion Properties

        #region Constructors

        public Model()
        {
            _molecules = new Dictionary<string, Molecule>();
            Molecules = new ReadOnlyDictionary<string, Molecule>(_molecules);
            GeneralErrors = new List<string>();
        }

        #endregion Constructors

        #region Methods

        public void RepositionAll(double x, double y)
        {
            foreach (Molecule molecule in Molecules.Values)
            {
                molecule.RepositionAll(x, y);
            }
        }

        public ChemistryBase GetFromPath(string path)
        {
            try
            {
                //first part of the path has to be a molecule
                if (path.StartsWith("/"))
                {
                    path = path.Substring(1); //strip off the first separator
                }

                string molID = path.UpTo("/");

                if (!Molecules.ContainsKey(molID))
                {
                    throw new ArgumentException("First child is not a molecule");
                }

                string relativepath = Helpers.Utils.GetRelativePath(molID, path);
                if (relativepath != "")
                {
                    return Molecules[molID].GetFromPath(relativepath);
                }
                else
                {
                    return Molecules[molID];
                }
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Object {path} not found {ex.Message}");
            }
        }

        public bool RemoveMolecule(Molecule mol)
        {
            var res = _molecules.Remove(mol.InternalId);
            if (res)
            {
                NotifyCollectionChangedEventArgs e =
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                        new List<Molecule> { mol });
                OnMoleculesChanged(this, e);
                UpdateMoleculeEventHandlers(e);
            }

            return res;
        }

        public Molecule AddMolecule(Molecule newMol)
        {
            _molecules[newMol.Id] = newMol;
            NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    new List<Molecule> { newMol });
            OnMoleculesChanged(this, e);
            UpdateMoleculeEventHandlers(e);
            return newMol;
        }

        public void Relabel(bool includeNames)
        {
            int iBondcount = 0, iAtomCount = 0, iMolcount = 0;

            foreach (Molecule m in Molecules.Values)
            {
                m.ReLabel(includeNames, ref iMolcount, ref iAtomCount, ref iBondcount);
            }
        }

        public void Refresh()
        {
            foreach (var molecule in Molecules.Values)
            {
                molecule.Refresh();
            }
        }

        public Model Copy()
        {
            Model copy = new Model();
            foreach (var child in Molecules.Values)
            {
                Molecule m = child.Copy();
                copy.AddMolecule(m);
                m.Parent = copy;
            }

            return copy;
        }

        public Model Clone()
        {
            var clone = this.CloneExcept(new[] { nameof(_molecules), nameof(_boundingBox) });
            clone.ClearMolecules();
            var molList = Molecules.Values.ToList();
            foreach (Molecule source in molList)
            {
                var target = source.Clone();
                target.CheckIntegrity();
                clone.AddMolecule(target);
            }

            return clone;
        }

        private void ClearMolecules()
        {
            _molecules.Clear();
        }

        public void ScaleToAverageBondLength(double newLength)
        {
            if (MeanBondLength > 0)
            {
                double scale = newLength / MeanBondLength;
                var allAtoms = GetAllAtoms();
                foreach (var atom in allAtoms)
                {
                    atom.Position = new Point(atom.Position.X * scale, atom.Position.Y * scale);
                }
            }
        }

        public List<Atom> GetAllAtoms()
        {
            List<Atom> allAtoms = new List<Atom>();
            foreach (Molecule mol in Molecules.Values)
            {
                mol.BuildAtomList(allAtoms);
            }

            return allAtoms;
        }

        public List<Bond> GetAllBonds()
        {
            List<Bond> allBonds = new List<Bond>();
            foreach (Molecule mol in Molecules.Values)
            {
                mol.BuildBondList(allBonds);
            }

            return allBonds;
        }

        public List<Molecule> GetAllMolecules()
        {
            List<Molecule> allMolecules = new List<Molecule>();
            foreach (Molecule mol in Molecules.Values)
            {
                mol.BuildMolList(allMolecules);
            }

            return allMolecules;
        }

        public void RescaleForCml()
        {
            if (ScaledForXaml)
            {
                double newLength = Globals.SingleAtomPseudoBondLength / Globals.ScaleFactorForXaml;

                if (MeanBondLength > 0)
                {
                    newLength = MeanBondLength / Globals.ScaleFactorForXaml;
                }

                ScaleToAverageBondLength(newLength);

                ScaledForXaml = false;
            }
        }

        public void RescaleForXaml(bool forDisplay)
        {
            if (!ScaledForXaml)
            {
                double newLength = Globals.SingleAtomPseudoBondLength * Globals.ScaleFactorForXaml;

                if (MeanBondLength > 0)
                {
                    newLength = MeanBondLength * Globals.ScaleFactorForXaml;
                }

                ScaleToAverageBondLength(newLength);

                ScaledForXaml = true;

                XamlBondLength = newLength;
                OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(BoundingBox)));
                OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(XamlBondLength)));

                if (forDisplay)
                {
                    RepositionAll(MinX, MinY);
                }
            }
        }

        public void CheckIntegrity()
        {
            var mols = GetAllMolecules();
            foreach (Molecule mol in mols)
            {
                mol.CheckIntegrity();
            }
        }
    }

    #endregion Methods
}