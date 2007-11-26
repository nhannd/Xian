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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    public class RegistrationWorkflowTool
    {
        public abstract class WorkflowItemTool : Tool<IRegistrationWorkflowItemToolContext>, IDropHandler<RegistrationWorklistItem>
        {
            protected string _operationName;

            public WorkflowItemTool(string operationName)
            {
                _operationName = operationName;
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
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(this.Context.SelectedItems);
                bool success = Execute(item, this.Context.DesktopWindow, this.Context.Folders);
                if (success)
                {
                    this.Context.SelectedFolder.Refresh();
                }
            }

            protected string OperationName
            {
                get { return _operationName; }
            }

            protected abstract bool Execute(RegistrationWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders);

            #region IDropHandler<RegistrationWorklistItem> Members

            public virtual bool CanAcceptDrop(IDropContext dropContext, ICollection<RegistrationWorklistItem> items)
            {
                IRegistrationWorkflowFolderDropContext ctxt = (IRegistrationWorkflowFolderDropContext)dropContext;
                return ctxt.GetOperationEnablement(this.OperationName);
            }

            public virtual bool ProcessDrop(IDropContext dropContext, ICollection<RegistrationWorklistItem> items)
            {
                IRegistrationWorkflowFolderDropContext ctxt = (IRegistrationWorkflowFolderDropContext)dropContext;
                RegistrationWorklistItem item = CollectionUtils.FirstElement<RegistrationWorklistItem>(items);
                bool success = Execute(item, ctxt.DesktopWindow, ctxt.FolderSystem.Folders);
                if (success)
                {
                    ctxt.FolderSystem.SelectedFolder.Refresh();
                    return true;
                }
                return false;
            }

            #endregion
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Check-in")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Check-in")]
        [ClickHandler("apply", "Apply")]
		[IconSet("apply", IconScheme.Colour, "Icons.CheckInToolSmall.png", "Icons.CheckInToolMedium.png", "Icons.CheckInToolLarge.png")]
		[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
        [ExtensionOf(typeof(Folders.CheckedInFolder.DropHandlerExtensionPoint))]
        public class CheckInTool : WorkflowItemTool
        {
            public CheckInTool()
                : base("CheckInProcedure")
            {
            }

            protected override bool Execute(RegistrationWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
            {
                try
                {
                    CheckInOrderComponent checkInComponent = new CheckInOrderComponent(item); 
                    ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                        desktopWindow,
                        checkInComponent,
                        String.Format("Checking in {0}", PersonNameFormat.Format(item.PatientName)));

                    if (exitCode == ApplicationComponentExitCode.Accepted)
                    {
                        Platform.GetService<IRegistrationWorkflowService>(
                            delegate(IRegistrationWorkflowService service)
                            {
                                service.CheckInProcedure(new CheckInProcedureRequest(checkInComponent.SelectedRequestedProcedures));
                            });

                        IFolder checkInFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                            delegate(IFolder f) { return f is Folders.CheckedInFolder; });
                        checkInFolder.RefreshCount();

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, desktopWindow);
                    return false;
                }
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel Order")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Cancel Order")]
        [ClickHandler("apply", "Apply")]
		[IconSet("apply", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
		[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
        [ExtensionOf(typeof(Folders.CancelledFolder.DropHandlerExtensionPoint))]
        public class CancelTool : WorkflowItemTool, IDropHandler<RegistrationWorklistItem>
        {
            public CancelTool()
                : base("CancelOrder")
            {
            }

            protected override bool Execute(RegistrationWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
            {
                try
                {
                    CancelOrderComponent cancelOrderComponent = new CancelOrderComponent();
                    ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                        desktopWindow,
                        cancelOrderComponent,
                        String.Format(SR.TitleCancelOrder, PersonNameFormat.Format(item.PatientName)));

                    if (exitCode == ApplicationComponentExitCode.Accepted)
                    {
                        Platform.GetService<IRegistrationWorkflowService>(
                            delegate(IRegistrationWorkflowService service)
                            {
                                service.CancelOrder(new CancelOrderRequest(item.OrderRef, cancelOrderComponent.SelectedCancelReason));
                            });

                        IFolder cancelledFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                           delegate(IFolder f) { return f is Folders.CancelledFolder; });
                        cancelledFolder.RefreshCount();

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, desktopWindow);
                    return false;
                }
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Replace Order")]
        [ButtonAction("apply", "folderexplorer-items-toolbar/Replace Order")]
        [ClickHandler("apply", "Apply")]
        [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
        public class RegistrationReplaceOrderTool : WorkflowItemTool
        {
            public RegistrationReplaceOrderTool()
                : base("")
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

            protected override bool Execute(RegistrationWorklistItem item, IDesktopWindow desktopWindow, IEnumerable folders)
            {
                try
                {
                    ApplicationComponent.LaunchAsWorkspace(
                        desktopWindow,
                        new OrderEntryComponent(item.PatientRef, item.OrderRef, OrderEntryComponent.Mode.ReplaceOrder),
                        string.Format(SR.TitleReplaceOrder, PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn)),
                        delegate(IApplicationComponent c)
                        {
                            if (c.ExitCode == ApplicationComponentExitCode.Accepted)
                            {
                                IFolder cancelledFolder = CollectionUtils.SelectFirst<IFolder>(folders,
                                   delegate(IFolder f) { return f is Folders.CancelledFolder; });

                                cancelledFolder.RefreshCount();
                            }
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
}

