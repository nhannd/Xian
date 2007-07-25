using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    public interface IReportingWorkflowFolderDropContext : IDropContext
    {
        /// <summary>
        /// Gets the enablement of the specified operation from the folder system
        /// </summary>
        /// <param name="operationName"></param>
        /// <returns></returns>
        bool GetOperationEnablement(string operationName);

        /// <summary>
        /// Gets the folder that is the drop target of the current operation
        /// </summary>
        ReportingWorkflowFolder DropTargetFolder { get; }

        /// <summary>
        /// Gets the folder system that owns the drop target folder
        /// </summary>
        ReportingWorkflowFolderSystem FolderSystem { get; }
    }

    public abstract class ReportingWorkflowFolder : WorkflowFolder<ReportingWorklistItem>
    {
        class DropContext : IReportingWorkflowFolderDropContext
        {
            private ReportingWorkflowFolder _folder;

            public DropContext(ReportingWorkflowFolder folder)
            {
                _folder = folder;
            }

            #region IReportingWorkflowFolderDropContext Members

            public bool GetOperationEnablement(string operationName)
            {
                return _folder._folderSystem.GetOperationEnablement(operationName);
            }

            public ReportingWorkflowFolder DropTargetFolder
            {
                get { return _folder; }
            }

            public ReportingWorkflowFolderSystem FolderSystem
            {
                get
                {
                    return _folder._folderSystem;
                }
            }

            #endregion

            #region IDropContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _folder._folderSystem.DesktopWindow; }
            }

            #endregion
        }

        private readonly ReportingWorkflowFolderSystem _folderSystem;
        private IconSet _closedIconSet;
        private IconSet _openIconSet;

        private readonly EntityRef _worklistRef;
        private string _worklistClassName;

        public ReportingWorkflowFolder(ReportingWorkflowFolderSystem folderSystem, string folderName, EntityRef worklistRef, ExtensionPoint<IDropHandler<ReportingWorklistItem>> dropHandlerExtensionPoint)
            :base(folderSystem, folderName, new ReportingWorklistTable())
        {
            _folderSystem = folderSystem;

            _closedIconSet = new IconSet(IconScheme.Colour, "FolderClosedSmall.png", "FolderClosedMedium.png", "FolderClosedMedium.png");
            _openIconSet = new IconSet(IconScheme.Colour, "FolderOpenSmall.png", "FolderOpenMedium.png", "FolderOpenMedium.png");
            this.IconSet = _closedIconSet;
            this.ResourceResolver = new ResourceResolver(this.GetType().Assembly);
            if (dropHandlerExtensionPoint != null)
            {
                this.InitDragDropHandling(dropHandlerExtensionPoint, new DropContext(this));
            }

            _worklistRef = worklistRef;
        }

        public ReportingWorkflowFolder(ReportingWorkflowFolderSystem folderSystem, string folderName, ExtensionPoint<IDropHandler<ReportingWorklistItem>> dropHandlerExtensionPoint)
            : this(folderSystem, folderName, null, dropHandlerExtensionPoint)
        {
        }

        public ReportingWorkflowFolder(ReportingWorkflowFolderSystem folderSystem, string folderName)
            : this(folderSystem, folderName, null, null)
        {
        }

        public ReportingWorkflowFolder(ReportingWorkflowFolderSystem folderSystem, string folderName, EntityRef worklistRef)
            : this(folderSystem, folderName, worklistRef, null)
        {
        }

        public string WorklistClassName
        {
            get { return _worklistClassName; }
            set { _worklistClassName = value; }
        }

        public IconSet ClosedIconSet
        {
            get { return _closedIconSet; }
            set { _closedIconSet = value; }
        }

        public IconSet OpenIconSet
        {
            get { return _openIconSet; }
            set { _openIconSet = value; }
        }

        public override void OpenFolder()
        {
            if (_openIconSet != null)
                this.IconSet = _openIconSet;

            base.OpenFolder();
        }

        public override void CloseFolder()
        {
            if (_closedIconSet != null)
                this.IconSet = _closedIconSet;

            base.CloseFolder();
        }

        protected override bool CanQuery()
        {
            return true;
        }

        protected override IList<ReportingWorklistItem> QueryItems()
        {
            List<ReportingWorklistItem> worklistItems = null;
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    GetWorklistRequest request = _worklistRef == null
                        ? new GetWorklistRequest(this.WorklistClassName)
                        : new GetWorklistRequest(_worklistRef);

                    GetWorklistResponse response = service.GetWorklist(request);
                    worklistItems = response.WorklistItems;
                });

            if (worklistItems == null)
                worklistItems = new List<ReportingWorklistItem>();

            return worklistItems;
        }

        protected override int QueryCount()
        {
            int count = -1;
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    GetWorklistCountRequest request = _worklistRef == null
                        ? new GetWorklistCountRequest(this.WorklistClassName)
                        : new GetWorklistCountRequest(_worklistRef);

                    GetWorklistCountResponse response = service.GetWorklistCount(request);
                    count = response.ItemCount;
                });

            return count;
        }

        protected override bool IsMember(ReportingWorklistItem item)
        {
            throw new NotImplementedException();
        }

        public bool GetOperationEnablement(string operationName)
        {
            return _folderSystem.GetOperationEnablement(operationName);
        }
    }
}
