using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using NHibernate.Mapping;
using NHibernate.Cfg;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    [DataContract]
    public class ForeignKeyInfo : ConstraintInfo
    {
		private string _referencedTable;
		private List<string> _referencedColumns;

		public ForeignKeyInfo()
        {

        }

        public ForeignKeyInfo(ForeignKey fk, Configuration config)
            : base(fk)
        {
            //note: the fk object has a ReferencedTable property, but it doesn't always seem to be set
            //the reference class property is always set, so we use it instead to get the referenced table 
        	Table table = config.GetClassMapping(fk.ReferencedClass).Table;
            _referencedTable = table.Name;
			_referencedColumns = CollectionUtils.Map<Column, string>(
				table.PrimaryKey.ColumnCollection,
				delegate(Column column) { return column.Name; });
		}

    	[DataMember]
    	public string ReferencedTable
    	{
			get { return _referencedTable; }
			private set { _referencedTable = value; }
    	}

    	[DataMember]
    	public List<string> ReferencedColumns
    	{
			get { return _referencedColumns; }
			private set { _referencedColumns = value; }
    	}

		public bool Matches(ForeignKeyInfo that)
        {
            return this.ReferencedTable == that.ReferencedTable
				&& CollectionUtils.Equal<string>(this.ReferencedColumns, that.ReferencedColumns, false)
				&& base.Matches(that);
        }

        public override string Identity
        {
            get
            {
				// note that the identity is based entirely on the column names, not the name of the constraint
				// the column names are sorted because we want the identity to be independent of column ordering
				return this.ReferencedTable
					+ StringUtilities.Combine(CollectionUtils.Sort(this.ReferencedColumns), "")
					+ base.Identity;
            }
        }
    }
}
