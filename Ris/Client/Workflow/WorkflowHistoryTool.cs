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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	//NOTE: DEBUG'd out because there is no requirement for this feature, however it is a useful debugging tool (JR)
#if DEBUG
	[ButtonAction("view", "folderexplorer-items-toolbar/Workflow History", "View")]
	[MenuAction("view", "folderexplorer-items-contextmenu/Workflow History", "View")]
	[ButtonAction("view", "patientsearch-items-toolbar/Workflow History", "View")]
	[MenuAction("view", "patientsearch-items-contextmenu/Workflow History", "View")]
#endif
	[EnabledStateObserver("view", "Enabled", "EnabledChanged")]
	[Tooltip("view", "View workflow history")]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(EmergencyWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class WorkflowHistoryTool : Tool<IWorkflowItemToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public override void Initialize()
		{
			base.Initialize();

			this.Context.SelectionChanged += delegate
			{
				this.Enabled = DetermineEnablement();
			};
		}

		private bool DetermineEnablement()
		{
			return this.Context.Selection != null && this.Context.Selection.Items.Length == 1;
		}

		public bool Enabled
		{
			get
			{
				this.Enabled = DetermineEnablement();
				return _enabled;
			}
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public void View()
		{
			WorklistItemSummaryBase item = (WorklistItemSummaryBase) this.Context.Selection.Item;
			Open(item.OrderRef, this.Context.DesktopWindow);
		}

		protected static void Open(EntityRef orderRef, IDesktopWindow window)
		{
			ApplicationComponent.LaunchAsDialog(
				window,
				new WorkflowHistoryComponent(orderRef),
				"Workflow History");
		}
	}
}
