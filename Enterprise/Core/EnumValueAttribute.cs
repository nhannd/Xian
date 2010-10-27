#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Allows meta-data to be specified for each member of a C# enum.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false, Inherited=false)]
    public class EnumValueAttribute : Attribute
    {
        private string _value;
        private string _description;

        public EnumValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
