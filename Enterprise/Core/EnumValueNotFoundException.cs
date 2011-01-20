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

namespace ClearCanvas.Enterprise.Core
{
    public class EnumValueNotFoundException : Exception
    {
        public EnumValueNotFoundException(Type enumClass, string enumCode, Exception inner)
            : base(string.Format(SR.ExceptionEnumValueNotFound, enumCode, enumClass.FullName), inner)
        {
        }
   }
}
