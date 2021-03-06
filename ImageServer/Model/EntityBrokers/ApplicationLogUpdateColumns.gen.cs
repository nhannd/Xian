#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0//

#endregion

// This file is auto-generated by the ClearCanvas.Model.SqlServer.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    using System;
    using System.Xml;
    using ClearCanvas.ImageServer.Enterprise;

   public class ApplicationLogUpdateColumns : EntityUpdateColumns
   {
       public ApplicationLogUpdateColumns()
       : base("ApplicationLog")
       {}
        [EntityFieldDatabaseMappingAttribute(TableName="ApplicationLog", ColumnName="Host")]
        public String Host
        {
            set { SubParameters["Host"] = new EntityUpdateColumn<String>("Host", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ApplicationLog", ColumnName="Timestamp")]
        public DateTime Timestamp
        {
            set { SubParameters["Timestamp"] = new EntityUpdateColumn<DateTime>("Timestamp", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ApplicationLog", ColumnName="LogLevel")]
        public String LogLevel
        {
            set { SubParameters["LogLevel"] = new EntityUpdateColumn<String>("LogLevel", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ApplicationLog", ColumnName="Thread")]
        public String Thread
        {
            set { SubParameters["Thread"] = new EntityUpdateColumn<String>("Thread", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ApplicationLog", ColumnName="Message")]
        public String Message
        {
            set { SubParameters["Message"] = new EntityUpdateColumn<String>("Message", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ApplicationLog", ColumnName="Exception")]
        public String Exception
        {
            set { SubParameters["Exception"] = new EntityUpdateColumn<String>("Exception", value); }
        }
    }
}
