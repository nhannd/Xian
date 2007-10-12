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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
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

        ICollection<WorklistQueryResult> SelectedItems { get; }
        event EventHandler SelectedItemsChanged;
    }

    [ExtensionPoint]
    public class ModalityWorkflowToolExtensionPoint : ExtensionPoint<ITool>
    {
    }


    public class ModalityWorkflowFolderSystem : WorkflowFolderSystem<WorklistQueryResult>
    {
        class ModalityWorkflowToolContext : ToolContext, IModalityWorkflowToolContext
        {
            private ModalityWorkflowFolderSystem _owner;

            public ModalityWorkflowToolContext(ModalityWorkflowFolderSystem owner)
            {
                _owner = owner;
            }

            #region IModalityWorkflowToolContext Members

            public ICollection<WorklistQueryResult> SelectedItems
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
            WorklistQueryResult selectedItem = CollectionUtils.FirstElement<WorklistQueryResult>(this.SelectedItems);
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
            WorklistQueryResult worklistItem = _workflowService.GetWorklistItem(mpsRef);

            // force all folders to update this item
            foreach (ModalityWorkflowFolder folder in this.Folders)
            {
                folder.UpdateWorklistItem(worklistItem);
            }
        }

        private void ExecuteOperation(string operationName)
        {
            WorklistQueryResult selectedItem = CollectionUtils.FirstElement<WorklistQueryResult>(this.SelectedItems);
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
