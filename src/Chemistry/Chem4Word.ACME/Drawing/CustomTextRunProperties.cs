﻿using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace Chem4Word.ACME.Drawing
{
    partial class CustomTextSource
    {
        public class CustomTextRunProperties : TextRunProperties
        {
            private bool _subscript;
            public CustomTextRunProperties(bool subscript)
            {
                _subscript = subscript;
            }
            public override System.Windows.Media.Brush BackgroundBrush
            {
                get { return null; }
            }

            public override CultureInfo CultureInfo
            {
                get { return CultureInfo.CurrentCulture; }
            }

            public override double FontHintingEmSize
            {
                get { return 22; }
            }

            public override double FontRenderingEmSize
            {
                get { return 22; }
            }

            public override Brush ForegroundBrush
            {
                get { return Brushes.Black; }
            }

            public override System.Windows.TextDecorationCollection TextDecorations
            {
                get { return new System.Windows.TextDecorationCollection(); }
            }

            public override System.Windows.Media.TextEffectCollection TextEffects
            {
                get { return new TextEffectCollection(); }
            }

            public override System.Windows.Media.Typeface Typeface
            {
                get { return new Typeface("Calibri"); }
            }

            public override TextRunTypographyProperties TypographyProperties
            {
                get
                {
                    return new CustomTextRunTypographyProperties(_subscript);
                }
            }

        }
    }
}