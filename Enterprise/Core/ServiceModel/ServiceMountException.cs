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

namespace ClearCanvas.Enterprise.Core.ServiceModel
{
    public class ServiceMountException : Exception
    {
        public ServiceMountException(string message, Exception inner)
            :base(message, inner)
        {
        }

        public ServiceMountException(string message)
            :this(message, null)
        {
        }
    }
}
