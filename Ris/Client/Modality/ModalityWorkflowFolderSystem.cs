using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Modality
{

    public interface IModalityWorkflowToolContext : IToolContext
    {
        bool GetWorkflowOperationEnablement(string operationClass);
        void ExecuteWorkflowOperation(string operationClass);

        ICollection<ModalityWorklistQueryResult> SelectedItems { get; }
        event EventHandler SelectedItemsChanged;
    }

    [ExtensionPoint]
    public class ModalityWorkflowToolExtensionPoint : ExtensionPoint<ITool>
    {
    }


    public class ModalityWorkflowFolderSystem : WorkflowFolderSystem<ModalityWorklistQueryResult>
    {
        class ModalityWorkflowToolContext : ToolContext, IModalityWorkflowToolContext
        {
            private ModalityWorkflowFolderSystem _owner;

            public ModalityWorkflowToolContext(ModalityWorkflowFolderSystem owner)
            {
                _owner = owner;
            }

            #region IModalityWorkflowToolContext Members

            public ICollection<ModalityWorklistQueryResult> SelectedItems
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



        private IModalityWorkflowService _workflowService;
        private ToolSet _itemToolSet;
        private IDictionary<string, bool> _workflowEnablment;

        public ModalityWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
            :base(folderExplorer)
        {
            // important to initialize service before adding any folders, because folders may access service
            _workflowService = ApplicationContext.GetService<IModalityWorkflowService>();
            _workflowService.ModalityProcedureStepChanged += ModalityProcedureStepChangedEventHandler;

            this.SelectedItemsChanged += SelectedItemsChangedEventHandler;

            this.AddFolder(new Folders.ScheduledFolder(this));
            this.AddFolder(new Folders.InProgressFolder(this));
            this.AddFolder(new Folders.CompletedFolder(this));
            this.AddFolder(new Folders.CancelledFolder(this));

            _itemToolSet = new ToolSet(new ModalityWorkflowToolExtensionPoint(), new ModalityWorkflowToolContext(this));

            folderExplorer.AddItemActions(_itemToolSet.Actions);
        }

        private void SelectedItemsChangedEventHandler(object sender, EventArgs e)
        {
            // update workflow enablement
            ModalityWorklistQueryResult selectedItem = CollectionUtils.FirstElement<ModalityWorklistQueryResult>(this.SelectedItems);
            if (selectedItem != null)
            {
                _workflowEnablment = _workflowService.GetOperationEnablement(selectedItem.ProcedureStep);
            }
            else
            {
                _workflowEnablment = null;
            }
        }

        private void ModalityProcedureStepChangedEventHandler(object sender, EntityChangeEventArgs e)
        {
            // this should never happen
            if (e.ChangeType == EntityChangeType.Delete)
                throw new NotSupportedException("Unexpected deletion of modality procedure step");

            EntityRef<ModalityProcedureStep> mpsRef = (EntityRef<ModalityProcedureStep>)e.EntityRef;

            // retrieve the new or updated worklist item
            ModalityWorklistQueryResult worklistItem = _workflowService.GetWorklistItem(mpsRef);

            // force all folders to update this item
            foreach (ModalityWorkflowFolder folder in this.Folders)
            {
                folder.UpdateWorklistItem(worklistItem);
            }
        }

        private void ExecuteOperation(string operationName)
        {
            ModalityWorklistQueryResult selectedItem = CollectionUtils.FirstElement<ModalityWorklistQueryResult>(this.SelectedItems);
            _workflowService.ExecuteOperation(selectedItem.ProcedureStep, operationName);
        }

        private bool GetOperationEnablement(string operationName)
        {
            return _workflowEnablment == null ? false : _workflowEnablment[operationName];
        }

        public IModalityWorkflowService WorkflowService
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
            }
        }

    }
}
