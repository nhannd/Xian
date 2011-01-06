#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    public partial class PartitionSopClass : ServerEntity
    {
        #region Private Members
        private String _sopClassUid;
        private String _description;
        private bool _nonImage;
        #endregion

		[EntityFieldDatabaseMappingAttribute(TableName = "PartitionSopClass", ColumnName = "SopClassUid")]
		public String SopClassUid
        {
            get { return _sopClassUid; }
            set { _sopClassUid = value; }
        }
		[EntityFieldDatabaseMappingAttribute(TableName = "PartitionSopClass", ColumnName = "Description")]
		public String Description
        {
            get { return _description; }
            set { _description = value; }
        }
		[EntityFieldDatabaseMappingAttribute(TableName = "PartitionSopClass", ColumnName = "NonImage")]
		public bool NonImage
        {
            get { return _nonImage; }
            set { _nonImage = value; }
        }
    }
}
