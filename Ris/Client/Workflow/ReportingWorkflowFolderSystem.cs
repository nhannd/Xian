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
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionPoint]
	public class ReportingWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class ReportingWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class ReportingWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.Reporting)]
	public class ReportingWorkflowFolderSystem
		: ReportingWorkflowFolderSystemBase<ReportingWorkflowFolderExtensionPoint, ReportingWorkflowFolderToolExtensionPoint,
			ReportingWorkflowItemToolExtensionPoint>
	{
		public ReportingWorkflowFolderSystem()
			: base(SR.TitleReportingFolderSystem)
		{
		}

		protected override void AddDefaultFolders()
		{
			// add the personal folders, since they are not extensions and will not be automatically added
			this.Folders.Add(new Folders.Reporting.AssignedFolder());

			if (CurrentStaffCanSupervise())
			{
				this.Folders.Add(new Folders.Reporting.AssignedForReviewFolder());
			}

			this.Folders.Add(new Folders.Reporting.DraftFolder());

			if (ReportingSettings.Default.EnableTranscriptionWorkflow)
			{
				this.Folders.Add(new Folders.Reporting.InTranscriptionFolder());
				this.Folders.Add(new Folders.Reporting.ReviewTranscriptionFolder());
			}

			if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.SubmitForReview))
				this.Folders.Add(new Folders.Reporting.AwaitingReviewFolder());

			this.Folders.Add(new Folders.Reporting.VerifiedFolder());
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItem> items)
		{
			return WebResourcesSettings.Default.ReportingFolderSystemUrl;
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new Folders.Reporting.ReportingSearchFolder();
		}
	}
}
