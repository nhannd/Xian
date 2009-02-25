using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using NHibernate.Mapping;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    [DataContract]
	public class ConstraintInfo : ElementInfo
    {
		private string _name;
		private List<string> _columns;


        public ConstraintInfo()
        {

        }

        public ConstraintInfo(Constraint constraint)
        {
            _name = constraint.Name;
            _columns = CollectionUtils.Map<Column, string>(
                constraint.ColumnCollection,
                delegate(Column column) { return column.Name; });
        }

    	[DataMember]
    	public string Name
    	{
			get { return _name; }
			protected set { _name = value; }
    	}

    	[DataMember]
    	public List<string> Columns
    	{
			get { return _columns; }
			protected set { _columns = value; }
    	}

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
