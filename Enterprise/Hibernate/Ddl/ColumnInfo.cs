using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using NHibernate.Mapping;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    [DataContract]
    public class ColumnInfo : ElementInfo
    {
		private string _name;
		private int _length;
		private bool _nullable;
		private bool _unique;
		private string _sqlType;


        public ColumnInfo()
        {

        }

        public ColumnInfo(Column column, Table ownerTable, Configuration config, Dialect dialect)
        {
            _name = column.Name;
            _length = column.Length;
            _nullable = column.IsNullable;
            _unique = column.IsUnique;
            _sqlType = column.GetSqlType(dialect, new Mapping(config));
        }

    	[DataMember]
    	public string Name
    	{
			get { return _name; }
			private set { _name = value; }
    	}

    	[DataMember]
    	public int Length
    	{
			get { return _length; }
			private set { _length = value; }
    	}

    	[DataMember]
    	public bool Nullable
    	{
			get { return _nullable; }
			private set { _nullable = value; }
		}

    	[DataMember]
    	public bool Unique
    	{
			get { return _unique; }
			private set { _unique = value; }
    	}

    	[DataMember]
    	public string SqlType
    	{
			get { return _sqlType; }
			private set { _sqlType = value; }
    	}

		public bool Matches(ColumnInfo that)
        {
            return this.Name == that.Name
                && this.Length == that.Length
                && this.Nullable == that.Nullable
                && this.Unique == that.Unique
                && this.SqlType == that.SqlType;
        }

        public override string Identity
        {
            get { return this.Name; }
        }
    }
}
