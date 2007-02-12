using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
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

        ICollection<RegistrationWorklistItem> SelectedItems { get; }
        event EventHandler SelectedItemsChanged;

        IDesktopWindow DesktopWindow { get; }
    }

    public interface IRegistrationWorkflowFolderToolContext : IToolContext
    {
        event EventHandler SelectedFolderChanged;
        IDesktopWindow DesktopWindow { get; }
    }

    public class RegistrationWorkflowFolderSystem : WorkflowFolderSystem<RegistrationWorklistItem>
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

            public ICollection<RegistrationWorklistItem> SelectedItems
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

        private IRegistrationWorkflowService _workflowService;
        private ToolSet _itemToolSet;
        private ToolSet _folderToolSet;
        private IDictionary<string, bool> _workflowEnablment;

        public RegistrationWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
            :base(folderExplorer)
        {
            // important to initialize service before adding any folders, because folders may access service
            _workflowService = ApplicationContext.GetService<IRegistrationWorkflowService>();
            _workflowService.ModalityProcedureStepChanged += ModalityProcedureStepChangedEventHandler;

            this.SelectedItemsChanged += SelectedItemsChangedEventHandler;

            this.AddFolder(new Folders.ScheduledFolder(this));
            this.AddFolder(new Folders.CheckedInFolder(this));
            this.AddFolder(new Folders.InProgressFolder(this));
            this.AddFolder(new Folders.CompletedFolder(this));
            this.AddFolder(new Folders.CancelledFolder(this));

            _itemToolSet = new ToolSet(new RegistrationWorkflowItemToolExtensionPoint(), new RegistrationWorkflowItemToolContext(this));
            _folderToolSet = new ToolSet(new RegistrationWorkflowFolderToolExtensionPoint(), new RegistrationWorkflowFolderToolContext(this));

            folderExplorer.AddItemActions(_itemToolSet.Actions);
            folderExplorer.AddFolderActions(_folderToolSet.Actions);
        }

        private void SelectedItemsChangedEventHandler(object sender, EventArgs e)
        {
            //TODO:

            //// update workflow enablement
            //RegistrationWorklistItem selectedItem = CollectionUtils.FirstElement<RegistrationWorklistItem>(this.SelectedItems);
            //if (selectedItem != null)
            //{
            //    _workflowEnablment = _workflowService.GetOperationEnablement(selectedItem.ProcedureStep);
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
            //RegistrationWorklistItem worklistItem = _workflowService.GetWorklistItem(mpsRef);

            //// force all folders to update this item
            //foreach (RegistrationWorkflowFolder folder in this.Folders)
            //{
            //    folder.UpdateWorklistItem(worklistItem);
            //}
        }

        private void ExecuteOperation(string operationName)
        {
            //TODO:

            //RegistrationWorklistItem selectedItem = CollectionUtils.FirstElement<RegistrationWorklistItem>(this.SelectedItems);
            //_workflowService.ExecuteOperation(selectedItem.ProcedureStep, operationName);
        }

        private bool GetOperationEnablement(string operationName)
        {
            return _workflowEnablment == null ? false : _workflowEnablment[operationName];
        }

        public IRegistrationWorkflowService WorkflowService
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
