﻿// ---------------------------------------------------------------------------
//  Copyright (c) 2019, The .NET Foundation.
//  This software is released under the Apache License, Version 2.0.
//  The license and further copyright text can be found in the file LICENSE.md
//  at the root directory of the distribution.
// ---------------------------------------------------------------------------

using Chem4Word.ACME.Drawing;
using Chem4Word.Model2.Geometry;
using Chem4Word.Model2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Chem4Word.ACME.Adorners
{
    public class RingDrawer
    {
        private FixedRingAdorner _fixedRingAdorner;

        public RingDrawer(FixedRingAdorner fixedRingAdorner)
        {
            _fixedRingAdorner = fixedRingAdorner;
        }

        public void DrawNRing(DrawingContext drawingContext, List<NewAtomPlacement> newPlacements)
        {
            StreamGeometry ringGeometry = new StreamGeometry();
            if (_fixedRingAdorner.Unsaturated) //bit complicated as we have unsaturated bonds
            {
                using (var sgc = ringGeometry.Open())
                {
                    int newPlacementsCount = newPlacements.Count;

                    var locations = (from p in newPlacements.ToArray().Reverse()
                                     select p.Position).ToArray();
                    HashSet<NewAtomPlacement> visited = new HashSet<NewAtomPlacement>();
                    Point? centroid = Geometry<Point>.GetCentroid(locations, p => p);

                    var startAt =
                        newPlacementsCount % 2; //place the double bonds in odd membered rings where they should start

                    for (int i = startAt; i < newPlacementsCount + startAt; i++)
                    {
                        int firstIndex = i % newPlacementsCount;
                        var oldAtomPlacement = newPlacements[firstIndex];
                        int secondIndex = (firstIndex + 1) % newPlacementsCount;

                        var newAtomPlacement = newPlacements[secondIndex];

                        if (!visited.Contains(oldAtomPlacement) & !visited.Contains(newAtomPlacement) &&
                            !(oldAtomPlacement.ExistingAtom?.IsUnsaturated ??
                              false) && !(newAtomPlacement.ExistingAtom?.IsUnsaturated ?? false))
                        {
                            List<Point> dummy = new List<Point>();
                            BasicGeometry.DrawGeometry(sgc, Drawing.BondGeometry.DoubleBondGeometry(
                                                           oldAtomPlacement.Position,
                                                           newAtomPlacement.Position,
                                                           Math.Abs((newAtomPlacement.Position -
                                                                     oldAtomPlacement.Position).Length),
                                                           Globals.BondDirection.Clockwise,
                                                           ref dummy, centroid));
                            visited.Add(oldAtomPlacement);
                            visited.Add(newAtomPlacement);
                        }
                        else
                        {
                            BasicGeometry.DrawGeometry(
                                sgc,
                                BondGeometry.SingleBondGeometry(oldAtomPlacement.Position, newAtomPlacement.Position));
                        }

                        oldAtomPlacement = newAtomPlacement;
                    }

                    sgc.Close();
                }

                drawingContext.DrawGeometry(null, _fixedRingAdorner.BondPen, ringGeometry);
            }
            else //saturated ring, just draw a polygon
            {
                drawingContext.DrawGeometry(null, _fixedRingAdorner.BondPen, BasicGeometry.BuildPolyPath(_fixedRingAdorner.Placements, true));
            }
        }
    }
}