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
using ClearCanvas.Common.Utilities;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Provides methods for translating domain object class and property names into user-friendly
    /// equivalents.
    /// </summary>
    public static class TerminologyTranslator
    {
        /// <summary>
        /// Translates the name of the specified property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string Translate(PropertyInfo property)
        {
            return Translate(property.ReflectedType, property.Name);
        }

        /// <summary>
        /// Translates the name of the specified property on the specified domain class.
        /// </summary>
        /// <param name="domainClass"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string Translate(Type domainClass, string propertyName)
        {
            IResourceResolver resolver = new ResourceResolver(domainClass.Assembly);

            string key = domainClass.Name + propertyName;
            string localized = resolver.LocalizeString(key);
            if (localized == key)
                localized = resolver.LocalizeString(propertyName);

            return localized;
        }

        /// <summary>
        /// Translates the name of the specified domain class.
        /// </summary>
        /// <param name="domainClass"></param>
        /// <returns></returns>
        public static string Translate(Type domainClass)
        {
            IResourceResolver resolver = new ResourceResolver(domainClass.Assembly);
            return resolver.LocalizeString(domainClass.Name);
        }
    }
}
