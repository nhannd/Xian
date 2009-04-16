#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
	public class ProtocolWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class ProtocolWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class ProtocolWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.Protocolling)]
	public class ProtocolWorkflowFolderSystem
		: ReportingWorkflowFolderSystemBase<ProtocolWorkflowFolderExtensionPoint, ProtocolWorkflowFolderToolExtensionPoint,
			ProtocolWorkflowItemToolExtensionPoint>
	{
		public ProtocolWorkflowFolderSystem()
			: base(SR.TitleProtocollingFolderSystem)
		{
		}

		protected override void AddDefaultFolders()
		{
			// add the personal folders, since they are not extensions and will not be automatically added
			this.Folders.Add(new Folders.Reporting.AssignedToBeProtocolFolder());

			if (CurrentStaffCanSupervise())
			{
				this.Folders.Add(new Folders.Reporting.AssignedForReviewProtocolFolder());
			}
			this.Folders.Add(new Folders.Reporting.DraftProtocolFolder());

			if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview))
				this.Folders.Add(new Folders.Reporting.AwaitingApprovalProtocolFolder());

			this.Folders.Add(new Folders.Reporting.CompletedProtocolFolder());
			this.Folders.Add(new Folders.Reporting.RejectedProtocolFolder());
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItem> items)
		{
			return WebResourcesSettings.Default.ProtocollingFolderSystemUrl;
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new Folders.Reporting.ProtocollingSearchFolder();
		}
	}
}
