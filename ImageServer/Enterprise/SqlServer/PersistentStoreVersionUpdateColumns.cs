#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    public class PersistentStoreVersionUpdateColumns : EntityUpdateColumns
    {
        public PersistentStoreVersionUpdateColumns()
            : base("DatabaseVersion_")
        { }
        [EntityFieldDatabaseMapping(TableName = "DatabaseVersion_", ColumnName = "Build_")]
        public String Build
        {
            set { SubParameters["Build"] = new EntityUpdateColumn<String>("Build", value); }
        }
        [EntityFieldDatabaseMapping(TableName = "DatabaseVersion_", ColumnName = "Major_")]
        public String Major
        {
            set { SubParameters["Major"] = new EntityUpdateColumn<String>("Major", value); }
        }
        [EntityFieldDatabaseMapping(TableName = "DatabaseVersion_", ColumnName = "Minor_")]
        public String Minor
        {
            set { SubParameters["Minor"] = new EntityUpdateColumn<String>("Minor", value); }
        }
        [EntityFieldDatabaseMapping(TableName = "DatabaseVersion_", ColumnName = "Revision_")]
        public String Revision
        {
            set { SubParameters["Revision"] = new EntityUpdateColumn<String>("Revision", value); }
        }
    }
}