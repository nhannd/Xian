#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Resolve Protocol - Resubmit", "ResolveProtocol")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", AuthorityTokens.Workflow.Protocol.Resolve)]
    [ExtensionOf(typeof(RegistrationBookingWorkflowItemToolExtensionPoint))]
    public class ResubmitProtocolTool : Tool<IRegistrationWorkflowItemToolContext>
    {
        public void ResolveProtocol()
        {
            RegistrationWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
            if (item != null)
            {
                try
                {
                    Platform.GetService<IProtocollingWorkflowService>(
                        delegate(IProtocollingWorkflowService service)
                        {
                            service.ResubmitProtocol(new ResubmitProtocolRequest(item.OrderRef));

                            IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;

                            IFolder suspendedProtocolFolder = CollectionUtils.SelectFirst<IFolder>(context.Folders,
                                delegate(IFolder f) { return f is Folders.SuspendedProtocolFolder; });

                            if (suspendedProtocolFolder != null)
                            {
                                if (suspendedProtocolFolder.IsOpen)
                                    suspendedProtocolFolder.Refresh();
                                else
                                    suspendedProtocolFolder.RefreshCount();
                            }
                        });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
        }

        public virtual bool Enabled
        {
            get
            {
                bool enabled = false;

                RegistrationWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
                if (item != null)
                {
                    try
                    {
                        Platform.GetService<IProtocollingWorkflowService>(
                            delegate(IProtocollingWorkflowService service)
                            {
                                GetClericalProtocolOperationEnablementResponse response =
                                service.GetClericalProtocolOperationEnablement(new GetClericalProtocolOperationEnablementRequest(item.OrderRef));

                                enabled = response.CanResolveByResubmit;
                            });
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, this.Context.DesktopWindow);
                    }
                }

                return enabled;
            }
        }

        public virtual event EventHandler EnabledChanged
        {
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }
    }

    [MenuAction("apply", "folderexplorer-items-contextmenu/Resolve Protocol - Cancel Order", "ResolveProtocol")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", AuthorityTokens.Workflow.Protocol.Resolve)]
	[ExtensionOf(typeof(RegistrationBookingWorkflowItemToolExtensionPoint))]
    public class CancelProtocolTool : Tool<IRegistrationWorkflowItemToolContext>
    {
        public void ResolveProtocol()
        {
            RegistrationWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
            if (item != null)
            {
                try
                {
                    Platform.GetService<IProtocollingWorkflowService>(
                        delegate(IProtocollingWorkflowService service)
                        {
                            service.CancelProtocolAndOrder(new CancelProtocolAndOrderRequest(item.OrderRef));

                            IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;

                            IFolder rejectedProtocolFolder = CollectionUtils.SelectFirst<IFolder>(context.Folders,
                                delegate(IFolder f) { return f is Folders.RejectedProtocolFolder; });

                            if (rejectedProtocolFolder != null)
                            {
                                if (rejectedProtocolFolder.IsOpen)
                                    rejectedProtocolFolder.Refresh();
                                else
                                    rejectedProtocolFolder.RefreshCount();
                            }
                        });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
        }

        public virtual bool Enabled
        {
            get
            {
                bool enabled = false;

                RegistrationWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
                if (item != null)
                {
                    try
                    {
                        Platform.GetService<IProtocollingWorkflowService>(
                            delegate(IProtocollingWorkflowService service)
                            {
                                GetClericalProtocolOperationEnablementResponse response =
                                service.GetClericalProtocolOperationEnablement(new GetClericalProtocolOperationEnablementRequest(item.OrderRef));

                                enabled = response.CanResolveByCancel;
                            });
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, this.Context.DesktopWindow);
                    }
                }

                return enabled;
            }
        }

        public virtual event EventHandler EnabledChanged
        {
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }
    }
}
