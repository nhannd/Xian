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

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    public interface IReportingWorkflowItemToolContext : IWorkflowItemToolContext<ReportingWorklistItem>
    {
        ReportingWorkflowFolderSystemBase FolderSystem { get; }
    }

    public interface IReportingWorkflowFolderToolContext : IWorkflowFolderToolContext
    {
    }

    public abstract class ReportingWorkflowFolderSystemBase : WorkflowFolderSystem<ReportingWorklistItem>
    {
		class ReportingWorkflowItemToolContext : WorkflowItemToolContext, IReportingWorkflowItemToolContext
        {
            private readonly ReportingWorkflowFolderSystemBase _owner;

            public ReportingWorkflowItemToolContext(ReportingWorkflowFolderSystemBase owner)
				:base(owner)
            {
                _owner = owner;
            }

            #region IReportingWorkflowItemToolContext Members

            public ReportingWorkflowFolderSystemBase FolderSystem
            {
                get { return _owner; }
            }

            #endregion
        }

		class ReportingWorkflowFolderToolContext : WorkflowFolderToolContext, IReportingWorkflowFolderToolContext
        {
            public ReportingWorkflowFolderToolContext(ReportingWorkflowFolderSystemBase owner)
				:base(owner)
            {
            }
        }

        private IDictionary<string, bool> _workflowEnablement;

        public ReportingWorkflowFolderSystemBase(
			string title,
            IFolderExplorerToolContext folderExplorer,
            ExtensionPoint<IFolder> folderExtensionPoint,
            ExtensionPoint<ITool> itemToolExtensionPoint,
            ExtensionPoint<ITool> folderToolExtensionPoint)
            : base(title, folderExplorer, folderExtensionPoint)
        {
            _itemTools = new ToolSet(itemToolExtensionPoint, new ReportingWorkflowItemToolContext(this));
            _folderTools = new ToolSet(folderToolExtensionPoint, new ReportingWorkflowFolderToolContext(this));
        }

		protected override ListWorklistsForUserResponse QueryWorklistSet(ListWorklistsForUserRequest request)
		{
			ListWorklistsForUserResponse response = null;
			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					response = service.ListWorklistsForUser(request);
				});

			return response;
		}

		protected override IDictionary<string, bool> QueryOperationEnablement(ISelection selection)
		{
			IDictionary<string, bool> enablement = null;
			Platform.GetService<IReportingWorkflowService>(
				delegate(IReportingWorkflowService service)
				{
					ReportingWorklistItem item = (ReportingWorklistItem)selection.Item;
					GetOperationEnablementResponse response = service.GetOperationEnablement(new GetOperationEnablementRequest(item.ProcedureStepRef));
					enablement = response.OperationEnablementDictionary;
				});
			return enablement;
		}
    }
}
