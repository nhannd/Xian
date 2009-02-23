using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using NHibernate.Mapping;
using NHibernate.Cfg;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Model
{
    [DataContract]
    public class ForeignKeyInfo : ConstraintInfo
    {
        public ForeignKeyInfo()
        {

        }

        public ForeignKeyInfo(ForeignKey fk, Configuration config)
            : base(fk)
        {
            //note: the fk object has a ReferencedTable property, but it doesn't always seem to be set
            //the reference class property is always set, so we use it instead to get the referenced table 
            ReferencedTable = config.GetClassMapping(fk.ReferencedClass).Table.Name;
        }

        [DataMember]
        public string ReferencedTable;

        public override bool IsSame(ElementInfo other)
        {
            ForeignKeyInfo that = other as ForeignKeyInfo;
            if (that == null)
                return false;

            return Equals(this.ReferencedTable, that.ReferencedTable) &&
                CollectionUtils.Equal<string>(this.Columns, that.Columns, false);
        }

        public override bool IsIdentical(ElementInfo other)
        {
            ForeignKeyInfo that = (ForeignKeyInfo)other;
            return this.ReferencedTable == that.ReferencedTable
                && base.IsIdentical(other);
        }

        public override string SortKey
        {
            get { return this.ReferencedTable + base.SortKey; }
        }
    }
}
