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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionPoint]
	public class RadiologistAdminWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class RadiologistAdminWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class RadiologistAdminWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}


	/// <summary>
	/// A folder system that allowing radiologist admin to monitor personal items and reassign to another radiologist
	/// </summary>
	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.RadiologistAdmin)]
	public class RadiologistAdminWorkflowFolderSystem
		: ReportingWorkflowFolderSystemBase<RadiologistAdminWorkflowFolderExtensionPoint, RadiologistAdminWorkflowFolderToolExtensionPoint,
			RadiologistAdminWorkflowItemToolExtensionPoint>
	{
		public RadiologistAdminWorkflowFolderSystem()
			: base(SR.TitleRadiologistAdminFolderSystem)
		{
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItem> items)
		{
			if (items.Count != 1)
				return null;

			ReportingWorklistItem step = CollectionUtils.FirstElement(items);

			//TODO: having the client specify the step name name may not be a terribly good idea
			switch (step.ProcedureStepName)
			{
				case "Interpretation":
				case "Transcription":
				case "Transcription Review":
				case "Verification":
				case "Publication":
					return WebResourcesSettings.Default.ReportingFolderSystemUrl;
				case "Protocol":
					return WebResourcesSettings.Default.ProtocollingFolderSystemUrl;
				default:
					return null;
			}
		}

		public override bool AdvancedSearchEnabled
		{
			get { return false; }
		}

		public override string SearchMessage
		{
			get { return SR.MessageRadiologistAdminSearchMessage; }
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
        {
			return new Folders.RadiologistAdmin.RadiologistAdminSearchFolder();
        }
    }
}
