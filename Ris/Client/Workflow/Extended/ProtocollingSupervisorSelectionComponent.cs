#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
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
