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

        public override bool IsSame(ElementInfo other)
        {
            IndexInfo that = other as IndexInfo;
            if (that == null)
                return false;

            // indexes are sensitive to column ordering
            return CollectionUtils.Equal<string>(this.Columns, that.Columns, true);
        }

        public override bool IsIdentical(ElementInfo other)
        {
            ConstraintInfo that = (ConstraintInfo)other;
            return this.Name == that.Name &&
                CollectionUtils.Equal<string>(this.Columns, that.Columns, true);
        }

        public override string SortKey
        {
            get { return StringUtilities.Combine(this.Columns, ""); }
        }
    }
}
