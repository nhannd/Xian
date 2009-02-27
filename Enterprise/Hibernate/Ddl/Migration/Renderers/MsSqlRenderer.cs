using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Ddl.Migration;
using NHibernate.Dialect;
using ClearCanvas.Common.Utilities;
using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration.Renderers
{
	/// <summary>
	/// Implementation of <see cref="IRenderer"/> for MS-SQL 2000 and greater.
	/// </summary>
    class MsSqlRenderer : Renderer
    {
		public MsSqlRenderer(Configuration config)
			:base(config)
		{
		}

        public override IEnumerable<RelationalModelChange> PreFilter(IEnumerable<RelationalModelChange> changes)
        {
            List<RelationalModelChange> filtered = new List<RelationalModelChange>(changes);
            foreach (RelationalModelChange change in changes)
            {
                // if a primary key is being added
                if (change is AddPrimaryKeyChange)
                {
                    // check if the table is being added, in which case
                    // the primary key will be embedded in the CREATE TABLE statement
                    if (IsTableAdded(changes, change.Table))
                        filtered.Remove(change);
                }

                // if a primary key is being dropped
                if (change is DropPrimaryKeyChange)
                {
                    // check if the table is being dropped, in which case
                    // the primary key will be dropped automatically
                    if (IsTableDropped(changes, change.Table))
                        filtered.Remove(change);
                }
            }

			return base.PreFilter(filtered);
        }

        private bool IsTableAdded(IEnumerable<RelationalModelChange> changes, TableInfo table)
        {
            return CollectionUtils.Contains(changes,
                delegate(RelationalModelChange c)
                {
                    return c is AddTableChange && Equals(c.Table, table);
                });
        }

        private bool IsTableDropped(IEnumerable<RelationalModelChange> changes, TableInfo table)
        {
            return CollectionUtils.Contains(changes,
                delegate(RelationalModelChange c)
                {
                    return c is DropTableChange && Equals(c.Table, table);
                });
        }
    }
}
