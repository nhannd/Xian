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
    [DataContract]
    public class RelationalModelInfo : ElementInfo
    {
    	private List<TableInfo> _tables;
    	private List<EnumerationInfo> _enumerations;

        public RelationalModelInfo()
        {
			_tables = new List<TableInfo>();
			_enumerations = new List<EnumerationInfo>();
        }

		public RelationalModelInfo(Configuration config, Dialect dialect)
		{
			_tables = CollectionUtils.Map<Table, TableInfo>(GetTables(config),
							delegate(Table table) { return BuildTableInfo(table, config, dialect); });

			_enumerations = new EnumMetadataReader().GetEnums(config);
		}

    	[DataMember]
    	public List<TableInfo> Tables
    	{
			get { return _tables; }

			// for de-serialization
			private set { _tables = value; }
    	}

		[DataMember]
		public List<EnumerationInfo> Enumerations
    	{
			get { return _enumerations; }
			private set { _enumerations = value; }
    	}

		public TableInfo GetTable(string table)
		{
			return CollectionUtils.SelectFirst(_tables, delegate(TableInfo t) { return t.Name == table; });
		}

        public override string Identity
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

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

		private static TableInfo BuildTableInfo(Table table, Configuration config, Dialect dialect)
		{
			return new TableInfo(
				table.Name,
				table.Schema,
				CollectionUtils.Map<Column, ColumnInfo>(table.ColumnCollection, delegate(Column column) { return new ColumnInfo(column, table, config, dialect); }),
				new ConstraintInfo(table.PrimaryKey),
				CollectionUtils.Map<Index, IndexInfo>(table.IndexCollection, delegate(Index index) { return new IndexInfo(index); }),
				CollectionUtils.Map<ForeignKey, ForeignKeyInfo>(table.ForeignKeyCollection, delegate(ForeignKey fk) { return new ForeignKeyInfo(fk, config); }),
				CollectionUtils.Map<UniqueKey, ConstraintInfo>(table.UniqueKeyCollection, delegate(UniqueKey uk) { return new ConstraintInfo(uk); })
				);
		}
	}
}
