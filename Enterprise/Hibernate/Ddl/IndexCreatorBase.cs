using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using NHibernate.Cfg;
using NHibernate.Mapping;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Abstract base class for processors that create indexes.
	/// </summary>
    abstract class IndexCreatorBase : IDdlPreProcessor
    {
        public abstract void Process(Configuration config);

        protected void CreateIndex(Table table, IEnumerable<Column> columns)
        {
            string indexName = string.Format("IX_{0}",
                StringUtilities.Combine(columns, "", delegate(Column c) { return c.Name; }));

            CollectionUtils.ForEach(columns,
                delegate(Column c) { table.GetIndex(indexName).AddColumn(c); });
        }
    }
}
