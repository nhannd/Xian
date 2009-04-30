#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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