using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using NHibernate.Mapping;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Model
{
    [DataContract]
    public class ConstraintInfo : ElementInfo
    {
        public ConstraintInfo()
        {

        }

        public ConstraintInfo(Constraint constraint)
        {
            this.Name = constraint.Name;
            this.Columns = CollectionUtils.Map<Column, string>(
                constraint.ColumnCollection,
                delegate(Column column) { return column.Name; });
        }

        [DataMember]
        public string Name;

        [DataMember]
        public List<string> Columns;

        public override bool IsSame(ElementInfo other)
        {
            ConstraintInfo that = other as ConstraintInfo;
            if (that == null)
                return false;

            // the order of columns in constraints doesn't really matter, does it?
            // for association/collection tables, the columns may be in arbitrary order,
            // so we can't be sensitive to order
            // for other tables, there should only every be one column in the PK
            return CollectionUtils.Equal<string>(this.Columns, that.Columns, false);
        }

        public override bool IsIdentical(ElementInfo other)
        {
            ConstraintInfo that = (ConstraintInfo)other;
            return this.Name == that.Name &&
                CollectionUtils.Equal<string>(this.Columns, that.Columns, false);
        }

        public override string SortKey
        {
            get { return StringUtilities.Combine(this.Columns, ""); }
        }
    }
}
