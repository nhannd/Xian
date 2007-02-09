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
    public class RegistrationWorkflowToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IRegistrationWorkflowToolContext : IToolContext
    {
        bool GetWorkflowOperationEnablement(string operationClass);
        void ExecuteWorkflowOperation(string operationClass);

        ICollection<RegistrationWorklistItem> SelectedItems { get; }
        event EventHandler SelectedItemsChanged;

        IDesktopWindow DesktopWindow { get; }
    }

    public class RegistrationWorkflowFolderSystem : WorkflowFolderSystem<RegistrationWorklistItem>
    {
        class RegistrationWorkflowToolContext : ToolContext, IRegistrationWorkflowToolContext
        {
            private RegistrationWorkflowFolderSystem _owner;

            public RegistrationWorkflowToolContext(RegistrationWorkflowFolderSystem owner)
            {
                _owner = owner;
            }

            #region IRegistrationWorkflowToolContext Members

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

        private IRegistrationWorkflowService _workflowService;
        private ToolSet _toolSet;
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

            _toolSet = new ToolSet(new RegistrationWorkflowToolExtensionPoint(), new RegistrationWorkflowToolContext(this));
            
            folderExplorer.AddActions(_toolSet.Actions);
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

                if(_toolSet != null) _toolSet.Dispose();
            }
        }

    }
}
