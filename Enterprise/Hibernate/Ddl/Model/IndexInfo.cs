using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using NHibernate.Mapping;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Model
{
    [DataContract]
    public class IndexInfo : ElementInfo
    {
        public IndexInfo()
        {

        }

        public IndexInfo(Index index)
        {
            this.Name = index.Name;
            this.Columns = CollectionUtils.Map<Column, string>(
                index.ColumnCollection,
                delegate(Column column) { return column.Name; });
        }

        [DataMember]
        public string Name;

        [DataMember]
        public List<string> Columns;

		public bool Matches(IndexInfo that)
        {
            return this.Name == that.Name &&
                CollectionUtils.Equal<string>(this.Columns, that.Columns, true);
        }

        public override string Identity
        {
            get
            {
				// note that the identity is based entirely on the column names, not the name of the index
				// the column names are *not* sorted because we want the identity to be dependent on column ordering,
				// because the order of columns is important in an index, and multiple indexes may exist that differ
				// only in the order of the columns
            	return StringUtilities.Combine(this.Columns, "");
            }
        }
    }
}
