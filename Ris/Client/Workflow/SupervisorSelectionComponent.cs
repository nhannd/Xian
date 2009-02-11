using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class SupervisorSelectionComponent : StaffSelectionComponent
	{
		protected override string[] StaffTypes
		{
			get
			{
				// create supervisor lookup handler, using filters supplied in application settings
				string filters = ReportingSettings.Default.SupervisorStaffTypeFilters;
				string[] staffTypes = string.IsNullOrEmpty(filters)
										? new string[] { }
										: CollectionUtils.Map<string, string>(filters.Split(','), delegate(string s) { return s.Trim(); }).ToArray();
				return staffTypes;
			}
		}

		public override string LabelText
		{
			get { return "Supervisor"; }
		}
	}

	public class ReportingSupervisorSelectionComponent : SupervisorSelectionComponent
	{
		protected override string DefaultSupervisorID
		{
			get { return ReportingSettings.Default.SupervisorID; }
		}

		protected override void SetStaff(StaffSummary staff)
		{
			base.SetStaff(staff);
			ReportingSettings.Default.SupervisorID = staff == null ? "" : staff.StaffId;
			ReportingSettings.Default.Save();
		}
	}

	public class ProtocollingSupervisorSelectionComponent : SupervisorSelectionComponent
	{
		protected override string DefaultSupervisorID
		{
			get { return ProtocollingSettings.Default.SupervisorID; }
		}

		protected override void SetStaff(StaffSummary staff)
		{
			base.SetStaff(staff);
			ProtocollingSettings.Default.SupervisorID = staff == null ? "" : staff.StaffId;
			ProtocollingSettings.Default.Save();
		}
	}
}