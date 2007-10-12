#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt.Folders
{
    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.RegistrationScheduledWorklist)]
    public class ScheduledFolder : RegistrationWorkflowFolder
    {
        public ScheduledFolder(RegistrationWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			((SimpleActionModel)this.MenuModel).AddAction("ScheduledOption", "Option", "EditToolSmall.png", "Option",
                delegate() { DisplayOption(folderSystem.DesktopWindow); });

            this.RefreshTime = 0;
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Scheduled";
        }

        public ScheduledFolder(RegistrationWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Scheduled", null, null)
        {
        }

        public ScheduledFolder()
            : this(null)
        {
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            FolderOptionComponent optionComponent = new FolderOptionComponent(this.RefreshTime);
            if (ApplicationComponent.LaunchAsDialog(desktopWindow, optionComponent, "Option") == ApplicationComponentExitCode.Normal)
            {
                this.RefreshTime = optionComponent.RefreshTime;
            }
        }
    }

    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.RegistrationCheckedInWorklist)]
    public class CheckedInFolder : RegistrationWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<RegistrationWorklistItem>>
        {
        }

        public CheckedInFolder(RegistrationWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, new DropHandlerExtensionPoint())
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            ((SimpleActionModel)this.MenuModel).AddAction("ScheduledOption", "Option", "EditToolSmall.png", "Option",
                delegate() { DisplayOption(folderSystem.DesktopWindow); });

            this.RefreshTime = 0;
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Registration.Worklists+CheckIn";
        }

        public CheckedInFolder(RegistrationWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Checked In", null, null)
        {
        }

        public CheckedInFolder()
            : this(null)
        {
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            FolderOptionComponent optionComponent = new FolderOptionComponent(this.RefreshTime);
            if (ApplicationComponent.LaunchAsDialog(desktopWindow, optionComponent, "Option") == ApplicationComponentExitCode.Normal)
            {
                this.RefreshTime = optionComponent.RefreshTime;
            }
        }
    }

    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.RegistrationInProgressWorklist)]
    public class InProgressFolder : RegistrationWorkflowFolder
    {
        public InProgressFolder(RegistrationWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			((SimpleActionModel)this.MenuModel).AddAction("ScheduledOption", "Option", "EditToolSmall.png", "Option",
                delegate() { DisplayOption(folderSystem.DesktopWindow); });

            this.RefreshTime = 0;
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Registration.Worklists+InProgress";
        }

        public InProgressFolder(RegistrationWorkflowFolderSystem folderSystem)
            : this(folderSystem, "In Progress", null, null)
        {
        }

        public InProgressFolder()
            : this(null)
        {
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            FolderOptionComponent optionComponent = new FolderOptionComponent(this.RefreshTime);
            if (ApplicationComponent.LaunchAsDialog(desktopWindow, optionComponent, "Option") == ApplicationComponentExitCode.Normal)
            {
                this.RefreshTime = optionComponent.RefreshTime;
            }
        }
    }

    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.RegistrationCompletedWorklist)]
    public class CompletedFolder : RegistrationWorkflowFolder
    {
        public CompletedFolder(RegistrationWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			((SimpleActionModel)this.MenuModel).AddAction("ScheduledOption", "Option", "EditToolSmall.png", "Option",
                delegate() { DisplayOption(folderSystem.DesktopWindow); });

            this.RefreshTime = 0;
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Completed";
        }

        public CompletedFolder(RegistrationWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Completed", null, null)
        {
        }

        public CompletedFolder()
            : this(null)
        {
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            FolderOptionComponent optionComponent = new FolderOptionComponent(this.RefreshTime);
            if (ApplicationComponent.LaunchAsDialog(desktopWindow, optionComponent, "Option") == ApplicationComponentExitCode.Normal)
            {
                this.RefreshTime = optionComponent.RefreshTime;
            }
        }
    }

    [ExtensionOf(typeof(WorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.RegistrationCancelledWorklist)]
    public class CancelledFolder : RegistrationWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<RegistrationWorklistItem>>
        {
        }

        public CancelledFolder(RegistrationWorkflowFolderSystem folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, new DropHandlerExtensionPoint())
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			((SimpleActionModel)this.MenuModel).AddAction("ScheduledOption", "Option", "EditToolSmall.png", "Option",
                delegate() { DisplayOption(folderSystem.DesktopWindow); });

            this.RefreshTime = 0;
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Cancelled";
        }

        public CancelledFolder(RegistrationWorkflowFolderSystem folderSystem)
            : this(folderSystem, "Cancelled", null, null)
        {
        }

        public CancelledFolder()
            : this(null)
        {
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            FolderOptionComponent optionComponent = new FolderOptionComponent(this.RefreshTime);
            if (ApplicationComponent.LaunchAsDialog(desktopWindow, optionComponent, "Option") == ApplicationComponentExitCode.Normal)
            {
                this.RefreshTime = optionComponent.RefreshTime;
            }
        }
    }

    public class RegistrationSearchFolder : RegistrationWorkflowFolder
    {
        private SearchData _searchData;

        public RegistrationSearchFolder(RegistrationWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Search")
        {
			this.OpenIconSet = new IconSet(IconScheme.Colour, "SearchFolderOpenSmall.png", "SearchFolderOpenMedium.png", "SearchFolderOpenLarge.png");
			this.ClosedIconSet = new IconSet(IconScheme.Colour, "SearchFolderClosedSmall.png", "SearchFolderClosedMedium.png", "SearchFolderClosedLarge.png");
            if (this.IsOpen)
                this.IconSet = this.OpenIconSet;
            else
                this.IconSet = this.ClosedIconSet;

            this.RefreshTime = 0;
            //this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Search";
        }

        public SearchData SearchData
        {
            get { return _searchData; }
            set
            {
                _searchData = value;
                this.Refresh();
            }
        }

        protected override bool CanQuery()
        {
            if (this.SearchData != null)
                return true;

            return false;
        }

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            List<RegistrationWorklistItem> worklistItems = null;
            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    SearchResponse response = service.Search(new SearchRequest(this.SearchData));
                    worklistItems = response.WorklistItems;
                });

            if (worklistItems == null)
                worklistItems = new List<RegistrationWorklistItem>();

            return worklistItems;
        }

        protected override int QueryCount()
        {
            return this.ItemCount;
        }

    }
}
