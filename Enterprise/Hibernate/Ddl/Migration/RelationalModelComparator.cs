using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	class RelationalModelComparator
	{
		delegate IEnumerable<Change> ItemProcessor<T>(T item);
		delegate IEnumerable<Change> CompareItemProcessor<T>(T initial, T desired);

		private static readonly Dictionary<Type, int> _changeOrder = new Dictionary<Type, int>();

		static RelationalModelComparator()
		{
			// define the order that changes should occur to avoid dependency issues
			_changeOrder.Add(typeof(DropIndexChange), 0);
			_changeOrder.Add(typeof(DropForeignKeyChange), 1);
			_changeOrder.Add(typeof(DropUniqueConstraintChange), 2);
			_changeOrder.Add(typeof(DropPrimaryKeyChange), 3);
			_changeOrder.Add(typeof(DropTableChange), 4);
			_changeOrder.Add(typeof(DropColumnChange), 5);
			_changeOrder.Add(typeof(ModifyColumnChange), 6);
			_changeOrder.Add(typeof(AddColumnChange), 7);
			_changeOrder.Add(typeof(AddTableChange), 8);
			_changeOrder.Add(typeof(AddPrimaryKeyChange), 9);
			_changeOrder.Add(typeof(AddUniqueConstraintChange), 10);
			_changeOrder.Add(typeof(AddForeignKeyChange), 11);
			_changeOrder.Add(typeof(AddIndexChange), 12);
			_changeOrder.Add(typeof(DropEnumValueChange), 13);
			_changeOrder.Add(typeof(AddEnumValueChange), 14);
		}

		private readonly EnumOptions _enumOption;

		public RelationalModelComparator(EnumOptions enumOption)
		{
			_enumOption = enumOption;
		}


		public List<Change> CompareDatabases(RelationalModelInfo initial, RelationalModelInfo desired)
		{
			List<Change> changes = new List<Change>();

			// compare tables
			changes.AddRange(
				CompareSets(initial.Tables, desired.Tables,
							  AddTable,
							  DropTable,
							  CompareTables)
			);

			if(_enumOption != EnumOptions.none)
			{
				// compare enumeration values
				changes.AddRange(
					CompareSets(initial.Enumerations, desired.Enumerations,
						delegate(EnumerationInfo item) { return AddEnumeration(desired.GetTable(item.Table), item); },
						delegate(EnumerationInfo item) { return DropEnumeration(initial.GetTable(item.Table), item); },
						delegate(EnumerationInfo x, EnumerationInfo y) { return CompareEnumerations(desired.GetTable(y.Table), x, y); })
				);
			}

			// order changes correctly
			return CollectionUtils.Sort(changes,
						delegate(Change x, Change y)
						{
							int changeOrderValue = _changeOrder[x.GetType()]
								.CompareTo(_changeOrder[y.GetType()]);

							if (changeOrderValue != 0)
								return changeOrderValue;

							// if two changes are of same type, then order by table name
							return x.Table.Name.CompareTo(y.Table.Name);
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
				    delegate(ForeignKeyInfo item) { return new AddForeignKeyChange(t, item); }));
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
			if (!initial.Matches(desired))
				changes.Add(new ModifyColumnChange(table, initial, desired));
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
			// TODO can indexes be altered or do they need to be dropped and recreated?
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
			// TODO can foreign keys be altered or do they need to be dropped and recreated?
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
			// TODO can constraints be altered or do they need to be dropped and recreated?
			return changes;
		}

		private IEnumerable<Change> AddPrimaryKey(TableInfo table, ConstraintInfo item)
		{
			return new Change[] { new AddPrimaryKeyChange(table, item) };
		}

		private IEnumerable<Change> DropPrimaryKey(TableInfo table, ConstraintInfo item)
		{
			return new Change[] { new DropPrimaryKeyChange(table, item) };
		}

		private IEnumerable<Change> ComparePrimaryKeys(TableInfo table, ConstraintInfo initial, ConstraintInfo desired)
		{
			List<Change> changes = new List<Change>();
			// TODO can constraints be altered or do they need to be dropped and recreated?
			return changes;
		}

		private IEnumerable<Change> AddEnumeration(TableInfo table, EnumerationInfo item)
		{
			if(_enumOption == EnumOptions.all || (_enumOption == EnumOptions.hard && item.IsHard))
			{
				return CollectionUtils.Map<EnumerationMemberInfo, Change>(
					item.Members,
					delegate(EnumerationMemberInfo member)
					{
						return new AddEnumValueChange(table, member);
					});
			}
			else
			{
				// nothing to do 
				return new Change[] { };
			}
		}

		private IEnumerable<Change> DropEnumeration(TableInfo table, EnumerationInfo item)
		{
			// nothing to do - the table will be dropped
			return new Change[] {};
		}

		private IEnumerable<Change> CompareEnumerations(TableInfo table, EnumerationInfo initial, EnumerationInfo desired)
		{
			// note: for soft enumerations, we don't do any updates, because they may have been customized already
			// hence only hard enums should ever be compared (need to ensure what is in the database matches the C# enum definition)
			if (_enumOption != EnumOptions.none && desired.IsHard)
			{
				return CompareSets(initial.Members, desired.Members,
						   delegate(EnumerationMemberInfo item) { return AddEnumerationValue(table, item); },
						   delegate(EnumerationMemberInfo item) { return DropEnumerationValue(table, item); },
						   delegate(EnumerationMemberInfo x, EnumerationMemberInfo y) { return CompareEnumerationValues(table, x, y); });
			}
			else
			{
				return new Change[] { };
			}
		}

		private IEnumerable<Change> AddEnumerationValue(TableInfo table, EnumerationMemberInfo item)
		{
			return new Change[] { new AddEnumValueChange(table, item) };
		}

		private IEnumerable<Change> DropEnumerationValue(TableInfo table, EnumerationMemberInfo item)
		{
			return new Change[] { new DropEnumValueChange(table, item) };
		}

		private IEnumerable<Change> CompareEnumerationValues(TableInfo table, EnumerationMemberInfo initial, EnumerationMemberInfo desired)
		{
			// nothing to do - once a value is populated, we do not update it, because it may have been customized
			return new Change[] { };
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
					delegate(T x) { return CollectionUtils.Contains(initial, delegate(T y) { return x.Identity == y.Identity; }); });

			// partition initial set into those items that are contained in the desired set (true) and
			// those that are not (false)
			IDictionary<bool, List<T>> b = GroupBy<T, bool>(initial,
					delegate(T x) { return CollectionUtils.Contains(desired, delegate(T y) { return x.Identity == y.Identity; }); });

			// these items need to be added
			List<T> adds;
			if(a.TryGetValue(false, out adds))
			{
				foreach (T add in adds)
					changes.AddRange(addProcessor(add));
			}

			// these items need to be dropped
			List<T> drops;
			if (b.TryGetValue(false, out drops))
			{
				foreach (T drop in drops)
					changes.AddRange(dropProcessor(drop));
			}

			// these items exist in both sets, so they need to be compared one by one
			// these keys should either both exist, or both not exist
			List<T> desiredCommon = a.ContainsKey(true) ? a[true] : new List<T>();
			List<T> initialCommon = b.ContainsKey(true) ? b[true] : new List<T>();

			// sort these vectors by identity, so that they are aligned
			desiredCommon.Sort(delegate(T x, T y) { return x.Identity.CompareTo(y.Identity); });
			initialCommon.Sort(delegate(T x, T y) { return x.Identity.CompareTo(y.Identity); });
            
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