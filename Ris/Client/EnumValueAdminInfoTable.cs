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
