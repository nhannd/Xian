using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Enterprise
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EntityFieldDatabaseMappingAttribute:Attribute
    {
        private string _tableName;
        private string _columnName;


        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }
    }
}
