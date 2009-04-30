#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Base class for rules that represent simple invariant constraints on a property of an object.
    /// </summary>
    public abstract class SimpleInvariantSpecification : ISpecification, IPropertyBoundRule
    {
        private PropertyInfo[] _properties;

        public SimpleInvariantSpecification(PropertyInfo[] properties)
        {
            _properties = properties;
        }

        public SimpleInvariantSpecification(PropertyInfo property)
        {
            _properties = new PropertyInfo[] { property };
        }


        #region ISpecification Members

        public abstract TestResult Test(object obj);

        #endregion


        public PropertyInfo[] Properties
        {
            get { return _properties; }
        }

        public PropertyInfo Property
        {
            get { return _properties[0]; }
        }

        protected object GetPropertyValue(object obj)
        {
            
            return GetPropertyValue(obj, _properties[0]);
        }

        protected object[] GetPropertyValues(object obj)
        {
            return CollectionUtils.Map<PropertyInfo, object>(_properties,
                delegate(PropertyInfo property) { return GetPropertyValue(obj, property); }).ToArray();
        }

        private object GetPropertyValue(object obj, PropertyInfo property)
        {
            return property.GetGetMethod().Invoke(obj, null);
        }
    }
}
