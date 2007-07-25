using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    public interface ITechnologistWorkflowFolderDropContext : IDropContext
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
        TechnologistWorkflowFolderBase DropTargetFolder { get; }

        /// <summary>
        /// Gets the folder system that owns the drop target folder
        /// </summary>
        TechnologistWorkflowFolderSystem FolderSystem { get; }
    }

    public abstract class TechnologistWorkflowFolderBase : WorkflowFolder<ModalityWorklistItem>
    {
        class DropContext : ITechnologistWorkflowFolderDropContext
        {
            private TechnologistWorkflowFolderBase _folder;

            public DropContext(TechnologistWorkflowFolderBase folder)
            {
                _folder = folder;
            }

            #region ITechnologistWorkflowFolderDropContextMembers

            public bool GetOperationEnablement(string operationName)
            {
                return _folder._folderSystem.GetOperationEnablement(operationName);
            }

            public TechnologistWorkflowFolderBase DropTargetFolder
            {
                get { return _folder; }
            }

            public TechnologistWorkflowFolderSystem FolderSystem
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

        private TechnologistWorkflowFolderSystem _folderSystem;
        private IconSet _closedIconSet;
        private IconSet _openIconSet;

        private readonly EntityRef _worklistRef;
        private string _worklistClassName;

        public TechnologistWorkflowFolderBase(TechnologistWorkflowFolderSystem folderSystem, string folderName, string folderDescription, EntityRef worklistRef, ExtensionPoint<IDropHandler<ModalityWorklistItem>> dropHandlerExtensionPoint)
            : base(folderSystem, folderName, folderDescription, new ModalityWorklistTable())
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

        public TechnologistWorkflowFolderBase(TechnologistWorkflowFolderSystem folderSystem, string folderName, ExtensionPoint<IDropHandler<ModalityWorklistItem>> dropHandlerExtensionPoint)
            : this(folderSystem, folderName, null, null, dropHandlerExtensionPoint)
        {
        }

        public TechnologistWorkflowFolderBase(TechnologistWorkflowFolderSystem folderSystem, string folderName)
            : this(folderSystem, folderName, null, null, null)
        {
        }

        public TechnologistWorkflowFolderBase(TechnologistWorkflowFolderSystem folderSystem, string folderName, string folderDescription, EntityRef worklistRef)
            : this(folderSystem, folderName, folderDescription, worklistRef, null)
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

        protected override int QueryCount()
        {
            int count = -1;

            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    GetWorklistCountRequest request = _worklistRef == null 
                        ? new GetWorklistCountRequest(this.WorklistClassName)
                        : new GetWorklistCountRequest(_worklistRef);

                    GetWorklistCountResponse response = service.GetWorklistCount(request);
                    count = response.ItemCount;
                });

            return count;
        }

        protected override IList<ModalityWorklistItem> QueryItems()
        {
            List<ModalityWorklistItem> worklistItems = null;

            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    GetWorklistRequest request = _worklistRef == null 
                        ? new GetWorklistRequest(this.WorklistClassName)
                        : new GetWorklistRequest(_worklistRef);

                    GetWorklistResponse response = service.GetWorklist(request);
                    worklistItems = response.WorklistItems;
                });

            return worklistItems ?? new List<ModalityWorklistItem>();
        }

        //protected override bool CanAcceptDrop(ModalityWorklistItem item)
        //{
        //    return false;
        //}

        //protected override bool ConfirmAcceptDrop(ICollection<ModalityWorklistItem> items)
        //{
        //    return true;
        //}

        //protected override bool ProcessDrop(ModalityWorklistItem item)
        //{
        //    return false;
        //}

        protected override bool IsMember(ModalityWorklistItem item)
        {
            return true;
        }
    }
}
