#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.IO;
using System.Reflection;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Generates scripts to insert enumeration values into tables  
	/// </summary>
	abstract class EnumValueInsertGenerator : DdlScriptGenerator
	{
		protected class Insert
		{
			public Table Table;
			public string Code;
			public string Value;
			public string Description;
			public float DisplayOrder;

			public string GetCreateScript(Dialect dialect, string defaultSchema)
			{
				return string.Format("insert into {0} (Code_, Value_, Description_, DisplayOrder_, Deactivated_) values ({1}, {2}, {3}, {4}, {5})",
					Table.GetQualifiedName(dialect, defaultSchema),
					SqlFormat(Code),
					SqlFormat(Value),
					SqlFormat(Description),
					DisplayOrder,
					SqlFormat(false.ToString()));
			}

			private string SqlFormat(string str)
			{
				if (str == null)
					return "NULL";

				// make sure to escape ' to ''
				return string.Format("'{0}'", str.Replace("'", "''"));
			}
		}

		protected Dictionary<Type, Type> _mapClassToEnum;

		protected EnumValueInsertGenerator()
		{
			_mapClassToEnum = new Dictionary<Type, Type>();
			foreach (PluginInfo plugin in Platform.PluginManager.Plugins)
			{
				foreach (Type type in plugin.Assembly.GetTypes())
				{
					if (type.IsEnum)
					{
						EnumValueClassAttribute attr = CollectionUtils.FirstElement<EnumValueClassAttribute>(
							type.GetCustomAttributes(typeof(EnumValueClassAttribute), false));
						if (attr != null)
							_mapClassToEnum.Add(attr.EnumValueClass, type);
					}
				}
			}
		}

		protected abstract bool CanProcessEnum(Type enumValueClass);
		protected abstract void ProcessEnum(Type enumValueClass, Table table, List<Insert> inserts);

		public override string[] GenerateCreateScripts(Configuration config, Dialect dialect)
		{
			// mapped enum classes
			ICollection<PersistentClass> persistentEnumClasses = CollectionUtils.Select<PersistentClass>(
				config.ClassMappings,
				delegate(PersistentClass c) { return typeof(EnumValue).IsAssignableFrom(c.MappedClass); });

			List<Insert> inserts = new List<Insert>();
			foreach (PersistentClass pclass in persistentEnumClasses)
			{
				if (this.CanProcessEnum(pclass.MappedClass))
					ProcessEnum(pclass.MappedClass, pclass.Table, inserts);
			}

			string defaultSchema = config.GetProperty(NHibernate.Cfg.Environment.DefaultSchema);

			List<string> scripts = CollectionUtils.Map<Insert, string>(inserts,
				delegate(Insert i) { return i.GetCreateScript(dialect, defaultSchema); });
			scripts.Sort();
			return scripts.ToArray();
		}

		public override string[] GenerateDropScripts(Configuration config, Dialect dialect)
		{
			return new string[] { };    // nothing to do
		}
	}

	class HardEnumValueInsertGenerator : EnumValueInsertGenerator
	{
		protected override bool CanProcessEnum(Type enumValueClass)
		{
			return _mapClassToEnum.ContainsKey(enumValueClass);
		}

		protected override void ProcessEnum(Type enumValueClass, Table table, List<Insert> inserts)
		{
			Type enumType = _mapClassToEnum[enumValueClass];

			int displayOrder = 1;

			// note that we process the enum constants in order of the underlying value assigned
			// so that the initial displayOrder reflects the natural ordering
			// (see msdn docs for Enum.GetValues for details)
			foreach (object value in Enum.GetValues(enumType))
			{
				string code = Enum.GetName(enumType, value);
				FieldInfo fi = enumType.GetField(code);
				EnumValueAttribute attr = AttributeUtils.GetAttribute<EnumValueAttribute>(fi);
				if (attr != null)
				{
					Insert insert = new Insert();
					insert.Table = table;
					insert.Code = code;
					insert.Value = attr.Value;
					insert.Description = attr.Description;

					// add 1 because we want all display order values to initially be greater than 1
					insert.DisplayOrder = displayOrder++;

					inserts.Add(insert);
				}
			}
		}
	}

	class SoftEnumValueInsertGenerator : EnumValueInsertGenerator
	{
		protected override bool CanProcessEnum(Type enumValueClass)
		{
			return !_mapClassToEnum.ContainsKey(enumValueClass);
		}

		protected override void ProcessEnum(Type enumValueClass, Table table, List<Insert> inserts)
		{
			// look for an embedded resource that matches the enum class
			string res = string.Format("{0}.enum.xml", enumValueClass.FullName);
			IResourceResolver resolver = new ResourceResolver(enumValueClass.Assembly);
			try
			{
				using (Stream xmlStream = resolver.OpenResource(res))
				{
					XmlDocument xmlDoc = new XmlDocument();
					xmlDoc.Load(xmlStream);
					int displayOrder = 1;
					foreach (XmlElement enumValueElement in xmlDoc.GetElementsByTagName("enum-value"))
					{
						enumValueElement.GetAttribute("code");

						Insert insert = new Insert();
						insert.Table = table;
						insert.Code = enumValueElement.GetAttribute("code");
						XmlElement valueNode = CollectionUtils.FirstElement<XmlElement>(enumValueElement.GetElementsByTagName("value"));
						if (valueNode != null)
							insert.Value = valueNode.InnerText;
						XmlElement descNode = CollectionUtils.FirstElement<XmlElement>(enumValueElement.GetElementsByTagName("description"));
						if (descNode != null)
							insert.Description = descNode.InnerText;

						insert.DisplayOrder = displayOrder++;

						inserts.Add(insert);
					}
				}
			}
			catch (Exception)
			{
				// no embedded resource found - nothing to insert
			}
		}
	}
}
