using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using NHibernate.Mapping;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a constraint in a relational database model.
	/// </summary>
    [DataContract]
	public class ConstraintInfo : ElementInfo
    {
		private string _name;
		private List<string> _columns;


        internal ConstraintInfo()
        {

        }

		internal ConstraintInfo(Constraint constraint)
        {
            _name = constraint.Name;
            _columns = CollectionUtils.Map<Column, string>(
                constraint.ColumnCollection,
                delegate(Column column) { return column.Name; });
        }

		/// <summary>
		/// Gets the name of the constraint.
		/// </summary>
    	[DataMember]
    	public string Name
    	{
			get { return _name; }
			protected set { _name = value; }
    	}

		/// <summary>
		/// Gets the names of the columns on which the constraint is defined.
		/// </summary>
    	[DataMember]
    	public List<string> Columns
    	{
			get { return _columns; }
			protected set { _columns = value; }
    	}

		/// <summary>
		/// Returns true if this constraint matches that, property for property.
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
        public bool Matches(ConstraintInfo that)
        {
            return this.Name == that.Name &&
                CollectionUtils.Equal<string>(this.Columns, that.Columns, false);
        }

		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
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
