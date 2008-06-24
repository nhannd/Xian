using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class EnumValueInfoTable : Table<EnumValueInfo>
	{
		public EnumValueInfoTable()
		{
			this.Columns.Add(new TableColumn<EnumValueInfo, string>(SR.ColumnEnumCode,
				delegate(EnumValueInfo info) { return info.Code; }, 0.4F));
			this.Columns.Add(new TableColumn<EnumValueInfo, string>(SR.ColumnEnumValue,
				delegate(EnumValueInfo info) { return info.Value; }, 1.0F));
			this.Columns.Add(new TableColumn<EnumValueInfo, string>(SR.ColumnEnumDescription,
				delegate(EnumValueInfo info) { return info.Description; }, 1.5F));
		}
	}

}
