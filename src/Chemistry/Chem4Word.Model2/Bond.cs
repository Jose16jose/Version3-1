﻿// ---------------------------------------------------------------------------
//  Copyright (c) 2019, The .NET Foundation.
//  This software is released under the Apache License, Version 2.0.
//  The license and further copyright text can be found in the file LICENSE.md
//  at the root directory of the distribution.
// ---------------------------------------------------------------------------

using Chem4Word.Model2.Annotations;
using Chem4Word.Model2.Geometry;
using Chem4Word.Model2.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml.Linq;
using static Chem4Word.Model2.Helpers.CML;
using static Chem4Word.Model2.Helpers.Globals;

namespace Chem4Word.Model2
{
    public class Bond : ChemistryBase, INotifyPropertyChanged
    {
        #region Properties

        public string EndAtomID
        {
            get => _endAtomId;
            set => _endAtomId = value;
        }

        public string StartAtomID
        {
            get => _startAtomId;
            set => _startAtomId = value;
        }

        public Atom EndAtom
        {
            get { return Parent.Atoms[EndAtomID]; }
        }

        public Atom StartAtom
        {
            get { return Parent.Atoms[StartAtomID]; }
        }

        public List<Atom> GetAtoms()
        {
            return new List<Atom>() { StartAtom, EndAtom };
        }

        public Molecule Parent { get; set; }

        public Model Model
        {
            get { return Parent.Model; }
        }

        public List<Ring> Rings
        {
            get
            {
                List<Ring> result = new List<Ring>();
                foreach (Ring parentRing in Parent.Rings)
                {
                    if (parentRing.Atoms.Contains(StartAtom) && parentRing.Atoms.Contains(EndAtom))
                    {
                        result.Add(parentRing);
                    }
                }

                return result;
            }
        }

        public Point MidPoint => new Point((StartAtom.Position.X + EndAtom.Position.X) / 2,
            (StartAtom.Position.Y + EndAtom.Position.Y) / 2);

        public string Id { get; set; }
        internal string InternalId => _internalId;

        public string Path
        {
            get
            {
                if (Parent == null)
                {
                    return Id;
                }
                else
                {
                    return Parent.Path + "/" + Id;
                }
            }
        }

        #region Bond Orders

        private string _order;

        public string Order
        {
            get { return _order; }
            set
            {
                if (value.Equals("0.5"))
                {
                    value = OrderPartial01;
                }
                if (value.Equals("1") || value.Equals("S"))
                {
                    value = OrderSingle;
                }
                if (value.Equals("1.5"))
                {
                    value = OrderPartial12;
                }
                if (value.Equals("2") || value.Equals("D"))
                {
                    value = OrderDouble;
                }
                if (value.Equals("3") || value.Equals("T"))
                {
                    value = OrderTriple;
                }
                if (value.Equals("0"))
                {
                    value = OrderZero;
                }

                _order = value;
                OnPropertyChanged();

                //StartAtom?.ImplicitHChanged();
                //EndAtom?.ImplicitHChanged();
            }
        }

        public double? OrderValue => OrderToOrderValue(Order);

        public static double? OrderToOrderValue(string order)
        {
            switch (order)
            {
                case OrderZero:
                case OrderOther:
                    return 0;

                case OrderPartial01:
                    return 0.5;

                case OrderSingle:
                    return 1;

                case OrderPartial12:
                    return 1.5;

                case OrderAromatic:
                    return 1.5;

                case OrderDouble:
                    return 2;

                case OrderPartial23:
                    return 2.5;

                case OrderTriple:
                    return 3;

                default:
                    return null;
            }
        }

        #endregion Bond Orders

        private Globals.BondStereo _stereo;
        private string _endAtomId;
        private string _startAtomId;

        public Globals.BondStereo Stereo
        {
            get { return _stereo; }
            set
            {
                _stereo = value;
                OnPropertyChanged();
            }
        }

        public object Tag { get; set; }
        public List<string> Messages { get; private set; }

        private Globals.BondDirection? _implicitPlacement = null; //caching variable, can be trashed
        private string _internalId;

        public Globals.BondDirection Placement
        {
            get
            {
                if (OrderValue == 2)
                {
                    //force a recalc of the rings if necessary
                    if (!Parent.RingsCalculated)
                    {
                        Parent.RebuildRings();
                    }
                    return ExplicitPlacement ?? ImplicitPlacement ?? Globals.BondDirection.None;
                }
                else
                {
                    return Globals.BondDirection.None;
                }
            }
            set
            {
                ExplicitPlacement = value;
                OnPropertyChanged();
            }
        }

        public Globals.BondDirection? ExplicitPlacement { get; set; }

        private Vector? VectorOnSideOfNonHAtomFromStartLigands(Atom startAtom, Atom endAtom, IEnumerable<Atom> startLigands)
        {
            Vector posDisplacementVector = BondVector.Perpendicular();
            Vector negDisplacementVector = -posDisplacementVector;
            posDisplacementVector.Normalize();
            negDisplacementVector.Normalize();

            posDisplacementVector = posDisplacementVector * 3;
            negDisplacementVector = negDisplacementVector * 3;

            Point posEndPoint = endAtom.Position + posDisplacementVector;
            Point negEndPoint = endAtom.Position + negDisplacementVector;

            Atom nonHAtom = startAtom.Neighbours.FirstOrDefault(n => n != endAtom && (Element)n.Element != Globals.PeriodicTable.H);
            if (nonHAtom != null)
            {
                Point nonHAtomLoc = nonHAtom.Position;

                double posDist = (nonHAtomLoc - posEndPoint).Length;
                double negDist = (nonHAtomLoc - negEndPoint).Length;

                bool posDisplacement = posDist < negDist;
                Vector displacementVector = posDisplacement ? posDisplacementVector : negDisplacementVector;

                return displacementVector;
            }

            return null;
        }

        public Ring PrimaryRing
        {
            get
            {
                //Debug.Assert(Parent.RingsCalculated);
                if (!Rings.Any()) //no rings
                {
                    return null;
                }
                else
                {
                    List<Ring> ringList = Parent.SortedRings;
                    var firstRing = (
                        from Ring r in ringList
                        where r.Atoms.Contains(this.StartAtom) && r.Atoms.Contains(this.EndAtom)
                        select r
                    ).FirstOrDefault();
                    return firstRing;
                }
            }
        }

        public bool IsCyclic()
        {
            return Rings.Any();
        }

        public Vector? GetPrettyCyclicDoubleBondVector()
        {
            Debug.Assert(Parent.RingsCalculated);

            Vector? vector = null;

            if (PrimaryRing != null)
            {
                List<Ring> ringList = Rings.Where(x => x.Priority > 0).OrderBy(x => x.Priority).ToList();

                if (ringList.Any()) //no rings
                {
                    Point? ringCentroid = PrimaryRing.Centroid;
                    vector = ringCentroid - MidPoint;
                }
            }

            return vector;
        }

        public Vector? GetPrettyDoubleBondVector()
        {
            Vector? vector = null;

            if (IsCyclic())
            {
                return GetPrettyCyclicDoubleBondVector();
            }

            // We're acyclic.

            //GetAtomAndLigandIDs(0, out StartAtom, out a1LigandIDs);

            var startLigands = (from Atom a in StartAtom.Neighbours
                                where a != EndAtom
                                select a).ToList();

            if (!startLigands.Any())
            {
                return null;
            }

            var endLigands = (from Atom b in EndAtom.Neighbours
                              where b != StartAtom
                              select b).ToList();

            if (!endLigands.Any())
            {
                return null;
            }

            if (startLigands.Count() > 2 || endLigands.Count() > 2)
            {
                return null;
            }

            if (startLigands.Count() == 2 && endLigands.Count() == 2)
            {
                return null;
            }

            if (startLigands.AreAllH() && endLigands.AreAllH())
            {
                return null;
            }

            if (startLigands.ContainNoH() && endLigands.ContainNoH())
            {
                return null;
            }

            if (startLigands.GetHCount() == 1 && endLigands.GetNonHCount() == 1)
            {
                if (endLigands.Count() == 2)
                {
                    if (endLigands.GetHCount() == 2 || endLigands.ContainNoH())
                    {
                        //Double sided bond on the side of the non H atom from StartLigands
                        //Elbow bond :¬)
                        //DrawElbowBond(StartAtom);
                        return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, startLigands);
                    }
                    // Now must have 1 H and 1 !H
                    if (AtomsAreCis(startLigands.GetFirstNonH(), endLigands.GetFirstNonH())
                        /*if a2a H on the same side as a1a H*/)
                    {
                        //double bond on the side of non H
                        return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, startLigands);
                    }

                    //Now must be a trans bond.
                    return null;
                }
                else
                {
                    //Count now 1
                    if (endLigands.GetHCount() == 1)
                    {
                        //Double bond on the side of non H from StartLigands, bevel 1 end.
                        return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, startLigands);
                    }

                    //Now !H
                    if (AtomsAreCis(startLigands.GetFirstNonH(), endLigands.GetFirstNonH())
                        /*EndAtomAtom's !H is on the same side as StartAtomAtom's !H*/)
                    {
                        //double bond on the side of !H from StartLigands, bevel both ends
                        return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, startLigands);
                    }

                    //Now must be a trans bond.
                    return null;
                }
            }
            else if (startLigands.AreAllH())
            {
                if (endLigands.Count() == 2)
                {
                    if (endLigands.ContainNoH())
                    {
                        return null;
                    }

                    //Must now have 1 H and 1 !H
                    //double bond on the side of EndLigands' !H, bevel 1 end only
                    return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, endLigands);
                }
                //Count must now be 1
                // Must now be 1 !H
                // Double bond on the side of EndLigands' !H, bevel 1 end only.
                return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, endLigands);
            }
            else if (startLigands.GetHCount() == 0)
            {
                if (endLigands.Count() == 2)
                {
                    if (endLigands.AreAllH())
                    {
                        return null;
                    }
                    if (endLigands.ContainNoH())
                    {
                        return null;
                    }
                    // Now must have 1 H and 1 !H
                    //Double bond on the side of EndLigands' !H, bevel both ends.
                    return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, endLigands);
                }
                //Count is 1
                if (endLigands.GetHCount() == 1)
                {
                    return null;
                }

                if (endLigands.GetHCount() == 0)
                {
                    //double bond on the side of EndLigands' !H, bevel both ends.
                    return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, endLigands);
                }
            }
            // StartLigands' count = 1
            else if (startLigands.GetHCount() == 1)
            {
                if (endLigands.Count() == 2)
                {
                    if (endLigands.AreAllH())
                    {
                        // Already caught.
                        // ToDo: Check with Clyde
                        //throw new ApplicationException("This scenario should already have been caught");
                        return null;
                    }
                    if (endLigands.ContainNoH())
                    {
                        return null;
                    }

                    //Now EndLigands contains 1 H and 1 !H
                    //double bond on side of EndLigands' !H, bevel 1 end
                    return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, endLigands);
                }

                if (endLigands.AreAllH())
                {
                    // Already caught.
                    // ToDo: Check with Clyde
                    //throw new ApplicationException("This scenario should already hve been caught");
                    return null;
                }
            }
            else if (startLigands.GetHCount() == 0)
            {
                if (endLigands.Count() == 2)
                {
                    if (endLigands.AreAllH())
                    {
                        //Double dbond on the side of StartLigands' !H, bevel one end.
                        return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, startLigands);
                    }
                    if (endLigands.ContainNoH())
                    {
                        //double bond on the side of StartLigands' !H, bevel both end.
                        return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, startLigands);
                    }

                    //Now EndLigands contains 1 H and 1 !H
                    if (AtomsAreCis(startLigands.GetFirstNonH(), endLigands.GetFirstNonH())
                        /* EndLigands' !H is on the same side as StartAtomAtom's !H */)
                    {
                        //double bond on the side of StartLigands' !H, bevel both ends
                        return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, startLigands);
                    }

                    // Must be trans
                    return null;
                }

                // atoms2Atoms length = 1
                if (endLigands.GetHCount() == 1)
                {
                    //double bond on side of StartLigands' !H, bevel one end.
                    return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, startLigands);
                }

                // EndLigands must contain 1 !H
                if (AtomsAreCis(startLigands.GetFirstNonH(), endLigands.GetFirstNonH())
                    /* EndLigands' !H is on same side as StartLigands' !H */)
                {
                    //double bond on side of StartLigands' !H, bevel both ends
                    return VectorOnSideOfNonHAtomFromStartLigands(StartAtom, EndAtom, startLigands);
                }

                //Must be trans
                return null;
            }

            return vector;
        }

        private Globals.BondDirection? GetPlacement()
        {
            Globals.BondDirection dir = Globals.BondDirection.None;

            var vec = GetPrettyDoubleBondVector();

            if (vec == null)
            {
                dir = Globals.BondDirection.None;
            }
            else
            {
                dir = (Globals.BondDirection)Math.Sign(Vector.CrossProduct(vec.Value, BondVector));
            }

            return dir;
        }

        public Globals.BondDirection? ImplicitPlacement
        {
            get
            {
                if (_implicitPlacement == null) //we haven't even touched this yet
                {
                    _implicitPlacement = GetPlacement(); //so guess what it should be
                }

                return _implicitPlacement;
            }
            internal set { _implicitPlacement = value; }
        }

        public Vector BondVector
        {
            get { return StartAtom.Position - EndAtom.Position; }
        }

        public double HatchScaling => BondVector.Length / (Globals.SingleAtomPseudoBondLength * 2);

        #endregion Properties

        #region Constructors

        public Bond()
        {
            Id = Guid.NewGuid().ToString("D");
            _internalId = Id;
            Messages = new List<string>();
        }

        public Bond(XElement cmlElement) : this()
        {
            string[] atomRefs = cmlElement.Attribute("atomRefs2")?.Value.Split(' ');
            if (atomRefs?.Length == 2)
            {
                StartAtomID = atomRefs[0];
                EndAtomID = atomRefs[1];
            }
            var bondRef = cmlElement.Attribute("id")?.Value;
            Id = bondRef ?? Id;
            Order = cmlElement.Attribute("order")?.Value;

            var stereoElems = CML.GetStereo(cmlElement);

            if (stereoElems.Any())
            {
                var stereo = stereoElems[0].Value;

                switch (stereo)
                {
                    case "N":
                        Stereo = Globals.BondStereo.None;
                        break;

                    case "W":
                        Stereo = Globals.BondStereo.Wedge;
                        break;

                    case "H":
                        Stereo = Globals.BondStereo.Hatch;
                        break;

                    case "S":
                        Stereo = Globals.BondStereo.Indeterminate;
                        break;

                    case "C":
                        Stereo = Globals.BondStereo.Cis;
                        break;

                    case "T":
                        Stereo = Globals.BondStereo.Trans;
                        break;

                    default:
                        Stereo = Globals.BondStereo.None;
                        break;
                }
            }
            Globals.BondDirection? dir = null;

            var dirAttr = cmlElement.Attribute(c4w + "placement");
            if (dirAttr != null)
            {
                Globals.BondDirection temp;

                if (Enum.TryParse(dirAttr.Value, out temp))
                {
                    dir = temp;
                }
            }

            if (dir != null)
            {
                Placement = dir.Value;
            }
        }

        #endregion Constructors

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region Methods

        public Atom OtherAtom(Atom a)
        {
            return OtherAtom(a.Id);
        }

        private Atom OtherAtom(string aId)
        {
            return Parent?.Atoms[OtherAtomID(aId)];
        }

        private string OtherAtomID(string aId)
        {
            if (aId.Equals(StartAtomID))
            {
                return EndAtomID;
            }
            else if (aId.Equals(EndAtomID))
            {
                return StartAtomID;
            }
            else
            {
                throw new ArgumentException("Atom ID is not part of this Bond.", aId);
            }
        }

        #region Geometry Routines

        public bool AtomsAreCis(Atom atomA, Atom atomB)
        {
            //do an assert to make sure that we're not calling this routine with atoms detached from the bond atoms
            //Debug.Assert(StartAtom.Neighbours.Contains(atomA) & EndAtom.Neighbours.Contains(atomB)|| StartAtom.Neighbours.Contains(atomB) & EndAtom.Neighbours.Contains(atomA));

            // Note: Add null checks as this has been found to be blowing up
            if (atomA != null && atomB != null
                              && StartAtom.Neighbours != null & EndAtom.Neighbours != null
                              && StartAtom.Neighbours.Count() > 0 && EndAtom.Neighbours.Count() > 0)
            {
                if (StartAtom.Neighbours.Contains(atomA))
                {
                    //draw two lines from the end atom to atom a and start atom to atom b and see if they intersect
                    return BasicGeometry.LineSegmentsIntersect(EndAtom.Position, atomA.Position,
                               StartAtom.Position, atomB.Position) != null;
                }
                else
                {
                    //draw the lines the other way around
                    return BasicGeometry.LineSegmentsIntersect(EndAtom.Position, atomB.Position,
                               StartAtom.Position, atomA.Position) != null;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion Geometry Routines

        #endregion Methods

        #region Overrides

        public override string ToString()
        {
            return $"Bond {Id} - {InternalId}: From {StartAtomID} to {EndAtomID}";
        }

        #endregion Overrides

        public Bond Clone()
        {
            return new Bond().CloneExcept(this, new[] { "Id" });
        }
    }
}