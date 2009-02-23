using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using NHibernate.Mapping;
using NHibernate.Cfg;
using Iesi.Collections;
using ClearCanvas.Common.Utilities;
using System.IO;
using NHibernate.Dialect;
using ClearCanvas.Enterprise.Hibernate.Ddl.Model;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	public class XmlWriter
	{






		/// <summary>
		/// Defines the root XML tag under which data is exported.
		/// </summary>
		private readonly Configuration _config;
		private readonly Dialect _dialect;

		public XmlWriter(Configuration config, Dialect dialect)
		{
			_config = config;
			_dialect = dialect;
		}

		public void WriteModel(TextWriter tw)
		{
			using (XmlTextWriter writer = new XmlTextWriter(tw))
			{
				writer.Formatting = System.Xml.Formatting.Indented;

				DatabaseSchemaInfo schemaInfo = new DatabaseSchemaInfo(
					CollectionUtils.Map<Table, TableInfo>(GetTables(_config),
														  delegate(Table table) { return BuildTableInfo(table); })
					);

				Write(writer, schemaInfo);
			}
 		}

		public DatabaseSchemaInfo ReadModel(TextReader tr)
		{
			using (XmlTextReader reader = new XmlTextReader(tr))
			{
				reader.WhitespaceHandling = WhitespaceHandling.None;
				return (DatabaseSchemaInfo) Read(reader, typeof(DatabaseSchemaInfo));
			}
		}

		private List<Table> GetTables(Configuration cfg)
		{
			// build set of all tables
			HybridSet tables = new HybridSet();
			foreach (PersistentClass pc in cfg.ClassMappings)
			{
				tables.AddAll(pc.TableClosureCollection);
			}

			foreach (Collection collection in cfg.CollectionMappings)
			{
				tables.Add(collection.CollectionTable);
			}

			return CollectionUtils.Sort<Table>(tables,
				delegate(Table x, Table y)
				{
					return x.Name.CompareTo(y.Name);
				});
		}

		private TableInfo BuildTableInfo(Table table)
		{
			return new TableInfo(
				table.Name,
				table.Schema,
				CollectionUtils.Map<Column, ColumnInfo>(table.ColumnCollection, delegate (Column column) { return new ColumnInfo(column, table, _config, _dialect);}),
                new ConstraintInfo(table.PrimaryKey),
				CollectionUtils.Map<Index, IndexInfo>(table.IndexCollection, delegate(Index index) { return new IndexInfo(index); }),
				CollectionUtils.Map<ForeignKey, ForeignKeyInfo>(table.ForeignKeyCollection, delegate(ForeignKey fk) { return new ForeignKeyInfo(fk, _config); }),
				CollectionUtils.Map<UniqueKey, ConstraintInfo>(table.UniqueKeyCollection, delegate(UniqueKey uk) { return new ConstraintInfo(uk); })
				);
		}

		/// <summary>
		/// Writes the specified data to the specified xml writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="data"></param>
		private static void Write(System.Xml.XmlWriter writer, object data)
		{
			JsmlSerializer.Serialize(writer, data, data.GetType().Name, false);
		}

		/// <summary>
		/// Reads an object of the specified class from the xml reader.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="dataContractClass"></param>
		/// <returns></returns>
		private static object Read(XmlReader reader, Type dataContractClass)
		{
			return JsmlSerializer.Deserialize(reader, dataContractClass);
		}

		/// <summary>
		/// Gets the assembly qualified name of the type, but without all the version and culture info.
		/// </summary>
		/// <param name="entityClass"></param>
		/// <returns></returns>
		private static string GetSafeClassName(Type entityClass)
		{
			return string.Format("{0}, {1}", entityClass.FullName, entityClass.Assembly.GetName().Name);
		}
	}
}
