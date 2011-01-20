#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin;

namespace ClearCanvas.Ris.Client
{
	public class EnumValueAdminInfoTable : Table<EnumValueAdminInfo>
	{
		public EnumValueAdminInfoTable()
		{
			this.Columns.Add(new TableColumn<EnumValueAdminInfo, string>(SR.ColumnEnumCode,
				delegate(EnumValueAdminInfo info) { return info.Code; }, 0.4F));
			this.Columns.Add(new TableColumn<EnumValueAdminInfo, string>(SR.ColumnEnumValue,
				delegate(EnumValueAdminInfo info) { return info.Value; }, 1.0F));
			this.Columns.Add(new TableColumn<EnumValueAdminInfo, string>(SR.ColumnEnumDescription,
				delegate(EnumValueAdminInfo info) { return info.Description; }, 1.5F));
		}
	}

}
