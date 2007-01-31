using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Modality
{

    public interface IModalityWorkflowToolContext : IToolContext
    {
        //bool GetWorkflowOperationEnablement(string operationClass);
        //void ExecuteWorkflowOperation(string operationClass);

        //ICollection<ModalityWorklistQueryResult> SelectedItems { get; }
        //event EventHandler SelectedItemsChanged;
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
        }



        private IAcquisitionWorkflowService _workflowService;
        private ToolSet _toolSet;

        public ModalityWorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
            :base(folderExplorer)
        {
            // important to initialize service before adding any folders, because folders may access service
            _workflowService = ApplicationContext.GetService<IAcquisitionWorkflowService>();
            _workflowService.ModalityProcedureStepChanged += ModalityProcedureStepChangedEventHandler;

            this.AddFolder(new Folders.ScheduledFolder(this));
            this.AddFolder(new Folders.InProgressFolder(this));
            this.AddFolder(new Folders.CompletedFolder(this));
            this.AddFolder(new Folders.CancelledFolder(this));

            _toolSet = new ToolSet(new ModalityWorkflowToolExtensionPoint(), new ModalityWorkflowToolContext(this));
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

        public IAcquisitionWorkflowService WorkflowService
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
