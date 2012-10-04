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
    public class GeneratorMeasureSetting
    {
        private string _name;
        private object _setting;

        public GeneratorMeasureSetting(string name, object setting)
        {
            _name = name;
            _setting = setting;
        }

        public string Name
        {
            get { return _name; }
        }

        public object Setting
        {
            get { return _setting; }
            set { _setting = value; }
        }

    }
}
