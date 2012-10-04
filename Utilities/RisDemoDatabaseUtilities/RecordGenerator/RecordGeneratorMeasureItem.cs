#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator
{
    public class RecordGeneratorMeasureItem
    {
        private string _label;
        private double _value;
        private DateTime _timeOfMeasure;
        private string _info;

        public RecordGeneratorMeasureItem(DateTime TimeOfMeasure, string label, string info, double measureValue)
        {
            _label = label;
            _value = measureValue;
            _timeOfMeasure = TimeOfMeasure;
            _info = info;
        }

        public string Label
        {
            get { return _label; }
        }

        public string Info
        {
            get { return _info; }
        }

        public double MeasureValue
        {
            get { return _value; }
        }

        public DateTime TimeOfMeasure
        {
            get { return _timeOfMeasure; }
        }
    }
}
