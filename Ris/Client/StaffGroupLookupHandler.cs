using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public class StaffGroupLookupHandler : LookupHandler<StaffGroupTextQueryRequest, StaffGroupSummary>
	{
		private readonly DesktopWindow _desktopWindow;
		private readonly bool _electiveGroupsOnly;

		public StaffGroupLookupHandler(DesktopWindow desktopWindow, bool electiveGroupsOnly)
		{
			_desktopWindow = desktopWindow;
			_electiveGroupsOnly = electiveGroupsOnly;
		}

		public override bool ResolveNameInteractive(string query, out StaffGroupSummary result)
		{
			result = null;

			StaffGroupSummaryComponent staffComponent = new StaffGroupSummaryComponent(true, query, _electiveGroupsOnly);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, staffComponent, SR.TitleStaffGroups);

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

		protected override TextQueryResponse<StaffGroupSummary> DoQuery(StaffGroupTextQueryRequest request)
		{
			request.ElectiveGroupsOnly = _electiveGroupsOnly;

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