using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Ddl.Migration;
using NHibernate.Dialect;
using ClearCanvas.Common.Utilities;
using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration.Renderers
{
    class MsSqlRenderer : Renderer
    {
		public MsSqlRenderer(Configuration config, Dialect dialect)
			:base(config, dialect)
		{
		}

        public override IEnumerable<Change> PreFilter(IEnumerable<Change> changes)
        {
            List<Change> filtered = new List<Change>(changes);
            foreach (Change change in changes)
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

            return base.PreFilter(changes);
        }

        private bool IsTableAdded(IEnumerable<Change> changes, TableInfo table)
        {
            return CollectionUtils.Contains(changes,
                delegate(Change c)
                {
                    return c is AddTableChange && Equals(c.Table, table);
                });
        }

        private bool IsTableDropped(IEnumerable<Change> changes, TableInfo table)
        {
            return CollectionUtils.Contains(changes,
                delegate(Change c)
                {
                    return c is DropTableChange && Equals(c.Table, table);
                });
        }
    }
}
