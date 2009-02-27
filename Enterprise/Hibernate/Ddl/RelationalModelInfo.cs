using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Common.Utilities;
using Iesi.Collections;
using NHibernate.Cfg;
using NHibernate.Mapping;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a relational database model.
	/// </summary>
    [DataContract]
    public class RelationalModelInfo : ElementInfo
    {
    	private List<TableInfo> _tables;
    	private List<EnumerationInfo> _enumerations;

		/// <summary>
		/// Constructor that creates an empty model.
		/// </summary>
        public RelationalModelInfo()
        {
			_tables = new List<TableInfo>();
			_enumerations = new List<EnumerationInfo>();
        }

		/// <summary>
		/// Constructor that creates a model from all NHibernate mappings and embedded enumeration information
		/// in the set of installed plugins.
		/// </summary>
		/// <param name="config"></param>
		public RelationalModelInfo(Configuration config)
		{
			_tables = CollectionUtils.Map<Table, TableInfo>(GetTables(config),
							delegate(Table table) { return BuildTableInfo(table, config); });

			_enumerations = new EnumMetadataReader().GetEnums(config);
		}

		/// <summary>
		/// Gets the set of tables.
		/// </summary>
    	[DataMember]
    	public List<TableInfo> Tables
    	{
			get { return _tables; }

			// for de-serialization
			private set { _tables = value; }
    	}

		/// <summary>
		/// Gets the set of enumerations.
		/// </summary>
		[DataMember]
		public List<EnumerationInfo> Enumerations
    	{
			get { return _enumerations; }
			private set { _enumerations = value; }
    	}

		/// <summary>
		/// Gets the table that matches the specified (unqualified) name, or null if no match.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public TableInfo GetTable(string table)
		{
			return CollectionUtils.SelectFirst(_tables, delegate(TableInfo t) { return t.Name == table; });
		}

		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
		public override string Identity
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

		#region Helpers

		/// <summary>
		/// Gets the set of NHibernate <see cref="Table"/> objects known to the specified configuration.
		/// </summary>
		/// <param name="cfg"></param>
		/// <returns></returns>
		private static List<Table> GetTables(Configuration cfg)
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

		/// <summary>
		/// Converts an NHibernate <see cref="Table"/> object to a <see cref="TableInfo"/> object.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		private static TableInfo BuildTableInfo(Table table, Configuration config)
		{
			Dialect dialect = Dialect.GetDialect(config.Properties);
			return new TableInfo(
				table.Name,
				table.Schema,
				CollectionUtils.Map<Column, ColumnInfo>(table.ColumnCollection, delegate(Column column) { return new ColumnInfo(column, config, dialect); }),
				new ConstraintInfo(table.PrimaryKey),
				CollectionUtils.Map<Index, IndexInfo>(table.IndexCollection, delegate(Index index) { return new IndexInfo(index); }),
				CollectionUtils.Map<ForeignKey, ForeignKeyInfo>(table.ForeignKeyCollection, delegate(ForeignKey fk) { return new ForeignKeyInfo(fk, config); }),
				CollectionUtils.Map<UniqueKey, ConstraintInfo>(table.UniqueKeyCollection, delegate(UniqueKey uk) { return new ConstraintInfo(uk); })
				);
		}

		#endregion
	}
}
