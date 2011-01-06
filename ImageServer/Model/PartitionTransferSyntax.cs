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
	public partial class PartitionTransferSyntax
	{
        #region Private Members
        private String _description;
        private Boolean _lossless;
        private String _uid;
        #endregion

        #region Public Properties
		[EntityFieldDatabaseMapping(TableName = "PartitionTransferSyntax", ColumnName = "Description")]
        public String Description
        {
        get { return _description; }
        set { _description = value; }
        }

		[EntityFieldDatabaseMappingAttribute(TableName = "PartitionTransferSyntax", ColumnName = "Lossless")]
        public Boolean Lossless
        {
        get { return _lossless; }
        set { _lossless = value; }
        }

		[EntityFieldDatabaseMappingAttribute(TableName = "PartitionTransferSyntax", ColumnName = "Uid")]
		public String Uid
        {
        get { return _uid; }
        set { _uid = value; }
        }
        #endregion
	}
}
