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
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// For internal use.  Copied from NHibernate source.
    /// </summary>
    internal class Mapping : IMapping
    {
        private readonly Configuration configuration;

        public Mapping(Configuration configuration)
        {
            this.configuration = configuration;
        }

        private PersistentClass GetPersistentClass(string className)
        {
            PersistentClass pc = configuration.GetClassMapping(className);
            if (pc == null)
            {
                throw new MappingException("persistent class not known: " + className);
            }
            return pc;
        }

        public IType GetIdentifierType(string className)
        {
            return GetPersistentClass(className).Identifier.Type;
        }

        public string GetIdentifierPropertyName(string className)
        {
            PersistentClass pc = GetPersistentClass(className);
            if (!pc.HasIdentifierProperty)
            {
                return null;
            }
            return pc.IdentifierProperty.Name;
        }

        public IType GetReferencedPropertyType(string className, string propertyName)
        {
            PersistentClass pc = GetPersistentClass(className);
            Property prop = pc.GetProperty(propertyName);

            if (prop == null)
            {
                throw new MappingException("property not known: " + pc.MappedClass.FullName + '.' + propertyName);
            }
            return prop.Type;
        }

        public bool HasNonIdentifierPropertyNamedId(string className)
        {
            return "id".Equals(GetIdentifierPropertyName(className));
        }
    }
}
