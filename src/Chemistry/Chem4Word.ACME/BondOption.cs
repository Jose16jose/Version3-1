﻿// ---------------------------------------------------------------------------
//  Copyright (c) 2019, The .NET Foundation.
//  This software is released under the Apache License, Version 2.0.
//  The license and further copyright text can be found in the file LICENSE.md
//  at the root directory of the distribution.
// ---------------------------------------------------------------------------


using System;
using System.Windows;
using Chem4Word.Model2.Helpers;

namespace Chem4Word.ACME
{
    /// <summary>
    /// Rolls up Bond Stereo and Order into a single class to facilitate binding.
    /// Deals with combinations of Orders and Stereo
    /// </summary>
    public class BondOption : DependencyObject
    {
        public int Id
        {
            get { return (int)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(BondOption), new PropertyMetadata(default(int)));

        public String Order
        {
            get { return (string)GetValue(OrderProperty); }
            set { SetValue(OrderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Order.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrderProperty =
            DependencyProperty.Register("Order", typeof(string), typeof(BondOption), new PropertyMetadata(Globals.OrderSingle));

        public Globals.BondStereo? Stereo
        {
            get { return (Globals.BondStereo?)GetValue(BondStereoEnumsProperty); }
            set { SetValue(BondStereoEnumsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BondStereoEnums.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BondStereoEnumsProperty =
            DependencyProperty.Register("Stereo", typeof(Globals.BondStereo?), typeof(BondOption), new PropertyMetadata(null));

        public Style DisplayStyle
        {
            get { return (Style)GetValue(DisplayStyleProperty); }
            set { SetValue(DisplayStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayStyleProperty =
            DependencyProperty.Register("DisplayStyle", typeof(Style), typeof(BondOption), new PropertyMetadata(null));

        public override string ToString()
        {
            return $"{Order} - {Stereo}";
        }
    }
}