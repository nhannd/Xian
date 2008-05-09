using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public class StaffGroupLookupHandler : LookupHandler<TextQueryRequest, StaffGroupSummary>
	{
		private readonly DesktopWindow _desktopWindow;

		public StaffGroupLookupHandler(DesktopWindow desktopWindow)
		{
			_desktopWindow = desktopWindow;
		}

		public override bool ResolveNameInteractive(string query, out StaffGroupSummary result)
		{
			result = null;

			StaffGroupSummaryComponent staffComponent = new StaffGroupSummaryComponent(true, query);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, staffComponent, SR.TitleStaff);

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				result = (StaffGroupSummary)CollectionUtils.FirstElement(staffComponent.SummarySelection.Items);
			}

			return (result != null);
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