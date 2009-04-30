#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
