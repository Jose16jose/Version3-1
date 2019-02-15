﻿// ---------------------------------------------------------------------------
//  Copyright (c) 2019, The .NET Foundation.
//  This software is released under the Apache License, Version 2.0.
//  The license and further copyright text can be found in the file LICENSE.md
//  at the root directory of the distribution.
// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Chem4Word.Model2.Geometry
{
    /// See https://www.topcoder.com/community/data-science/data-science-tutorials/geometry-concepts-line-intersection-and-its-applications/
    /// for implementing some good basic operations in geometry
    /// </summary>

    public static class BasicGeometry
    {
        /// <summary>
        /// gets signed angle between three points
        /// direction is anticlockwise
        /// example:
        /// GetAngle(new Point2(1,0), new Point2(0,0), new Point2(0,1)) => Math.PI/2
        /// GetAngle(new Point2(-1,0), new Point2(0,0), new Point2(0,1)) => -Math.PI/2
        /// GetAngle(new Point2(0,1), new Point2(0,0), new Point2(1,0)) => -Math.PI/2
        ///
        /// </summary>
        /// <param name="point0">first point</param>
        /// <param name="point1">centre point</param>
        /// <param name="point2">final point</param>
        /// <param name="epsilon"></param>
        /// <exception cref="ArgumentException">if any atoms are coincident</exception>
        /// <returns>null if any points are null</returns>
        /// <summary>
        public static double? GetAngle(Point? point0, Point? point1, Point? point2, double epsilon)
        {
            double? angle = null;

            if (point0 != null && point1 != null && point2 != null)
            {
                if ((point1 - point0).Value.Length < epsilon || (point2 - point2).Value.Length < epsilon)
                {
                    Debugger.Break();
                    throw new ArgumentException("coincident points in GetAngle");
                }

                Vector from = point0.Value - point1.Value;
                Vector to = point2.Value - point1.Value;
                angle = Vector.AngleBetween(from, to);
            }

            return angle;
        }

        /// <summary>
        /// AngleBetween - the angle between 2 vectors
        /// </summary>
        /// <returns>
        /// Returns the the angle in degrees between vector1 and vector2
        /// </returns>
        /// <param name="vector1"> The first Vector </param>
        /// <param name="vector2"> The second Vector </param>
        public static double AngleBetween(Vector vector1, Vector vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }

        #region extension methods

        public static Vector Perpendicular(this Vector v)
        {
            return new Vector(-v.Y, v.X);
        }

        public static Vector ScreenSouth = new Vector(0, 1);

        public static Vector ScreenEast = new Vector(1, 0);

        public static Vector ScreenNorth = -ScreenSouth;

        public static Vector ScreenWest = -ScreenEast;

        #endregion extension methods

        public static double Determinant(Vector vector1, Vector vector2)
        {
            return vector1.X * vector2.Y - vector1.Y * vector2.X;
        }

        /// <summary>
        /// Determines whether two line segments intersect.
        /// Used mainly for determining cis/trans geometry of double bonds
        /// </summary>
        /// <param name="segment1Start">Point at which first segment starts</param>
        /// <param name="segment1End">Point at which first segment ends</param>
        /// <param name="segment2Start">Point at which second segment starts</param>
        /// <param name="segment2End">Point at which second segment ends</param>
        /// <returns>Point at which both lines intersect, null if otherwise</returns>
        public static Point? LineSegmentsIntersect(Point segment1Start, Point segment1End, Point segment2Start, Point segment2End)
        {
            double t;
            double u;
            IntersectLines(out t, out u, segment1Start, segment1End, segment2Start, segment2End);
            if ((t >= 0) && (u >= 0) && (t <= 1) && (u <= 1)) //voila, we have an intersection
            {
                Vector vIntersect = (segment1End - segment1Start) * t;
                return segment1Start + vIntersect;
            }
            return null;
        }

        /// <summary>
        /// intersects two straight line segments.  Returns two values that indicate
        /// how far along the segments the intersection takes place.
        /// Values between 0 and 1 for both segments indicate the lines cross
        /// Values between 0 and 1 for ONE segment indicates that the projection
        /// of the other segment intersects it
        /// </summary>
        /// <param name="t">proportion along the line of the first segment</param>
        /// <param name="u">proportion along the line of the second segment</param>
        /// <param name="segment1Start">what it says</param>
        /// <param name="segment1End">what it says</param>
        /// <param name="segment2Start">what it says</param>
        /// <param name="segment2End">what it says</param>
        ///
        public static void IntersectLines(out double t, out double u, Point segment1Start, Point segment1End, Point segment2Start,
            Point segment2End)
        {
            double det = Determinant(segment1End - segment1Start, segment2Start - segment2End);
            t = Determinant(segment2Start - segment1Start, segment2Start - segment2End) / det;
            u = Determinant(segment1End - segment1Start, segment2Start - segment1Start) / det;
        }

        // ReSharper disable once InconsistentNaming
        public static CompassPoints SnapTo2EW(double angleFromNorth)
        {
            if (angleFromNorth >= 0 || angleFromNorth <= -180)
            {
                return CompassPoints.East;
            }

            return CompassPoints.West;
        }

        // ReSharper disable once InconsistentNaming
        public static CompassPoints SnapTo4NESW(double angleFromNorth)
        {
            if (angleFromNorth >= -45 && angleFromNorth <= 45)
            {
                return CompassPoints.North;
            }

            if (angleFromNorth > 45 && angleFromNorth < 135)
            {
                return CompassPoints.East;
            }

            if (angleFromNorth > -135 && angleFromNorth < -45)
            {
                return CompassPoints.West;
            }

            return CompassPoints.South;
        }

        public static int SnapToClock(double angleFromNorth)
        {
            int tolerance = 15;
            var sector = SnapAngleToTolerance(angleFromNorth, tolerance);
            return sector;
        }

        private static int SnapAngleToTolerance(double angleFromNorth, int tolerance)
        {
            if (angleFromNorth < 0)
            {
                angleFromNorth += 360.0;
            }
            double adjustedAngle = angleFromNorth + tolerance;
            int sector = (int)adjustedAngle / (tolerance * 2);
            return sector;
        }

        /// <summary>
        /// Takes a list of poinst and builds a  Path object from it.
        /// Generally used for constructing masks
        /// </summary>
        /// <param name="hull">List of points making up the path </param>
        /// <returns></returns>
        public static Path BuildPath(List<Point> hull, bool isClosed = true)
        {
            var points = hull.ToArray();

            Path path = new Path
            {
                StrokeThickness = 0.0,
            };

            if (points.Length == 0)
            {
                return path;
            }

            PathSegmentCollection pathSegments = new PathSegmentCollection();
            for (int i = 1; i < points.Length; i++)
            {
                pathSegments.Add(new LineSegment(points[i], true));
            }
            path.Data = new PathGeometry
            {
                Figures = new PathFigureCollection
                {
                    new PathFigure
                    {
                        StartPoint = points[0],
                        Segments = pathSegments,
                        IsClosed = isClosed,
                        IsFilled = true
                    }
                }
            };

            return path;
        }

        public static StreamGeometry BuildPolyPath(List<Point> hull, bool isClosed = true)
        {
            var points = hull.ToArray();
            StreamGeometry geo = new StreamGeometry();
            using (StreamGeometryContext c = geo.Open())
            {
                c.BeginFigure(points[0], true, isClosed);
                c.PolyLineTo(points.Skip(1).ToArray(), true, true);
                c.Close();
            }

            return geo;
        }

        public static System.Windows.Media.Geometry CreateGeometry(DrawingGroup drawingGroup)
        {
            var geometry = new GeometryGroup();

            foreach (var drawing in drawingGroup.Children)
            {
                if (drawing is GeometryDrawing)
                {
                    geometry.Children.Add(((GeometryDrawing)drawing).Geometry);
                }
                else if (drawing is GlyphRunDrawing)
                {
                    geometry.Children.Add(((GlyphRunDrawing)drawing).GlyphRun.BuildGeometry());
                }
                else if (drawing is DrawingGroup)
                {
                    geometry.Children.Add(CreateGeometry((DrawingGroup)drawing));
                }
            }

            geometry.Transform = drawingGroup.Transform;
            return geometry;
        }
    }
}