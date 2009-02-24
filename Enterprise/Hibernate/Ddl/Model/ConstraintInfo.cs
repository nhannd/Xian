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

        public bool Matches(ConstraintInfo that)
        {
            return this.Name == that.Name &&
                CollectionUtils.Equal<string>(this.Columns, that.Columns, false);
        }

        public override string Identity
        {
            get
            {
				// note that the identity is based entirely on the column names, not the name of the constraint
				// the column names are sorted because we want the identity to be independent of column ordering
            	return StringUtilities.Combine(CollectionUtils.Sort(this.Columns), "");
            }
        }
    }
}
