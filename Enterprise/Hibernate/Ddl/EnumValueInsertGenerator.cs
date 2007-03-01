using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Enterprise;
using NHibernate.Metadata;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Generates scripts to insert enumeration values into tables  
    /// </summary>
    class EnumValueInsertGenerator : IDdlScriptGenerator
    {
        #region IDdlScriptGenerator Members

        public string[] GenerateCreateScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
        {
            List<string> scripts = new List<string>();
            foreach (IClassMetadata metaData in store.Metadata.Values)
            {
                string className = metaData.MappedClass.Name;
                if (className.EndsWith("Enum"))
                {
                    Type baseClass = metaData.MappedClass.BaseType;
                    Type enumType = baseClass.GetGenericArguments()[0];
                    foreach (FieldInfo fi in enumType.GetFields())
                    {
                        // ignore compiler generated fields - only care about the actual enum values
                        if (!fi.IsSpecialName && fi.FieldType == enumType)
                        {
                            // according to our convention, this is the table name
                            // (unfortunately seems there is no way to obtain the table name from the meta-data)
                            string tableName = string.Format("{0}_", className);

                            // the code is just the value of the field name
                            string code = SqlFormat(fi.Name);

                            // try to get an attribute
                            EnumValueAttribute attr = GetAttribute(fi);

                            // extract the Value from the attribute, if it exists, otherwise just use the field name
                            string value = SqlFormat((attr != null) ? attr.Value : fi.Name);

                            // extract the Description from the attribute, if it exists, otherwise set this to NULL
                            string description = SqlFormat(attr != null ? attr.Description : null);
                            string script = string.Format("insert into {0} (Code_, Value_, Description_) values ({1}, {2}, {3})",
                                tableName,
                                code,
                                value,
                                description);
                            scripts.Add(script);
                        }
                    }
                }
            }
            return scripts.ToArray();
        }

        public string[] GenerateDropScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
        {
            return new string[] { };    // nothing to do
        }

        #endregion

        private EnumValueAttribute GetAttribute(FieldInfo fi)
        {
            object[] attrs = fi.GetCustomAttributes(typeof(EnumValueAttribute), false);
            return attrs.Length > 0 ? (EnumValueAttribute)attrs[0] : null;
        }

        private string SqlFormat(string str)
        {
            if (str == null)
                return "NULL";

            // make sure to escape ' to ''
            return string.Format("'{0}'", str.Replace("'", "''"));
        }

    }
}
