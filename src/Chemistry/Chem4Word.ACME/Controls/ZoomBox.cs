﻿// ---------------------------------------------------------------------------
//  Copyright (c) 2019, The .NET Foundation.
//  This software is released under the Apache License, Version 2.0.
//  The license and further copyright text can be found in the file LICENSE.md
//  at the root directory of the distribution.
// ---------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Chem4Word.ACME.Controls
{
    public class ZoomBox : Control
    {
        private Thumb _zoomThumb;
        private Canvas _zoomCanvas;
        private Slider _zoomSlider;
        private ScaleTransform _transform;
        private double _scale;
        #region DPs

        #region ScrollViewer

        public ScrollViewer ScrollViewer
        {
            get => (ScrollViewer)GetValue(ScrollViewerProperty);
            set => SetValue(ScrollViewerProperty, value);
        }

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(ZoomBox));

        #endregion ScrollViewer

        #region ChemistryCanvas

        public static readonly DependencyProperty ChemistryCanvasProperty =
            DependencyProperty.Register("ChemistryCanvas", typeof(ChemistryCanvas), typeof(ZoomBox),
                new FrameworkPropertyMetadata(null,
                    OnChemistryCanvasChanged));

        public ChemistryCanvas ChemistryCanvas
        {
            get => (ChemistryCanvas)GetValue(ChemistryCanvasProperty);
            set => SetValue(ChemistryCanvasProperty, value);
        }

        private static void OnChemistryCanvasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomBox target = (ZoomBox)d;
            ChemistryCanvas oldDesignerCanvas = (ChemistryCanvas)e.OldValue;
            ChemistryCanvas newDesignerCanvas = target.ChemistryCanvas;
            target.OnChemistryCanvasChanged(oldDesignerCanvas, newDesignerCanvas);
        }

        protected virtual void OnChemistryCanvasChanged(ChemistryCanvas oldDesignerCanvas, ChemistryCanvas newDesignerCanvas)
        {
            if (oldDesignerCanvas != null)
            {
                newDesignerCanvas.LayoutUpdated -= DesignerCanvas_LayoutUpdated;
                newDesignerCanvas.MouseWheel -= DesignerCanvas_MouseWheel;
            }

            if (newDesignerCanvas != null)
            {
                newDesignerCanvas.LayoutUpdated += DesignerCanvas_LayoutUpdated;
                newDesignerCanvas.MouseWheel += DesignerCanvas_MouseWheel;
                newDesignerCanvas.LayoutTransform = ChemistryCanvas.LayoutTransform;
            }
        }

        #endregion ChemistryCanvas

        #endregion DPs

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ScrollViewer == null)
            {
                return;
            }

            _zoomThumb = Template.FindName("PART_ZoomThumb", this) as Thumb;
            if (_zoomThumb == null)
            {
                Debugger.Break();
                throw new Exception("PART_ZoomThumb template is missing!");
            }

            _zoomCanvas = Template.FindName("PART_ZoomCanvas", this) as Canvas;
            if (_zoomCanvas == null)
            {
                Debugger.Break();
                throw new Exception("PART_ZoomCanvas template is missing!");
            }

            _zoomSlider = Template.FindName("PART_ZoomSlider", this) as Slider;
            if (_zoomSlider == null)
            {
                Debugger.Break();
                throw new Exception("PART_ZoomSlider template is missing!");
            }

            _zoomThumb.DragDelta += Thumb_DragDelta;
            _zoomSlider.ValueChanged += ZoomSlider_ValueChanged;
            _transform = new ScaleTransform();
            ChemistryCanvas.LayoutTransform = _transform;
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double scale = e.NewValue / e.OldValue;
            double halfViewportHeight = ScrollViewer.ViewportHeight / 2;
            double halfViewportWidth = ScrollViewer.ViewportWidth / 2;

            _transform.ScaleX *= scale;
            _transform.ScaleY *= scale;

            double newVerticalOffset = ((ScrollViewer.VerticalOffset + halfViewportHeight) * scale - halfViewportHeight);
            
            double newHorizontalOffset = ((ScrollViewer.HorizontalOffset + halfViewportWidth) / scale - halfViewportWidth);

            ScrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);
            ScrollViewer.ScrollToVerticalOffset(newVerticalOffset);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //InvalidateScale(out var scale, out _, out _);
            ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset + e.HorizontalChange / _scale);
            ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + e.VerticalChange / _scale);
        }

        private void DesignerCanvas_LayoutUpdated(object sender, EventArgs e)
        {
            InvalidateScale(out _scale, out var xOffset, out var yOffset);
            _zoomThumb.Width = ScrollViewer.ViewportWidth * _scale;
            _zoomThumb.Height = ScrollViewer.ViewportHeight * _scale;
            Canvas.SetLeft(_zoomThumb, xOffset + ScrollViewer.HorizontalOffset * _scale);
            Canvas.SetTop(_zoomThumb, yOffset + ScrollViewer.VerticalOffset * _scale);
        }

        private void DesignerCanvas_MouseWheel(object sender, EventArgs e)
        {
            MouseWheelEventArgs wheel = (MouseWheelEventArgs)e;

            //divide the value by 15 so that it is smoother

            double value = wheel.Delta / 15;
            if (Math.Abs(value) > 10d)
            {
                value = 10 * Math.Sign(value);
            }

            _zoomSlider.Value += value;
        }

        private void InvalidateScale(out double scale, out double xOffset, out double yOffset)
        {
            double w = ChemistryCanvas.ActualWidth * _transform.ScaleX;
            double h = ChemistryCanvas.ActualHeight * _transform.ScaleY;

            // zoom canvas size
            double x = _zoomCanvas.ActualWidth;
            double y = _zoomCanvas.ActualHeight;
            double scaleX = x / w;
            double scaleY = y / h;
            scale = (scaleX < scaleY) ? scaleX : scaleY;
            xOffset = (x - scale * w) / 2;
            yOffset = (y - scale * h) / 2;
        }
    }
}