using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	internal class Mapping : IMapping
	{
		private readonly Configuration _config;

		public Mapping(Configuration config)
		{
			this._config = config;
		}

		private PersistentClass GetPersistentClass(System.Type type)
		{
			PersistentClass pc = _config.GetClassMapping(type);
			if (pc == null)
			{
				throw new Exception("persistent class not known: " + type.FullName);
			}
			return pc;
		}

		public IType GetIdentifierType(System.Type persistentClass)
		{
			return GetPersistentClass(persistentClass).Identifier.Type;
		}

		public string GetIdentifierPropertyName(System.Type persistentClass)
		{
			PersistentClass pc = GetPersistentClass(persistentClass);
			if (!pc.HasIdentifierProperty)
			{
				return null;
			}
			return pc.IdentifierProperty.Name;
		}

		public IType GetPropertyType(System.Type persistentClass, string propertyName)
		{
			PersistentClass pc = GetPersistentClass(persistentClass);
			NHibernate.Mapping.Property prop = pc.GetProperty(propertyName);

			if (prop == null)
			{
				throw new Exception("property not known: " + persistentClass.FullName + '.' + propertyName);
			}
			return prop.Type;
		}
	}
}
