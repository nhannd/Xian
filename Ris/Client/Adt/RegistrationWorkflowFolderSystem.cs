using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class RegistrationWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionPoint]
    public class RegistrationWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IRegistrationWorkflowItemToolContext : IToolContext
    {
        bool GetWorkflowOperationEnablement(string operationClass);
        void ExecuteWorkflowOperation(string operationClass);

        ICollection<WorklistItem> SelectedItems { get; }
        event EventHandler SelectedItemsChanged;

        IDesktopWindow DesktopWindow { get; }
    }

    public interface IRegistrationWorkflowFolderToolContext : IToolContext
    {
        SearchCriteria SearchCriteria { set; }

        event EventHandler SelectedFolderChanged;
        IDesktopWindow DesktopWindow { get; }
    }

    public class RegistrationWorkflowFolderSystem : WorkflowFolderSystem<WorklistItem>
    {
        class RegistrationWorkflowItemToolContext : ToolContext, IRegistrationWorkflowItemToolContext
        {
            private RegistrationWorkflowFolderSystem _owner;

            public RegistrationWorkflowItemToolContext(RegistrationWorkflowFolderSystem owner)
            {
                _owner = owner;
            }

            #region IRegistrationWorkflowItemToolContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _owner.DesktopWindow; }
            }

            public ICollection<WorklistItem> SelectedItems
            {
                get { return _owner.SelectedItems; }
            }

            public event EventHandler SelectedItemsChanged
            {
                add { _owner.SelectedItemsChanged += value; }
                remove { _owner.SelectedItemsChanged -= value; }
            }

            public bool GetWorkflowOperationEnablement(string operationClass)
            {
                return _owner.GetOperationEnablement(operationClass);
            }

            public void ExecuteWorkflowOperation(string operationClass)
            {
                _owner.ExecuteOperation(operationClass);
            }

            #endregion
        }

        class RegistrationWorkflowFolderToolContext : ToolContext, IRegistrationWorkflowFolderToolContext
        {
            private RegistrationWorkflowFolderSystem _owner;

            public RegistrationWorkflowFolderToolContext(RegistrationWorkflowFolderSystem owner)
            {
                _owner = owner;
            }

            #region IRegistrationWorkflowItemToolContext Members

            public SearchCriteria SearchCriteria
            {
                set { _owner.SearchCriteria = value as PatientProfileSearchCriteria; }
            }

            public event EventHandler SelectedFolderChanged
            {
                add { _owner.SelectedItemsChanged += value; }
                remove { _owner.SelectedItemsChanged -= value; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _owner.DesktopWindow; }
            }

            #endregion
        }

        private IWorklistService _workflowService;
        private ToolSet _itemToolSet;
        private ToolSet _folderToolSet;
        private IDictionary<string, bool> _workflowEnablment;
        private Folders.SearchFolder _searchFolder;

        public SearchCriteria SearchCriteria
        {
            get { return (_searchFolder == null ? null : _searchFolder.SearchCriteria); }
            set 
            {
                _searchFolder.SearchCriteria = value;
                SelectedFolder = _searchFolder;
            }
        }

        public RegistrationWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
            :base(folderExplorer)
        {
            // important to initialize service before adding any folders, because folders may access service
            _workflowService = ApplicationContext.GetService<IWorklistService>();
            _workflowService.ModalityProcedureStepChanged += ModalityProcedureStepChangedEventHandler;

            this.SelectedItemsChanged += SelectedItemsChangedEventHandler;

            this.AddFolder(new Folders.ScheduledFolder(this));
            this.AddFolder(new Folders.CheckedInFolder(this));
            this.AddFolder(new Folders.InProgressFolder(this));
            this.AddFolder(new Folders.CompletedFolder(this));
            this.AddFolder(new Folders.CancelledFolder(this));
            this.AddFolder(_searchFolder = new Folders.SearchFolder(this));

            _itemToolSet = new ToolSet(new RegistrationWorkflowItemToolExtensionPoint(), new RegistrationWorkflowItemToolContext(this));
            _folderToolSet = new ToolSet(new RegistrationWorkflowFolderToolExtensionPoint(), new RegistrationWorkflowFolderToolContext(this));

            folderExplorer.AddItemActions(_itemToolSet.Actions);
            folderExplorer.AddFolderActions(_folderToolSet.Actions);
        }

        private void SelectedItemsChangedEventHandler(object sender, EventArgs e)
        {
            //TODO:

            //// update workflow enablement
            //WorklistItem selectedItem = CollectionUtils.FirstElement<WorklistItem>(this.SelectedItems);
            //if (selectedItem != null)
            //{
            //    WorklistQueryResult result = CollectionUtils.FirstElement<WorklistQueryResult>(selectedItem.QueryResults);
            //    _workflowEnablment = _workflowService.GetOperationEnablement(result.ProcedureStep);
            //}
            //else
            //{
            //    _workflowEnablment = null;
            //}
        }

        private void ModalityProcedureStepChangedEventHandler(object sender, EntityChangeEventArgs e)
        {
            // TODO:

            //// this should never happen
            //if (e.ChangeType == EntityChangeType.Delete)
            //    throw new NotSupportedException("Unexpected deletion of modality procedure step");

            //EntityRef<ModalityProcedureStep> mpsRef = (EntityRef<ModalityProcedureStep>)e.EntityRef;

            //// retrieve the new or updated worklist item
            //WorklistItem worklistItem = _workflowService.GetWorklistItem(mpsRef);

            //// force all folders to update this item
            //foreach (RegistrationWorkflowFolder folder in this.Folders)
            //{
            //    folder.UpdateWorklistItem(worklistItem);
            //}
        }

        private void ExecuteOperation(string operationName)
        {
            //TODO:

            //WorklistItem selectedItem = CollectionUtils.FirstElement<WorklistItem>(this.SelectedItems);
            //_workflowService.ExecuteOperation(selectedItem.ProcedureStep, operationName);
        }

        private bool GetOperationEnablement(string operationName)
        {
            return _workflowEnablment == null ? false : _workflowEnablment[operationName];
        }

        public IWorklistService WorkflowService
        {
            get { return _workflowService; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _workflowService.ModalityProcedureStepChanged -= ModalityProcedureStepChangedEventHandler;

                if(_itemToolSet != null) _itemToolSet.Dispose();
                if (_folderToolSet != null) _folderToolSet.Dispose();
            }
        }

    }
}
