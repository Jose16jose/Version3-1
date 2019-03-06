﻿// ---------------------------------------------------------------------------
//  Copyright (c) 2019, The .NET Foundation.
//  This software is released under the Apache License, Version 2.0.
//  The license and further copyright text can be found in the file LICENSE.md
//  at the root directory of the distribution.
// ---------------------------------------------------------------------------

using System;
using Chem4Word.Model2.Helpers;
using Newtonsoft.Json;

namespace Chem4Word.Model2
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Group
    {
        [JsonProperty]
        public string Component { get; set; }

        [JsonProperty]
        public int Count { get; set; }

        public Group(string e, int c)
        {
            Component = e;
            Count = c;
        }

        public override string ToString()
        {
            return $"{Component} * {Count}";
        }

        /// <summary>
        /// Calculated combined AtomicWeight
        /// </summary>
        public double AtomicWeight
        {
            get
            {
                if (Globals.PeriodicTable.HasElement(Component))
                {
                    return ((Element)Globals.PeriodicTable[Component]).AtomicWeight * Count;
                }
                else
                {
                    FunctionalGroup fg = Globals.FunctionalGroupsDictionary[Component];
                    if (fg != null)
                    {
                        return Globals.FunctionalGroupsDictionary[Component].AtomicWeight * Count;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
    }
}