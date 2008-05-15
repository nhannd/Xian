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

using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Reporting
{
    [ExtensionPoint]
    public class ReportingMainWorkflowFolderExtensionPoint : ExtensionPoint<IFolder>
    {
    }

    [ExtensionPoint]
    public class ReportingMainWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionPoint]
    public class ReportingMainWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public class ReportingMainWorkflowFolderSystem : ReportingWorkflowFolderSystemBase, ISearchDataHandler
    {
        private readonly Folders.SearchFolder _searchFolder;

        public ReportingMainWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
            : base(folderExplorer, 
            new ReportingMainWorkflowFolderExtensionPoint(),
            new ReportingMainWorkflowItemToolExtensionPoint(),
            new ReportingMainWorkflowFolderToolExtensionPoint())
        {
			if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Development.ViewUnfilteredWorkflowFolders))
            {
                this.AddFolder(new Folders.ToBeReportedFolder(this));
            }

            this.AddFolder(new Folders.AssignedFolder(this));
            this.AddFolder(new Folders.DraftFolder(this));

            if (ReportingSettings.Default.EnableTranscriptionWorkflow)
                this.AddFolder(new Folders.InTranscriptionFolder(this));

            this.AddFolder(new Folders.ToBeVerifiedFolder(this));
            this.AddFolder(new Folders.VerifiedFolder(this));

            if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.UnsupervisedReporting))
                this.AddFolder(new Folders.ReviewResidentReportFolder(this));


            this.AddFolder(_searchFolder = new Folders.SearchFolder(this));
            folderExplorer.RegisterSearchDataHandler(this);
        }

        public override string DisplayName
        {
            get { return "Reporting"; }
        }

        public override string PreviewUrl
        {
            get { return WebResourcesSettings.Default.RadiologistFolderSystemUrl; }
        }

        public SearchData SearchData
        {
            set
            {
                _searchFolder.SearchData = value;
                SelectedFolder = _searchFolder;
            }
        }

        public override void SelectedItemDoubleClickedEventHandler(object sender, System.EventArgs e)
        {
            base.SelectedItemDoubleClickedEventHandler(sender, e);

            EditReportTool editTool = (EditReportTool)CollectionUtils.SelectFirst(this.ItemTools.Tools,
                delegate(ITool tool) { return tool is EditReportTool; });

            if (editTool != null && editTool.Enabled)
                editTool.Apply();
        }
    }
}
