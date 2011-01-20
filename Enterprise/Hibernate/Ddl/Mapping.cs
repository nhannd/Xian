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
