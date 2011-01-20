#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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

			var staffComponent = new StaffGroupSummaryComponent(true, query, _electiveGroupsOnly);
			var exitCode = ApplicationComponent.LaunchAsDialog(
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
				service => response = service.TextQuery(request));
			return response;
		}
	}
}