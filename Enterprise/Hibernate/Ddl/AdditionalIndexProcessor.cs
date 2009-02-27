using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using Iesi.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Mapping;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Adds additional indexes to the Hibernate relational model, according to what is defined in *.dbi.xml files
    /// that are found in plugins.
    /// </summary>
    /// <remarks>
    /// This processor scans all plugins for *.dbi.xml resource files.  These files contain instructions for creating
    /// specific indexes in an XML format. See the file AdditionalIndexProcessor.dbi.xml.
    /// </remarks>
    class AdditionalIndexProcessor : IndexCreatorBase
    {
        public override void Process(Configuration config)
        {
            Dictionary<string, Table> tables = GetTables(config);

            // create a resource resolver that will scan all plugins
			// TODO: we should only scan plugins that are tied to the specified PersistentStore, but there is currently no way to know this
            IResourceResolver resolver = new ResourceResolver(
                CollectionUtils.Map<PluginInfo, Assembly>(Platform.PluginManager.Plugins,
                    delegate(PluginInfo pi) { return pi.Assembly; }).ToArray());

            // find all dbi resources
            Regex rx = new Regex("dbi.xml$", RegexOptions.Compiled|RegexOptions.IgnoreCase);
            string[] dbiFiles = resolver.FindResources(rx);

            foreach (string dbiFile in dbiFiles)
            {
                using(Stream stream = resolver.OpenResource(dbiFile))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(stream);
                    foreach(XmlElement indexElement in xmlDoc.SelectNodes("indexes/index"))
                    {
                        ProcessIndex(indexElement, tables);
                    }
                }
            }

        }

        private void ProcessIndex(XmlElement indexElement, Dictionary<string, Table> tables)
        {
            string tableName = indexElement.GetAttribute("table");
            List<string> columnNames = CollectionUtils.Map<string, string>(indexElement.GetAttribute("columns").Split(),
                                                                           delegate(string s) { return s.Trim(); });

            if(!string.IsNullOrEmpty(tableName) && columnNames.Count > 0)
            {
                Table table;
                if(!tables.TryGetValue(tableName, out table))
                    return;
                List<Column> columns = CollectionUtils.Map<string, Column>(columnNames,
                                           delegate (string name)
                                           {
                                               return CollectionUtils.SelectFirst<Column>(
                                                   table.ColumnCollection,
                                                   delegate(Column c) { return c.Name == name; });
                                           });

                CreateIndex(table, columns);
            }
        }

		private Dictionary<string, Table> GetTables(Configuration config)
        {
            // build a set of all tables known to NH
            Dictionary<string, Table> tableSet = new Dictionary<string, Table>();
            foreach (PersistentClass c in config.ClassMappings)
            {
                tableSet[c.Table.Name] = c.Table;
            }

            foreach (Collection mapping in config.CollectionMappings)
            {
                tableSet[mapping.CollectionTable.Name] = mapping.CollectionTable;
                tableSet[mapping.Table.Name] = mapping.Table;
            }
            return tableSet;
        }
    }
}
