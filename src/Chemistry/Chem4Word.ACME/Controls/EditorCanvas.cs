﻿// ---------------------------------------------------------------------------
//  Copyright (c) 2019, The .NET Foundation.
//  This software is released under the Apache License, Version 2.0.
//  The license and further copyright text can be found in the file LICENSE.md
//  at the root directory of the distribution.
// ---------------------------------------------------------------------------

using Chem4Word.ACME.Drawing;
using Chem4Word.Model2;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using static Chem4Word.ACME.Drawing.BondVisual;
using static Chem4Word.Model2.Geometry.BasicGeometry;

namespace Chem4Word.ACME.Controls
{
    public class EditorCanvas : ChemistryCanvas
    {
        #region Constructors

        public EditorCanvas() : base()
        {
        }

        #endregion Constructors



        #region Methods

        public Rect GetMoleculeBoundingBox(Molecule mol)
        {
            Rect union = Rect.Empty;
            var atomList = new List<Atom>();

            mol.BuildAtomList(atomList);
            foreach (Atom atom in atomList)
            {
                union.Union(chemicalVisuals[atom].ContentBounds);
            }

            return union;
        }

        public Rect GetMoleculeBoundingBox(List<Molecule> adornedMolecules)
        {
            Rect union = Rect.Empty;
            foreach (Molecule molecule in adornedMolecules)
            {
                union.Union(GetMoleculeBoundingBox(molecule));
            }

            return union;
        }

        /// <summary>
        /// Draws a 'ghost image' of the selected molecules.
        /// Useful in visual operations
        /// </summary>
        /// <param name="adornedMolecules">List of molecules to ghost</param>
        /// <returns></returns>
        public Geometry GhostMolecule(List<Molecule> adornedMolecules, Transform operation = null)
        {
            var atomList = new List<Atom>();
            List<Bond> bondList = new List<Bond>();
            foreach (Molecule mol in adornedMolecules)
            {
                mol.BuildAtomList(atomList);
            }

            return PartialGhost(atomList, operation);
        }

        /// <summary>
        /// Use for 'rubber-banding' atoms during drag operations
        /// </summary>
        /// <param name="selectedAtoms">List of selected atoms to rubber-band</param>
        /// <param name="shear">TranslateTransform describing how atoms are shifted. Omit to obtain an overlay to transform later</param>
        /// <returns>Geometry of deformed atoms</returns>
        public Geometry PartialGhost(List<Atom> selectedAtoms, Transform shear = null)
        {
            //var neighbourSet = new HashSet<Atom>();
            HashSet<Bond> bondSet = new HashSet<Bond>();
            Dictionary<Atom, Point> transformedPositions = new Dictionary<Atom, Point>();
            //compile a set of all the neighbours of the selected atoms
            foreach (Atom atom in selectedAtoms)
            {
                foreach (Atom neighbour in atom.Neighbours)
                {
                    //add in all the existing position for neigbours not in selected atoms
                    if (!selectedAtoms.Contains(neighbour))
                    {
                        //neighbourSet.Add(neighbour); //don't worry about adding them twice
                        transformedPositions[neighbour] = neighbour.Position;
                    }
                }
                //add in the bonds
                foreach (Bond bond in atom.Bonds)
                {
                    bondSet.Add(bond);//don't worry about adding them twice
                }
                //and while we're at it, work out the new locations

                //if we're just getting an overlay then don't bother transforming
                if (shear != null)
                {
                    transformedPositions[atom] = shear.Transform(atom.Position);
                }
                else
                {
                    transformedPositions[atom] = atom.Position;
                }
            }

            StreamGeometry ghostGeometry = new StreamGeometry();

            double atomRadius = this.Chemistry.Model.XamlBondLength / 7.50;
            using (StreamGeometryContext ghostContext = ghostGeometry.Open())
            {
                Dictionary<Atom, Geometry> atomGeometries = new Dictionary<Atom, Geometry>();

                foreach (Atom atom in transformedPositions.Keys)
                {
                    var newPosition = transformedPositions[atom];

                    if (atom.SymbolText != "")
                    {
                        EllipseGeometry atomCircle = new EllipseGeometry(newPosition, atomRadius, atomRadius);
                        DrawGeometry(ghostContext, atomCircle);
                        atomGeometries[atom] = atomCircle;
                    }
                    else
                    {
                        atomGeometries[atom] = Geometry.Empty;
                    }
                }
                foreach (Bond bond in bondSet)
                {

                    List<Point> throwaway = new List<Point>();
                    var startAtomPosition = transformedPositions[bond.StartAtom];
                    var endAtomPosition = transformedPositions[bond.EndAtom];
                    var startAtomVisual = (AtomVisual)(chemicalVisuals[bond.StartAtom]) ;
                    var endAtomVisual = (AtomVisual)(chemicalVisuals[bond.EndAtom]);
                    var descriptor = GetBondDescriptor(startAtomVisual, endAtomVisual, Chemistry.Model.XamlBondLength,
                                                       bond.Stereo, startAtomPosition, endAtomPosition, bond.OrderValue,
                                                       bond.Placement, bond.Centroid, bond.SubsidiaryRing?.Centroid);
                    descriptor.Start = startAtomPosition;
                    descriptor.End = endAtomPosition;
                    var bondgeom =  descriptor.DefiningGeometry;
                    DrawGeometry(ghostContext, bondgeom);
                }
                ghostContext.Close();
            }

            return ghostGeometry;
        }


        #endregion Methods

        #region Overrides

        /// <summary>
        /// Doesn't autosize the chemistry to fit, unlike the display
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            return DesiredSize;
        }

        #endregion Overrides

        public AtomVisual GetAtomVisual(Atom adornedAtom)
        {
            return chemicalVisuals[adornedAtom] as AtomVisual;
        }
    }
}