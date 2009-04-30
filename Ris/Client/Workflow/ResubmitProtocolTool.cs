#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Re-submit for Protocol", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Re-submit for Protocol", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.ResubmitOrderSmall.png", "Icons.ResubmitOrderMedium.png", "Icons.ResubmitOrderLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.Resubmit)]
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	public class ResubmitProtocolTool : WorkflowItemTool<RegistrationWorklistItem, IWorkflowItemToolContext<RegistrationWorklistItem>>
	{
		public ResubmitProtocolTool()
			: base("ResubmitProtocol")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IProtocollingWorkflowService));
		}

		protected override bool Execute(RegistrationWorklistItem item)
		{
			Platform.GetService<IProtocollingWorkflowService>(
				delegate(IProtocollingWorkflowService service)
				{
					service.ResubmitProtocol(new ResubmitProtocolRequest(item.OrderRef));
				});

			this.Context.InvalidateFolders(typeof(Folders.Registration.RejectedProtocolFolder));
			this.Context.InvalidateFolders(typeof(Folders.Registration.PendingProtocolFolder));

			return true;
		}
	}
}
