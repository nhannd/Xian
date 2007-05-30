using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    public interface IRegistrationWorkflowFolderDropContext : IDropContext
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
        RegistrationWorkflowFolder DropTargetFolder { get; }

        /// <summary>
        /// Gets the folder system that owns the drop target folder
        /// </summary>
        RegistrationWorkflowFolderSystem FolderSystem { get; }
    }

    public abstract class RegistrationWorkflowFolder : WorkflowFolder<RegistrationWorklistItem>
    {
        class DropContext : IRegistrationWorkflowFolderDropContext
        {
            private RegistrationWorkflowFolder _folder;

            public DropContext(RegistrationWorkflowFolder folder)
            {
                _folder = folder;
            }

            #region IRegistrationWorkflowFolderDropContext Members

            public bool GetOperationEnablement(string operationName)
            {
                return _folder._folderSystem.GetOperationEnablement(operationName);
            }

            public RegistrationWorkflowFolder DropTargetFolder
            {
                get { return _folder; }
            }

            public RegistrationWorkflowFolderSystem FolderSystem
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


        private RegistrationWorkflowFolderSystem _folderSystem;
        private IconSet _closedIconSet;
        private IconSet _openIconSet;

        private string _worklistClassName;

        public RegistrationWorkflowFolder(RegistrationWorkflowFolderSystem folderSystem, string folderName, ExtensionPoint<IDropHandler<RegistrationWorklistItem>> dropHandlerExtensionPoint)
            : base(folderSystem, folderName, new RegistrationWorklistTable())
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
        }

        public RegistrationWorkflowFolder(RegistrationWorkflowFolderSystem folderSystem, string folderName)
            :this(folderSystem, folderName, null)
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

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            List<RegistrationWorklistItem> worklistItems = null;
            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    GetWorklistResponse response = service.GetWorklist(new GetWorklistRequest(this.WorklistClassName));
                    worklistItems = response.WorklistItems;
                });

            if (worklistItems == null)
                worklistItems = new List<RegistrationWorklistItem>();

            return worklistItems;
        }

        protected override int QueryCount()
        {
            int count = -1;
            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    GetWorklistCountResponse response = service.GetWorklistCount(new GetWorklistCountRequest(this.WorklistClassName));
                    count = response.ItemCount;
                });

            return count;
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            throw new NotImplementedException();
        }

        public bool GetOperationEnablement(string operationName)
        {
            return _folderSystem.GetOperationEnablement(operationName);
        }
   }
}