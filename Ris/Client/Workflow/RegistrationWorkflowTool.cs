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

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class RegistrationWorkflowTool : WorkflowItemTool<RegistrationWorklistItem, IRegistrationWorkflowItemToolContext>
	{
		protected RegistrationWorkflowTool(string operationName)
			: base(operationName)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IRegistrationWorkflowService));
		}
	}


	[MenuAction("apply", "folderexplorer-items-contextmenu/Check-in", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Check-in", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.CheckInToolSmall.png", "Icons.CheckInToolMedium.png", "Icons.CheckInToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Procedure.CheckIn)]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class CheckInProceduresTool : RegistrationWorkflowTool
	{
		public CheckInProceduresTool()
			: base("CheckInProcedure")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDropHandler(typeof(Folders.Registration.CheckedInFolder), this);
		}

		protected override bool Execute(RegistrationWorklistItem item)
		{
			CheckInOrderComponent checkInComponent = new CheckInOrderComponent(item);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow,
				checkInComponent,
				String.Format("Checking in {0}", PersonNameFormat.Format(item.PatientName)));

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				this.Context.InvalidateFolders(typeof(Folders.Registration.CheckedInFolder));
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}

