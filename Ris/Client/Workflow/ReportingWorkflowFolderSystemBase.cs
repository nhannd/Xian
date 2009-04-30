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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public interface IReportingWorkflowItemToolContext : IWorkflowItemToolContext<ReportingWorklistItem>
	{
	}

	public interface IReportingWorkflowFolderToolContext : IWorkflowFolderToolContext
	{
	}

	public abstract class ReportingWorkflowFolderSystemBase<TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint>
		: WorklistFolderSystem<ReportingWorklistItem, TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint, IReportingWorkflowService>
		where TFolderExtensionPoint : ExtensionPoint<IWorklistFolder>, new()
		where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
	{
		class ReportingWorkflowItemToolContext : WorkflowItemToolContext, IReportingWorkflowItemToolContext
		{
			public ReportingWorkflowItemToolContext(WorkflowFolderSystem owner)
				: base(owner)
			{
			}
		}

		class ReportingWorkflowFolderToolContext : WorkflowFolderToolContext, IReportingWorkflowFolderToolContext
		{
			public ReportingWorkflowFolderToolContext(WorkflowFolderSystem owner)
				: base(owner)
			{
			}
		}

		protected ReportingWorkflowFolderSystemBase(string title)
			: base(title)
		{
		}

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new ReportingWorkflowFolderToolContext(this);
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new ReportingWorkflowItemToolContext(this);
		}

		protected static bool CurrentStaffCanSupervise()
		{
			string filters = ReportingSettings.Default.SupervisorStaffTypeFilters;
			List<string> staffTypes = string.IsNullOrEmpty(filters)
										? new List<string>()
										: CollectionUtils.Map<string, string>(filters.Split(','), delegate(string s) { return s.Trim(); });
			string currentUserStaffType = LoginSession.Current.Staff.StaffType.Code;
			return staffTypes.Contains(currentUserStaffType);
		}
	}
}
