using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
    /// <summary>
    /// Compares two instances of <see cref="RelationalModelInfo"/> to determine the changes that are needed
    /// to transform one to the other.
    /// </summary>
	class RelationalModelComparator
	{
		delegate IEnumerable<RelationalModelChange> ItemProcessor<T>(T item);
		delegate IEnumerable<RelationalModelChange> CompareItemProcessor<T>(T initial, T desired);


		private readonly EnumOptions _enumOption;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enumOption"></param>
		public RelationalModelComparator(EnumOptions enumOption)
		{
			_enumOption = enumOption;
		}

        /// <summary>
        /// Returns a <see cref="RelationalModelTransform"/> object that describe the changes
        /// required to transform the initial model into the desired model.
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="desired"></param>
        /// <returns></returns>
		public RelationalModelTransform CompareModels(RelationalModelInfo initial, RelationalModelInfo desired)
		{
			List<RelationalModelChange> changes = new List<RelationalModelChange>();

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

			return new RelationalModelTransform(changes);
		}


		private IEnumerable<RelationalModelChange> AddTable(TableInfo t)
		{
			List<RelationalModelChange> changes = new List<RelationalModelChange>();
			changes.Add(new AddTableChange(t));
			changes.AddRange(
				CollectionUtils.Map<IndexInfo, RelationalModelChange>(t.Indexes,
				    delegate(IndexInfo item) { return new AddIndexChange(t, item); }));
			changes.AddRange(
				CollectionUtils.Map<ConstraintInfo, RelationalModelChange>(t.UniqueKeys,
				    delegate(ConstraintInfo item) { return new AddUniqueConstraintChange(t, item); }));
			changes.AddRange(
				CollectionUtils.Map<ForeignKeyInfo, RelationalModelChange>(t.ForeignKeys,
				    delegate(ForeignKeyInfo item) { return new AddForeignKeyChange(t, item); }));
            changes.AddRange(
                CollectionUtils.Map<ConstraintInfo, RelationalModelChange>(new ConstraintInfo[] { t.PrimaryKey } ,
                    delegate(ConstraintInfo item) { return new AddPrimaryKeyChange(t, item); }));
            return changes;
		}

		private IEnumerable<RelationalModelChange> DropTable(TableInfo t)
		{
			List<RelationalModelChange> changes = new List<RelationalModelChange>();
			changes.AddRange(
				CollectionUtils.Map<IndexInfo, RelationalModelChange>(t.Indexes,
					delegate(IndexInfo item) { return new DropIndexChange(t, item); }));
			changes.AddRange(
				CollectionUtils.Map<ConstraintInfo, RelationalModelChange>(t.UniqueKeys,
					delegate(ConstraintInfo item) { return new DropUniqueConstraintChange(t, item); }));
			changes.AddRange(
				CollectionUtils.Map<ForeignKeyInfo, RelationalModelChange>(t.ForeignKeys,
					delegate(ForeignKeyInfo item) { return new DropForeignKeyChange(t, item); }));
            changes.AddRange(
                CollectionUtils.Map<ConstraintInfo, RelationalModelChange>(new ConstraintInfo[] { t.PrimaryKey } ,
                    delegate(ConstraintInfo item) { return new DropPrimaryKeyChange(t, item); }));
            changes.Add(new DropTableChange(t));
			return changes;
		}

		private IEnumerable<RelationalModelChange> CompareTables(TableInfo initial, TableInfo desired)
		{
			TableInfo table = desired;

			List<RelationalModelChange> changes = new List<RelationalModelChange>();
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


		private IEnumerable<RelationalModelChange> AddColumn(TableInfo table, ColumnInfo c)
		{
			return new RelationalModelChange[] { new AddColumnChange(table, c) };
		}

		private IEnumerable<RelationalModelChange> DropColumn(TableInfo table, ColumnInfo c)
		{
			return new RelationalModelChange[] { new DropColumnChange(table, c) };
		}

		private IEnumerable<RelationalModelChange> CompareColumns(TableInfo table, ColumnInfo initial, ColumnInfo desired)
		{
			List<RelationalModelChange> changes = new List<RelationalModelChange>();
			if (!initial.Matches(desired))
				changes.Add(new ModifyColumnChange(table, initial, desired));
			return changes;
		}

		private IEnumerable<RelationalModelChange> AddIndex(TableInfo table, IndexInfo c)
		{
			return new RelationalModelChange[] { new AddIndexChange(table, c) };
		}

		private IEnumerable<RelationalModelChange> DropIndex(TableInfo table, IndexInfo c)
		{
			return new RelationalModelChange[] { new DropIndexChange(table, c) };
		}

		private IEnumerable<RelationalModelChange> CompareIndexes(TableInfo table, IndexInfo initial, IndexInfo desired)
		{
			List<RelationalModelChange> changes = new List<RelationalModelChange>();
			// TODO can indexes be altered or do they need to be dropped and recreated?
			return changes;
		}

		private IEnumerable<RelationalModelChange> AddForeignKey(TableInfo table, ForeignKeyInfo c)
		{
			return new RelationalModelChange[] { new AddForeignKeyChange(table, c) };
		}

		private IEnumerable<RelationalModelChange> DropForeignKey(TableInfo table, ForeignKeyInfo c)
		{
			return new RelationalModelChange[] { new DropForeignKeyChange(table, c) };
		}

		private IEnumerable<RelationalModelChange> CompareForeignKeys(TableInfo table, ForeignKeyInfo initial, ForeignKeyInfo desired)
		{
			List<RelationalModelChange> changes = new List<RelationalModelChange>();
			// TODO can foreign keys be altered or do they need to be dropped and recreated?
			return changes;
		}

		private IEnumerable<RelationalModelChange> AddUniqueConstraint(TableInfo table, ConstraintInfo c)
		{
			return new RelationalModelChange[] { new AddUniqueConstraintChange(table, c) };
		}

		private IEnumerable<RelationalModelChange> DropUniqueConstraint(TableInfo table, ConstraintInfo c)
		{
			return new RelationalModelChange[] { new DropUniqueConstraintChange(table, c) };
		}

		private IEnumerable<RelationalModelChange> CompareUniqueConstraints(TableInfo table, ConstraintInfo initial, ConstraintInfo desired)
		{
			List<RelationalModelChange> changes = new List<RelationalModelChange>();
			// TODO can constraints be altered or do they need to be dropped and recreated?
			return changes;
		}

		private IEnumerable<RelationalModelChange> AddPrimaryKey(TableInfo table, ConstraintInfo item)
		{
			return new RelationalModelChange[] { new AddPrimaryKeyChange(table, item) };
		}

		private IEnumerable<RelationalModelChange> DropPrimaryKey(TableInfo table, ConstraintInfo item)
		{
			return new RelationalModelChange[] { new DropPrimaryKeyChange(table, item) };
		}

		private IEnumerable<RelationalModelChange> ComparePrimaryKeys(TableInfo table, ConstraintInfo initial, ConstraintInfo desired)
		{
			List<RelationalModelChange> changes = new List<RelationalModelChange>();
			// TODO can constraints be altered or do they need to be dropped and recreated?
			return changes;
		}

		private IEnumerable<RelationalModelChange> AddEnumeration(TableInfo table, EnumerationInfo item)
		{
            // check enum options to determine if this item should be considered
			if(_enumOption == EnumOptions.all || (_enumOption == EnumOptions.hard && item.IsHard))
			{
				return CollectionUtils.Map<EnumerationMemberInfo, RelationalModelChange>(
					item.Members,
					delegate(EnumerationMemberInfo member)
					{
						return new AddEnumValueChange(table, member);
					});
			}
			else
			{
				// nothing to do 
				return new RelationalModelChange[] { };
			}
		}

		private IEnumerable<RelationalModelChange> DropEnumeration(TableInfo table, EnumerationInfo item)
		{
			// nothing to do - the table will be dropped
			return new RelationalModelChange[] {};
		}

		private IEnumerable<RelationalModelChange> CompareEnumerations(TableInfo table, EnumerationInfo initial, EnumerationInfo desired)
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
				return new RelationalModelChange[] { };
			}
		}

		private IEnumerable<RelationalModelChange> AddEnumerationValue(TableInfo table, EnumerationMemberInfo item)
		{
			return new RelationalModelChange[] { new AddEnumValueChange(table, item) };
		}

		private IEnumerable<RelationalModelChange> DropEnumerationValue(TableInfo table, EnumerationMemberInfo item)
		{
			return new RelationalModelChange[] { new DropEnumValueChange(table, item) };
		}

		private IEnumerable<RelationalModelChange> CompareEnumerationValues(TableInfo table, EnumerationMemberInfo initial, EnumerationMemberInfo desired)
		{
			// nothing to do - once a value is populated, we do not update it, because it may have been customized
			return new RelationalModelChange[] { };
		}

        /// <summary>
        /// Compares the initial and desired sets of items, forwarding items to the appropriate
        /// callback and returning the aggregate set of results.
        /// </summary>
        /// <remarks>
        /// Items that appear in <paramref name="desired"/> but not in <paramref name="initial"/>
        /// are passed to <paramref name="addProcessor"/>.
        /// Items that appear in <paramref name="initial"/> but not in <paramref name="desired"/>
        /// are passed to <paramref name="dropProcessor"/>.
        /// Items that appear in both sets are passed to <paramref name="compareProcessor"/> for
        /// comparison.
        /// Results of all callbacks are aggregated and returned.
        /// </remarks>
		private IEnumerable<RelationalModelChange> CompareSets<T>(IEnumerable<T> initial, IEnumerable<T> desired,
		                                           ItemProcessor<T> addProcessor,
		                                           ItemProcessor<T> dropProcessor,
												   CompareItemProcessor<T> compareProcessor)
			where T : ElementInfo
		{
			List<RelationalModelChange> changes = new List<RelationalModelChange>();

			// partition desired set into those items that are contained in the initial set (true) and
			// those that are not (false)
			Dictionary<bool, List<T>> a = CollectionUtils.GroupBy<T, bool>(desired,
					delegate(T x) { return CollectionUtils.Contains(initial, delegate(T y) { return Equals(x, y); }); });

			// partition initial set into those items that are contained in the desired set (true) and
			// those that are not (false)
			Dictionary<bool, List<T>> b = CollectionUtils.GroupBy<T, bool>(initial,
                    delegate(T x) { return CollectionUtils.Contains(desired, delegate(T y) { return Equals(x, y); }); });

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
	}
}