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
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class EnumValueClassAttribute : Attribute
    {
        private Type _enumValueClass;

        public EnumValueClassAttribute(Type enumValueClass)
        {
            _enumValueClass = enumValueClass;
        }

        public Type EnumValueClass
        {
            get { return _enumValueClass; }
        }
    }
}
