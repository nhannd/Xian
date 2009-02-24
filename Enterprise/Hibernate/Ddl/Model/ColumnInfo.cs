using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using NHibernate.Mapping;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Model
{
    [DataContract]
    public class ColumnInfo : ElementInfo
    {
        public ColumnInfo()
        {

        }

        public ColumnInfo(Column column, Table ownerTable, Configuration config, Dialect dialect)
        {
            this.Name = column.Name;
            this.Length = column.Length;
            this.Nullable = column.IsNullable;
            this.Unique = column.IsUnique;
            this.SqlType = column.GetSqlType(dialect, new Mapping(config));
        }

        [DataMember]
        public string Name;
        [DataMember]
        public int Length;
        [DataMember]
        public bool Nullable;
        [DataMember]
        public bool Unique;
        [DataMember]
        public string SqlType;

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
