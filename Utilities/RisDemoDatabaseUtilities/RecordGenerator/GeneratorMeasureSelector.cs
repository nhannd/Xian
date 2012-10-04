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
    public class GeneratorMeasureSelector
    {
        private string _name;
        private bool _selected;

        public GeneratorMeasureSelector(string name, bool selected)
        {
            _name = name;
            _selected = selected;
        }

        public string Name
        {
            get { return _name; }
        }

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }        
    }
}
