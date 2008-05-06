using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;

namespace ClearCanvas.Ris.Client
{
	public class StaffGroupLookupHandler : LookupHandler<TextQueryRequest, StaffGroupSummary>
	{
		private DesktopWindow _desktopWindow;

		public StaffGroupLookupHandler(DesktopWindow desktopWindow)
		{
			_desktopWindow = desktopWindow;
		}

		public override bool ResolveNameInteractive(string query, out StaffGroupSummary result)
		{
			result = null;
			return false;
		}

		public override string FormatItem(StaffGroupSummary item)
		{
			return item.Name;
		}

		protected override TextQueryResponse<StaffGroupSummary> DoQuery(TextQueryRequest request)
		{
			TextQueryResponse<StaffGroupSummary> response = null;
			Platform.GetService<IStaffGroupAdminService>(
				delegate(IStaffGroupAdminService service)
					{
						response = service.TextQuery(request);
					});
			return response;
		}
	}
}