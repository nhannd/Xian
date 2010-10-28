#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;

namespace ClearCanvas.ImageServer.Enterprise
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumValueDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _longDescription;
        
        public EnumValueDescriptionAttribute(string description, string longDescription)
            :base(description)
        {
            _longDescription = longDescription;
        }

        public string LongDescription
        {
            get
            {
                return _longDescription;
            }
        }
    }

}
