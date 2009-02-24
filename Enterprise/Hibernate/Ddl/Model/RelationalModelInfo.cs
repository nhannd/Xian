using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Common.Utilities;
using Iesi.Collections;
using NHibernate.Cfg;
using NHibernate.Mapping;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Model
{
    [DataContract]
    public class RelationalModelInfo : ElementInfo
    {
        public RelationalModelInfo()
        {

        }

		public RelationalModelInfo(Configuration config, Dialect dialect)
		{
			this.Tables = CollectionUtils.Map<Table, TableInfo>(GetTables(config),
							delegate(Table table) { return BuildTableInfo(table, config, dialect); });
		}

        [DataMember]
        public List<TableInfo> Tables;

        public override string Identity
        {
            get { throw new Exception("The method or operation is not implemented."); }
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

		private TableInfo BuildTableInfo(Table table, Configuration config, Dialect dialect)
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
