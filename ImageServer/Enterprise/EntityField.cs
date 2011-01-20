#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
