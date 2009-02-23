using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Model
{
    [DataContract]
    public class DatabaseSchemaInfo : ElementInfo
    {
        public DatabaseSchemaInfo()
        {

        }

        public DatabaseSchemaInfo(List<TableInfo> tables)
        {
            Tables = tables;
        }

        [DataMember]
        public List<TableInfo> Tables;

        public override bool IsSame(ElementInfo other)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool IsIdentical(ElementInfo other)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string SortKey
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
    }
}
