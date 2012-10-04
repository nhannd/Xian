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

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities
{
    public class EntityGeneratorSetting
    {
        private string _name;
        private object _value;

        public EntityGeneratorSetting(string name, object value)
        {
            _name = name;
            _value = value;
        }

        public string Name 
        {
            get { return _name; }
        }

        public object Setting 
        {
            get { return _value; }
            set { _value = value; } 
        } 
    }
}
