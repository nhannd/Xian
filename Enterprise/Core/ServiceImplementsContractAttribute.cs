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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class ServiceImplementsContractAttribute : Attribute
    {
        private Type _serviceContract;

        public ServiceImplementsContractAttribute(Type serviceContract)
        {
            _serviceContract = serviceContract;
        }

        public Type ServiceContract
        {
            get { return _serviceContract; }
        }
    }
}
