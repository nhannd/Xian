using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using NHibernate.Mapping;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a column in a relational database model.
	/// </summary>
    [DataContract]
    public class ColumnInfo : ElementInfo
    {
		private string _name;
		private int _length;
		private bool _nullable;
		private bool _unique;
		private string _sqlType;

		/// <summary>
		/// Constructor.
		/// </summary>
        internal ColumnInfo()
        {

        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="config"></param>
		/// <param name="dialect"></param>
		internal ColumnInfo(Column column, Configuration config, Dialect dialect)
        {
            _name = column.Name;
            _length = column.Length;
            _nullable = column.IsNullable;
            _unique = column.IsUnique;
            _sqlType = column.GetSqlType(dialect, new Mapping(config));
        }

		/// <summary>
		/// Gets the column name.
		/// </summary>
    	[DataMember]
    	public string Name
    	{
			get { return _name; }
			private set { _name = value; }
    	}

		/// <summary>
		/// Gets the column length, or -1 if not applicable.
		/// </summary>
    	[DataMember]
    	public int Length
    	{
			get { return _length; }
			private set { _length = value; }
    	}

		/// <summary>
		/// Gets a value indicating whether the column is nullable.
		/// </summary>
    	[DataMember]
    	public bool Nullable
    	{
			get { return _nullable; }
			private set { _nullable = value; }
		}

		/// <summary>
		/// Gets a value indicating whether the column is defined as unique.
		/// </summary>
    	[DataMember]
    	public bool Unique
    	{
			get { return _unique; }
			private set { _unique = value; }
    	}

		/// <summary>
		/// Gets the SQL data type of the column.
		/// </summary>
    	[DataMember]
    	public string SqlType
    	{
			get { return _sqlType; }
			private set { _sqlType = value; }
    	}

		/// <summary>
		/// Returns true if this column matches that, property for property.
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public bool Matches(ColumnInfo that)
        {
            return this.Name == that.Name
                && this.Length == that.Length
                && this.Nullable == that.Nullable
                && this.Unique == that.Unique
                && this.SqlType == that.SqlType;
        }

		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
		public override string Identity
        {
            get { return this.Name; }
        }
    }
}
