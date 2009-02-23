using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Ddl.Model;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	class SchemaComparer
	{
		delegate IEnumerable<Change> ItemProcessor<T>(T item);
		delegate IEnumerable<Change> CompareItemProcessor<T>(T initial, T desired);

		private static readonly Dictionary<Type, int> _changeOrder = new Dictionary<Type, int>();

		static SchemaComparer()
		{
			// define the order that changes should occur to avoid dependency issues
			_changeOrder.Add(typeof(DropForeignKeyChange), 0);
			_changeOrder.Add(typeof(DropUniqueConstraintChange), 0);
			_changeOrder.Add(typeof(DropIndexChange), 1);
			_changeOrder.Add(typeof(DropTableChange), 2);
			_changeOrder.Add(typeof(DropColumnChange), 3);
			_changeOrder.Add(typeof(ColumnPropertiesChange), 4);
			_changeOrder.Add(typeof(AddColumnChange), 4);
			_changeOrder.Add(typeof(AddTableChange), 5);
			_changeOrder.Add(typeof(AddIndexChange), 6);
			_changeOrder.Add(typeof(AddUniqueConstraintChange), 7);
			_changeOrder.Add(typeof(AddForeignKeyChange), 7);
		}


		public List<Change> CompareDatabases(DatabaseSchemaInfo initial, DatabaseSchemaInfo desired)
		{
			IEnumerable<Change> changes = CompareSets(initial.Tables, desired.Tables,
			                                          AddTable,
			                                          DropTable,
			                                          CompareTables);

			// order changes correctly
			return CollectionUtils.Sort(changes,
						delegate(Change x, Change y)
						{
							return _changeOrder[x.GetType()].CompareTo(_changeOrder[y.GetType()]);
						});
		}

		private IEnumerable<Change> AddTable(TableInfo t)
		{
			List<Change> changes = new List<Change>();
			changes.Add(new AddTableChange(t));
			changes.AddRange(
				CollectionUtils.Map<IndexInfo, Change>(t.Indexes,
				    delegate(IndexInfo item) { return new AddIndexChange(t, item); }));
			changes.AddRange(
				CollectionUtils.Map<ConstraintInfo, Change>(t.UniqueKeys,
				    delegate(ConstraintInfo item) { return new AddUniqueConstraintChange(t, item); }));
			changes.AddRange(
				CollectionUtils.Map<ForeignKeyInfo, Change>(t.ForeignKeys,
				    delegate(ForeignKeyInfo item)
				    {
				    	return new AddForeignKeyChange(t, item);
				    }));
			return changes;
		}

		private IEnumerable<Change> DropTable(TableInfo t)
		{
			List<Change> changes = new List<Change>();
			changes.AddRange(
				CollectionUtils.Map<IndexInfo, Change>(t.Indexes,
					delegate(IndexInfo item) { return new DropIndexChange(t, item); }));
			changes.AddRange(
				CollectionUtils.Map<ConstraintInfo, Change>(t.UniqueKeys,
					delegate(ConstraintInfo item) { return new DropUniqueConstraintChange(t, item); }));
			changes.AddRange(
				CollectionUtils.Map<ForeignKeyInfo, Change>(t.ForeignKeys,
					delegate(ForeignKeyInfo item) { return new DropForeignKeyChange(t, item); }));
			changes.Add(new DropTableChange(t));
			return changes;
		}

		private IEnumerable<Change> CompareTables(TableInfo initial, TableInfo desired)
		{
			TableInfo table = desired;

			List<Change> changes = new List<Change>();
			changes.AddRange(
				CompareSets(initial.Columns, desired.Columns,
				            delegate(ColumnInfo item) { return AddColumn(table, item); },
				            delegate(ColumnInfo item) { return DropColumn(table, item); },
				            delegate(ColumnInfo x, ColumnInfo y) { return CompareColumns(table, x, y); }));

			changes.AddRange(
				CompareSets(initial.Indexes, desired.Indexes,
				            delegate(IndexInfo item) { return AddIndex(table, item); },
				            delegate(IndexInfo item) { return DropIndex(table, item); },
				            delegate(IndexInfo x, IndexInfo y) { return CompareIndexes(table, x, y); }));

			changes.AddRange(
				CompareSets(initial.ForeignKeys, desired.ForeignKeys,
				            delegate(ForeignKeyInfo item) { return AddForeignKey(table, item); },
				            delegate(ForeignKeyInfo item) { return DropForeignKey(table, item); },
				            delegate(ForeignKeyInfo x, ForeignKeyInfo y) { return CompareForeignKeys(table, x, y); }));

			changes.AddRange(
				CompareSets(initial.UniqueKeys, desired.UniqueKeys,
				            delegate(ConstraintInfo item) { return AddUniqueConstraint(table, item); },
				            delegate(ConstraintInfo item) { return DropUniqueConstraint(table, item); },
				            delegate(ConstraintInfo x, ConstraintInfo y) { return CompareUniqueConstraints(table, x, y); }));

			changes.AddRange(
				CompareSets(new ConstraintInfo[] { initial.PrimaryKey }, new ConstraintInfo[] { desired.PrimaryKey },
				            delegate(ConstraintInfo item) { return AddPrimaryKey(table, item); },
				            delegate(ConstraintInfo item) { return DropPrimaryKey(table, item); },
				            delegate(ConstraintInfo x, ConstraintInfo y) { return ComparePrimaryKeys(table, x, y); }));

			return changes;
		}


		private IEnumerable<Change> AddColumn(TableInfo table, ColumnInfo c)
		{
			return new Change[] { new AddColumnChange(table, c) };
		}

		private IEnumerable<Change> DropColumn(TableInfo table, ColumnInfo c)
		{
			return new Change[] { new DropColumnChange(table, c) };
		}

		private IEnumerable<Change> CompareColumns(TableInfo table, ColumnInfo initial, ColumnInfo desired)
		{
			List<Change> changes = new List<Change>();
			if (!initial.IsIdentical(desired))
				changes.Add(new ColumnPropertiesChange(table, initial, desired));
			return changes;
		}

		private IEnumerable<Change> AddIndex(TableInfo table, IndexInfo c)
		{
			return new Change[] { new AddIndexChange(table, c) };
		}

		private IEnumerable<Change> DropIndex(TableInfo table, IndexInfo c)
		{
			return new Change[] { new DropIndexChange(table, c) };
		}

		private IEnumerable<Change> CompareIndexes(TableInfo table, IndexInfo initial, IndexInfo desired)
		{
			List<Change> changes = new List<Change>();
			// TODO
			return changes;
		}

		private IEnumerable<Change> AddForeignKey(TableInfo table, ForeignKeyInfo c)
		{
			return new Change[] { new AddForeignKeyChange(table, c) };
		}

		private IEnumerable<Change> DropForeignKey(TableInfo table, ForeignKeyInfo c)
		{
			return new Change[] { new DropForeignKeyChange(table, c) };
		}

		private IEnumerable<Change> CompareForeignKeys(TableInfo table, ForeignKeyInfo initial, ForeignKeyInfo desired)
		{
			List<Change> changes = new List<Change>();
			// todo
			return changes;
		}

		private IEnumerable<Change> AddUniqueConstraint(TableInfo table, ConstraintInfo c)
		{
			return new Change[] { new AddUniqueConstraintChange(table, c) };
		}

		private IEnumerable<Change> DropUniqueConstraint(TableInfo table, ConstraintInfo c)
		{
			return new Change[] { new DropUniqueConstraintChange(table, c) };
		}

		private IEnumerable<Change> CompareUniqueConstraints(TableInfo table, ConstraintInfo initial, ConstraintInfo desired)
		{
			List<Change> changes = new List<Change>();
			//todo
			return changes;
		}

		private IEnumerable<Change> AddPrimaryKey(TableInfo table, ConstraintInfo item)
		{
			//todo
			throw new Exception("The method or operation is not implemented.");
		}

		private IEnumerable<Change> DropPrimaryKey(TableInfo table, ConstraintInfo item)
		{
			//todo
			throw new Exception("The method or operation is not implemented.");
		}

		private IEnumerable<Change> ComparePrimaryKeys(TableInfo table, ConstraintInfo initial, ConstraintInfo desired)
		{
			List<Change> changes = new List<Change>();
			//todo
			return changes;
		}

		private IEnumerable<Change> CompareSets<T>(IEnumerable<T> initial, IEnumerable<T> desired,
		                                           ItemProcessor<T> addProcessor,
		                                           ItemProcessor<T> dropProcessor,
												   CompareItemProcessor<T> compareProcessor)
			where T : ElementInfo
		{
			List<Change> changes = new List<Change>();

			// partition desired set into those items that are contained in the initial set (true) and
			// those that are not (false)
			IDictionary<bool, List<T>> a = GroupBy<T, bool>(desired,
											delegate(T x) { return CollectionUtils.Contains(initial, delegate(T y) { return x.IsSame(y); }); });

			// partition initial set into those items that are contained in the desired set (true) and
			// those that are not (false)
			IDictionary<bool, List<T>> b = GroupBy<T, bool>(initial,
											delegate(T x) { return CollectionUtils.Contains(desired, delegate(T y) { return x.IsSame(y); }); });

			// these items need to be added
			List<T> adds = a[false];
			foreach (T add in adds)
				changes.AddRange(addProcessor(add));

			// these items need to be dropped
			List<T> drops = b[false];
			foreach (T drop in drops)
				changes.AddRange(dropProcessor(drop));

			// these items exist in both sets, so they need to be compared one by one
			// first need to sort these vectors so that they are aligned
			// TODO: this won't work for Constraints, which may have differeing sort keys even though they are the same object!!!
			List<T> desiredCommon = CollectionUtils.Sort(a[true], delegate(T x, T y) { return x.SortKey.CompareTo(y.SortKey); });
			List<T> initialCommon = CollectionUtils.Sort(b[true], delegate(T x, T y) { return x.SortKey.CompareTo(y.SortKey); });
            
			// compare each of the common items
			for (int i = 0; i < initialCommon.Count; i++)
			{
				changes.AddRange(compareProcessor(initialCommon[i], desiredCommon[i]));
			}

			return changes;
		}

		public IDictionary<K, List<T>> GroupBy<T, K>(IEnumerable<T> items, Converter<T, K> keyFunc)
		{
			Dictionary<K, List<T>> results = new Dictionary<K, List<T>>();
			foreach (T item in items)
			{
				K key = keyFunc(item);
				List<T> group;
				if (!results.TryGetValue(key, out group))
				{
					results[key] = group = new List<T>();
				}
				group.Add(item);
			}
			return results;
		}
	}
}