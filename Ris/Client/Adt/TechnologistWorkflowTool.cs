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
using System.Collections;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    public abstract class TechnologistWorkflowTool : Tool<ITechnologistWorkflowItemToolContext>, IDropHandler<ModalityWorklistItem>
    {
        protected string _operationName;
        protected string _errorMessage;

        public TechnologistWorkflowTool(string operationName)
            : this(operationName, null)
        {
        }

        public TechnologistWorkflowTool(string operationName, string errorMessage)
        {
            _operationName = operationName;
            _errorMessage = errorMessage;
        }

        public virtual bool Enabled
        {
            get
            {
                return this.Context.GetWorkflowOperationEnablement(_operationName);
            }
        }

        public virtual event EventHandler EnabledChanged
        {
            add { this.Context.SelectedItemsChanged += value; }
            remove { this.Context.SelectedItemsChanged -= value; }
        }

        public virtual void Apply()
        {
            ExecuteAndRefresh(this.Context.DesktopWindow, this.Context.SelectedFolder, this.Context.Folders, this.Context.SelectedItems);
        }

        public string OperationName
        {
            get { return _operationName; }
        }

        protected abstract void Execute(ModalityWorklistItem item, IEnumerable folders);

        #region IDropHandler<ModalityWorklistItem> Members

        public virtual bool CanAcceptDrop(IDropContext dropContext, ICollection<ModalityWorklistItem> items)
        {
            ITechnologistWorkflowFolderDropContext ctxt = (ITechnologistWorkflowFolderDropContext)dropContext;
            return ctxt.GetOperationEnablement(this.OperationName);
        }

        public virtual bool ProcessDrop(IDropContext dropContext, ICollection<ModalityWorklistItem> items)
        {
            ITechnologistWorkflowFolderDropContext ctxt = (ITechnologistWorkflowFolderDropContext)dropContext;
            return ExecuteAndRefresh(ctxt.DesktopWindow, ctxt.FolderSystem.SelectedFolder, ctxt.FolderSystem.Folders, items);
        }

        #endregion

        private bool ExecuteAndRefresh(IDesktopWindow desktopWindow, IFolder selectedFolder, IEnumerable folders, IEnumerable items)
        {
            try
            {
                ModalityWorklistItem item = CollectionUtils.FirstElement<ModalityWorklistItem>(items);
                Execute(item, folders);
                selectedFolder.Refresh();
                return true;
            }
            catch(Exception e)
            {
                ExceptionHandler.Report(e, _errorMessage, desktopWindow);
                return false;                            
            }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Start")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Start")]
	[IconSet("apply", IconScheme.Colour, "StartToolSmall.png", "StartToolMedium.png", "StartToolLarge.png")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    //[ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    //[ExtensionOf(typeof(Folders.InProgressTechnologistWorkflowFolder.DropHandlerExtensionPoint))]
    public class StartTool : TechnologistWorkflowTool
    {
        public StartTool()
            : base("StartProcedure", SR.ExceptionTechnologistWorkflowStartTool)
        {
        }

        protected override void Execute(ModalityWorklistItem item, IEnumerable folders)
        {
            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    service.StartProcedure(new StartProcedureRequest(item.ProcedureStepRef));
                });

            IFolder folder = CollectionUtils.SelectFirst<IFolder>(folders,
                delegate(IFolder f) { return f is Folders.InProgressTechnologistWorkflowFolder; });
            folder.RefreshCount();
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Complete")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Complete")]
	[IconSet("apply", IconScheme.Colour, "CompleteToolSmall.png", "CompleteToolMedium.png", "CompleteToolLarge.png")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    //[ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    //[ExtensionOf(typeof(Folders.CompletedTechnologistWorkflowFolder.DropHandlerExtensionPoint))]
    public class CompleteTool : TechnologistWorkflowTool
    {
        public CompleteTool()
            : base("CompleteProcedure", SR.ExceptionTechnologistWorkflowCompleteTool)
        {
        }

        protected override void Execute(ModalityWorklistItem item, IEnumerable folders)
        {
            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    service.CompleteProcedure(new CompleteProcedureRequest(item.ProcedureStepRef));
                });

            IFolder folder = CollectionUtils.SelectFirst<IFolder>(folders,
                delegate(IFolder f) { return f is Folders.CompletedTechnologistWorkflowFolder; });
            folder.RefreshCount();
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Cancel")]
	[IconSet("apply", IconScheme.Colour, "DeleteToolSmall.png", "DeleteToolMedium.png", "DeleteToolLarge.png")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    //[ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    //[ExtensionOf(typeof(Folders.CancelledTechnologistWorkflowFolder.DropHandlerExtensionPoint))]
    public class CancelTool : TechnologistWorkflowTool
    {
        public CancelTool()
            : base("CancelProcedure", SR.ExceptionTechnologistWorkflowCancelTool)
        {
        }

        protected override void Execute(ModalityWorklistItem item, IEnumerable folders)
        {
            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    service.CancelProcedure(new CancelProcedureRequest(item.ProcedureStepRef));
                });

            IFolder folder = CollectionUtils.SelectFirst<IFolder>(folders,
                delegate(IFolder f) { return f is Folders.CancelledTechnologistWorkflowFolder; });
            folder.RefreshCount();
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Replace Order")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Replace Order")]
    [ClickHandler("apply", "Apply")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    public class TechnologistReplaceOrderTool : TechnologistWorkflowTool
    {
        public TechnologistReplaceOrderTool()
            : base("ReplaceOrder", SR.ExceptionTechnologistWorkflowReplaceOrderTool)
        {
        }

        protected override void Execute(ModalityWorklistItem item, IEnumerable folders)
        {
            OrderDetail existingOrder = null;

            try
            {
                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                        {
                            LoadOrderDetailResponse response = service.LoadOrderDetail(new LoadOrderDetailRequest(item.AccessionNumber));
                            existingOrder = response.OrderDetail;
                        });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }

            if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageReplaceOrder, MessageBoxActions.OkCancel) == DialogBoxAction.Ok)
            {
                ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    new OrderEntryComponent(existingOrder),
                    string.Format(SR.TitleNewOrder, PersonNameFormat.Format(item.PersonNameDetail), MrnFormat.Format(item.Mrn)),
                    delegate(IApplicationComponent c)
                    {
                        if (c.ExitCode == ApplicationComponentExitCode.Normal)
                        {
                            try
                            {
                                OrderEntryComponent component = (OrderEntryComponent)c;

                                Platform.GetService<IModalityWorkflowService>(
                                    delegate(IModalityWorkflowService service)
                                    {
                                        service.ReplaceOrder(new ReplaceOrderRequest(component.PlaceOrderRequest, component.CancelOrderRequest));
                                    });

                                IFolder folder = CollectionUtils.SelectFirst<IFolder>(folders,
                                    delegate(IFolder f) { return f is Folders.CancelledTechnologistWorkflowFolder; });
                                folder.RefreshCount();
                            }
                            catch (Exception e)
                            {
                                ExceptionHandler.Report(e, SR.ExceptionCannotReplaceOrder, this.Context.DesktopWindow);
                            }
                        }
                    });
            }
        }
    }
}
