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

        protected abstract bool Execute(ModalityWorklistItem item, IEnumerable folders);

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
                if (Execute(item, folders))
                {
                    selectedFolder.Refresh();
                    return true;
                }
            }
            catch(Exception e)
            {
                ExceptionHandler.Report(e, _errorMessage, desktopWindow);
            }
            return false;
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Start", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Start", "Apply")]
	[IconSet("apply", IconScheme.Colour, "StartToolSmall.png", "StartToolMedium.png", "StartToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    //[ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    //[ExtensionOf(typeof(Folders.InProgressTechnologistWorkflowFolder.DropHandlerExtensionPoint))]
    public class StartTool : TechnologistWorkflowTool
    {
        public StartTool()
            : base("StartProcedure", SR.ExceptionTechnologistWorkflowStartTool)
        {
        }

        protected override bool Execute(ModalityWorklistItem item, IEnumerable folders)
        {
            try
            {
                Platform.GetService<IModalityWorkflowService>(
                    delegate(IModalityWorkflowService service)
                    {
                        service.StartProcedure(new StartProcedureRequest(item.ProcedureStepRef));
                    });

                IFolder folder = CollectionUtils.SelectFirst<IFolder>(folders,
                    delegate(IFolder f) { return f is Folders.InProgressTechnologistWorkflowFolder; });
                folder.RefreshCount();
                return true;

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
                return false;
            }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Complete", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Complete", "Apply")]
	[IconSet("apply", IconScheme.Colour, "CompleteToolSmall.png", "CompleteToolMedium.png", "CompleteToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    //[ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    //[ExtensionOf(typeof(Folders.CompletedTechnologistWorkflowFolder.DropHandlerExtensionPoint))]
    public class CompleteTool : TechnologistWorkflowTool
    {
        public CompleteTool()
            : base("CompleteProcedure", SR.ExceptionTechnologistWorkflowCompleteTool)
        {
        }

        protected override bool Execute(ModalityWorklistItem item, IEnumerable folders)
        {
            try
            {
                Platform.GetService<IModalityWorkflowService>(
                    delegate(IModalityWorkflowService service)
                    {
                        service.CompleteProcedure(new CompleteProcedureRequest(item.ProcedureStepRef));
                    });

                IFolder folder = CollectionUtils.SelectFirst<IFolder>(folders,
                    delegate(IFolder f) { return f is Folders.InProgressTechnologistWorkflowFolder; });
                folder.RefreshCount();
                return true;

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
                return false;
            }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Cancel", "Apply")]
	[IconSet("apply", IconScheme.Colour, "DeleteToolSmall.png", "DeleteToolMedium.png", "DeleteToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    //[ExtensionOf(typeof(TechnologistWorkflowItemToolExtensionPoint))]
    //[ExtensionOf(typeof(Folders.CancelledTechnologistWorkflowFolder.DropHandlerExtensionPoint))]
    public class CancelTool : TechnologistWorkflowTool
    {
        public CancelTool()
            : base("CancelProcedure", SR.ExceptionTechnologistWorkflowCancelTool)
        {
        }

        protected override bool Execute(ModalityWorklistItem item, IEnumerable folders)
        {
            try
            {
                Platform.GetService<IModalityWorkflowService>(
                    delegate(IModalityWorkflowService service)
                    {
                        service.CancelProcedure(new CancelProcedureRequest(item.ProcedureStepRef));
                    });

                IFolder folder = CollectionUtils.SelectFirst<IFolder>(folders,
                    delegate(IFolder f) { return f is Folders.InProgressTechnologistWorkflowFolder; });
                folder.RefreshCount();
                return true;

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
                return false;
            }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Replace Order", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Replace Order", "Apply")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ExtensionOf(typeof(TechnologistMainWorkflowItemToolExtensionPoint))]
    public class TechnologistReplaceOrderTool : TechnologistWorkflowTool
    {
        public TechnologistReplaceOrderTool()
            : base("", SR.ExceptionTechnologistWorkflowReplaceOrderTool)
        {
        }

        public override bool Enabled
        {
            get
            {
                //TODO: this is a hack, since this tool isn't really a workflow tool it doesn't have a corresponding operation name
                return this.Context.SelectedItems.Count > 0;
            }
        }

        protected override bool Execute(ModalityWorklistItem item, IEnumerable folders)
        {
            try
            {
                ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    new OrderEntryComponent(item.PatientRef, item.OrderRef, OrderEntryComponent.Mode.ReplaceOrder),
                    string.Format(SR.TitleNewOrder, PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn)),
                    delegate
                    {
                        IFolder folder = CollectionUtils.SelectFirst<IFolder>(folders,
                            delegate(IFolder f) { return f is Folders.CancelledTechnologistWorkflowFolder; });
                        folder.RefreshCount();

                        folder = CollectionUtils.SelectFirst<IFolder>(folders,
                            delegate(IFolder f) { return f is Folders.ScheduledTechnologistWorkflowFolder; });
                        folder.RefreshCount();
                    });

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }

            // return false, because we don't need to update the selected folder until the workspace closes
            return false;
        }
    }
}
