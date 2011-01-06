#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
	public class PersistentStoreVersionSelectCriteria : EntitySelectCriteria
	{
		public PersistentStoreVersionSelectCriteria()
			: base("DatabaseVersion_")
		{ }

		public override object Clone()
		{
			throw new System.NotImplementedException();
		}

		[EntityFieldDatabaseMappingAttribute(TableName = "DatabaseVersion_", ColumnName = "Build_")]
		public ISearchCondition<System.String> Build
		{
			get
			{
				if (!SubCriteria.ContainsKey("Build_"))
				{
					SubCriteria["Build_"] = new SearchCondition<System.String>("Build_");
				}
				return (ISearchCondition<System.String>)SubCriteria["Build_"];
			}
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "DatabaseVersion_", ColumnName = "Major_")]
		public ISearchCondition<System.String> Major
		{
			get
			{
				if (!SubCriteria.ContainsKey("Major_"))
				{
					SubCriteria["Major_"] = new SearchCondition<System.String>("Major_");
				}
				return (ISearchCondition<System.String>)SubCriteria["Major_"];
			}
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "DatabaseVersion_", ColumnName = "Minor_")]
		public ISearchCondition<System.String> Minor
		{
			get
			{
				if (!SubCriteria.ContainsKey("Minor_"))
				{
					SubCriteria["Minor_"] = new SearchCondition<System.String>("Minor_");
				}
				return (ISearchCondition<System.String>)SubCriteria["Minor_"];
			}
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "DatabaseVersion_", ColumnName = "Revision_")]
		public ISearchCondition<System.String> Revision
		{
			get
			{
				if (!SubCriteria.ContainsKey("Revision_"))
				{
					SubCriteria["Revision_"] = new SearchCondition<System.String>("Revision_");
				}
				return (ISearchCondition<System.String>)SubCriteria["Revision_"];
			}
		}
	}
}
