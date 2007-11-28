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
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt.Folders
{
    [ExtensionOf(typeof(TechnologistMainWorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistScheduledWorklist)]
    [FolderPath("Technologist/Scheduled", true)]
    public class ScheduledTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        public ScheduledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, null)
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.TechnologistScheduledWorklist";
        }

        public ScheduledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public ScheduledTechnologistWorkflowFolder()
            : this(null)
        {
        }
    }

    [ExtensionOf(typeof(TechnologistMainWorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistCheckedInWorklist)]
    [FolderPath("Technologist/Checked In")]
    public class CheckedInTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public CheckedInTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.TechnologistCheckedInWorklist";
        }

        public CheckedInTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public CheckedInTechnologistWorkflowFolder()
            : this(null)
        {
        }
    }

    [ExtensionOf(typeof(TechnologistMainWorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistInProgressWorklist)]
    [FolderPath("Technologist/In Progress")]
    public class InProgressTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public InProgressTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.TechnologistInProgressWorklist";
        }

        public InProgressTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public InProgressTechnologistWorkflowFolder()
            : this(null)
        {
        }
    }

    [ExtensionOf(typeof(TechnologistMainWorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistCompletedWorklist)]
    [FolderPath("Technologist/Completed")]
    public class CompletedTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public CompletedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.TechnologistCompletedWorklist";
        }

        public CompletedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public CompletedTechnologistWorkflowFolder()
            : this(null)
        {
        }
    }

    [ExtensionOf(typeof(TechnologistMainWorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistUndocumentedWorklist)]
    [FolderPath("Technologist/Undocumented")]
    public class UndocumentedTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        public UndocumentedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef)
        {
            this.MenuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            ((SimpleActionModel)this.MenuModel).AddAction("ScheduledOption", "Option", "EditToolSmall.png", "Option",
                delegate { DisplayOption(folderSystem.DesktopWindow); });

            this.RefreshTime = 0;
            this.WorklistClassName = "ClearCanvas.Healthcare.TechnologistUndocumentedWorklist";
        }

        public UndocumentedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public UndocumentedTechnologistWorkflowFolder()
            : this(null)
        {
        }

        private void DisplayOption(IDesktopWindow desktopWindow)
        {
            FolderOptionComponent optionComponent = new FolderOptionComponent(this.RefreshTime);
            if (ApplicationComponent.LaunchAsDialog(desktopWindow, optionComponent, "Option") == ApplicationComponentExitCode.Accepted)
            {
                this.RefreshTime = optionComponent.RefreshTime;
            }
        }
    }

    //[ExtensionOf(typeof(TechnologistMainWorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistSuspendedWorklist)]
    [FolderPath("Technologist/Suspended")]
    public class SuspendedTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public SuspendedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.TechnologistSuspendedWorklist";
        }

        public SuspendedTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public SuspendedTechnologistWorkflowFolder()
            : this(null)
        {
        }
    }

    [ExtensionOf(typeof(TechnologistMainWorkflowFolderExtensionPoint))]
    [FolderForWorklistType(WorklistTokens.TechnologistCancelledWorklist)]
    [FolderPath("Technologist/Cancelled")]
    public class CancelledTechnologistWorkflowFolder : TechnologistWorkflowFolder
    {
        [ExtensionPoint]
        public class DropHandlerExtensionPoint : ExtensionPoint<IDropHandler<ModalityWorklistItem>>
        {
        }

        public CancelledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem, string folderDisplayName, string folderDescription, EntityRef worklistRef)
            : base(folderSystem, folderDisplayName, folderDescription, worklistRef, new DropHandlerExtensionPoint())
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.TechnologistCancelledWorklist";
        }

        public CancelledTechnologistWorkflowFolder(TechnologistWorkflowFolderSystemBase folderSystem)
            : this(folderSystem, null, null, null)
        {
        }

        public CancelledTechnologistWorkflowFolder()
            : this(null)
        {
        }
    }

    [FolderPath("Search")]
    public class TechnologistSearchFolder : TechnologistWorkflowFolder
    {
        private SearchData _searchData;

        public TechnologistSearchFolder(TechnologistWorkflowFolderSystemBase folderSystem)
            : base(folderSystem, null)
        {
            this.OpenIconSet = new IconSet(IconScheme.Colour, "SearchFolderOpenSmall.png", "SearchFolderOpenMedium.png", "SearchFolderOpenLarge.png");
            this.ClosedIconSet = new IconSet(IconScheme.Colour, "SearchFolderClosedSmall.png", "SearchFolderClosedMedium.png", "SearchFolderClosedLarge.png");
            if (this.IsOpen)
                this.IconSet = this.OpenIconSet;
            else
                this.IconSet = this.ClosedIconSet;

            this.RefreshTime = 0;
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

        protected override IList<ModalityWorklistItem> QueryItems()
        {
            List<ModalityWorklistItem> worklistItems = null;
            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    SearchRequest request = new SearchRequest();
                    request.TextQuery = this.SearchData.TextSearch;
                    request.ShowActiveOnly = this.SearchData.ShowActiveOnly;
                    TextQueryResponse<ModalityWorklistItem> response = service.Search(request);
                    worklistItems = response.Matches;
                });

            if (worklistItems == null)
                worklistItems = new List<ModalityWorklistItem>();

            return worklistItems;
        }

        protected override int QueryCount()
        {
            return this.ItemCount;
        }

    }
}
