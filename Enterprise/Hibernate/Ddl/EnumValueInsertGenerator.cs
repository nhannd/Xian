using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Enterprise;
using NHibernate.Metadata;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using NHibernate.Mapping;
using ClearCanvas.Common;
using System.IO;
using System.Xml;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Generates scripts to insert enumeration values into tables  
    /// </summary>
    class EnumValueInsertGenerator : DdlScriptGenerator
    {
        class Insert
        {
            public string Table;
            public string Code;
            public string Value;
            public string Description;

            public string GetCreateScript()
            {
                return string.Format("insert into {0} (Code_, Value_, Description_) values ({1}, {2}, {3})",
                    Table,
                    SqlFormat(Code),
                    SqlFormat(Value),
                    SqlFormat(Description));
            }

            private string SqlFormat(string str)
            {
                if (str == null)
                    return "NULL";

                // make sure to escape ' to ''
                return string.Format("'{0}'", str.Replace("'", "''"));
            }
        }

        public override string[] GenerateCreateScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
        {
            // build a map between enum classes and C# enums
            Dictionary<Type, Type> mapClassToEnum = new Dictionary<Type,Type>();
            foreach(PluginInfo plugin in Platform.PluginManager.Plugins)
            {
                foreach(Type type in plugin.Assembly.GetTypes())
                {
                    if(type.IsEnum)
                    {
                        EnumValueClassAttribute attr = CollectionUtils.FirstElement<EnumValueClassAttribute>(
                            type.GetCustomAttributes(typeof(EnumValueClassAttribute), false));
                        if(attr != null)
                            mapClassToEnum.Add(attr.EnumValueClass, type);
                    }
                }
            }

            // mapped enum classes
            ICollection<PersistentClass> persistentEnumClasses = CollectionUtils.Select<PersistentClass>(
                store.Configuration.ClassMappings,
                delegate(PersistentClass c) { return typeof(EnumValue).IsAssignableFrom(c.MappedClass); });

            List<Insert> inserts = new List<Insert>();
            foreach (PersistentClass pclass in persistentEnumClasses)
            {
                if (mapClassToEnum.ContainsKey(pclass.MappedClass))
                    ProcessHardEnum(mapClassToEnum[pclass.MappedClass], pclass.Table.Name, inserts);
                else
                    ProcessSoftEnum(pclass.MappedClass, pclass.Table.Name, inserts);
            }

            return CollectionUtils.Map<Insert, string, List<string>>(inserts,
                delegate(Insert i) { return i.GetCreateScript(); }).ToArray();
        }

        public override string[] GenerateDropScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
        {
            return new string[] { };    // nothing to do
        }

        private void ProcessSoftEnum(Type enumValueClass, string tableName, List<Insert> inserts)
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
                    foreach (XmlElement enumValueElement in xmlDoc.GetElementsByTagName("enum-value"))
                    {
                        enumValueElement.GetAttribute("code");

                        Insert insert = new Insert();
                        insert.Table = tableName;
                        insert.Code = enumValueElement.GetAttribute("code");
                        XmlElement valueNode = CollectionUtils.FirstElement<XmlElement>(enumValueElement.GetElementsByTagName("value"));
                        if (valueNode != null)
                            insert.Value = valueNode.InnerText;
                        XmlElement descNode = CollectionUtils.FirstElement<XmlElement>(enumValueElement.GetElementsByTagName("description"));
                        if (descNode != null)
                            insert.Description = valueNode.InnerText;

                        inserts.Add(insert);
                    }
                }
            }
            catch (Exception)
            {
                // no embedded resource found - nothing to insert
            }
        }

        private void ProcessHardEnum(Type enumType, string tableName, List<Insert> inserts)
        {
            foreach (FieldInfo fi in enumType.GetFields())
            {
                // try to get an attribute
                object[] attrs = fi.GetCustomAttributes(typeof(EnumValueAttribute), false);
                if (attrs.Length > 0)
                {
                    Insert insert = new Insert();
                    insert.Table = tableName;
                    insert.Code = fi.Name;
                    insert.Value = ((EnumValueAttribute)attrs[0]).Value;
                    insert.Description = ((EnumValueAttribute)attrs[0]).Description;

                    inserts.Add(insert);
                }
            }
        }
    }
}
