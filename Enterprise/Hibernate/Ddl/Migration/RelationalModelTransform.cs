using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	/// <summary>
	/// Describes a set of changes that transform a relational model.
	/// </summary>
	class RelationalModelTransform
	{
		private static readonly Dictionary<Type, int> _changeOrder = new Dictionary<Type, int>();

        /// <summary>
        /// Class constructor
        /// </summary>
		static RelationalModelTransform()
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

		private readonly List<RelationalModelChange> _changes;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="changes"></param>
		internal RelationalModelTransform(List<RelationalModelChange> changes)
		{
			_changes = changes;
		}

		/// <summary>
		/// Uses the specified renderer to render this transform.
		/// </summary>
		/// <param name="renderer"></param>
		/// <returns></returns>
		public Statement[] Render(IRenderer renderer)
		{
			// allow the renderer to modify the change set, and then sort the changes appropriately
			List<RelationalModelChange> filteredChanges = OrderChanges(renderer.PreFilter(_changes));

			List<Statement> statements = new List<Statement>();
			foreach (RelationalModelChange change in filteredChanges)
			{
				statements.AddRange(change.GetStatements(renderer));
			}
			return statements.ToArray();
		}

		private List<RelationalModelChange> OrderChanges(IEnumerable<RelationalModelChange> changes)
		{
			// the algorithm here tries to do 2 things:
			// 1. Re-organize groups of changes so as to avoid any dependency problems.
			// 2. Preserve the order of changes as much as possible, not re-ordering anything
			// that doesn't need to be re-ordered to satisfy 1.  This *should* keep changes pertaining to the
			// same table clustered together where possible, and also keep AddEnumValueChanges in order

			// group changes by type
			IDictionary<Type, List<RelationalModelChange>> groupedByType =
				CollectionUtils.GroupBy<RelationalModelChange, Type>(changes, delegate(RelationalModelChange c) { return c.GetType(); });

			// sort the types to avoid dependency issues
			List<Type> sortedTypes = CollectionUtils.Sort(groupedByType.Keys,
						delegate(Type x, Type y)
						{
							return _changeOrder[x].CompareTo(_changeOrder[y]);
						});


			// flatten changes back into a single list
			return CollectionUtils.Concat<RelationalModelChange>(
					CollectionUtils.Map<Type, List<RelationalModelChange>>(sortedTypes,
						delegate(Type t) { return groupedByType[t]; }).ToArray()
				   );
		}
	}
}
