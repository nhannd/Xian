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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Client.Workflow;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/View Images", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Verify", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.ViewImagesSmall.png", "Icons.ViewImagesMedium.png", "Icons.ViewImagesLarge.png")]
	[Tooltip("apply", "View Images")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("apply", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(OrderNoteboxItemToolExtensionPoint))]
	[ExtensionOf(typeof(EmergencyWorkflowItemToolExtensionPoint))]
	public class ViewImagesTool : Tool<IToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks>
		/// A no-args constructor is required by the framework.  Do not remove.
		/// </remarks>
		public ViewImagesTool()
		{
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			if (this.ContextBase is IReportingWorkflowItemToolContext)
			{
				((IReportingWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
			else if (this.ContextBase is IRegistrationWorkflowItemToolContext)
			{
				((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
			else if (this.ContextBase is IOrderNoteboxItemToolContext)
			{
				((IOrderNoteboxItemToolContext)this.ContextBase).SelectionChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
		}

		public bool Visible
		{
			get { return ViewImagesHelper.IsSupported; }
		}

		/// <summary>
		/// Notifies that the <see cref="Enabled"/> state of this tool has changed.
		/// </summary>
		public event EventHandler VisibleChanged
		{
			add {  }
			remove { }
		}

		private bool DetermineEnablement()
		{
			if (this.ContextBase is IReportingWorkflowItemToolContext)
			{
				return (((IReportingWorkflowItemToolContext)this.ContextBase).SelectedItems != null
					&& ((IReportingWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
			}
			else if (this.ContextBase is IRegistrationWorkflowItemToolContext)
			{
				return (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
					&& ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
			}
			else if (this.ContextBase is IOrderNoteboxItemToolContext)
			{
				return (((IOrderNoteboxItemToolContext)this.ContextBase).SelectedItems != null
					&& ((IOrderNoteboxItemToolContext)this.ContextBase).SelectedItems.Count == 1);
			}
			return false;
		}

		/// <summary>
		/// Gets whether this tool is enabled/disabled in the UI.
		/// </summary>
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

		/// <summary>
		/// Notifies that the <see cref="Enabled"/> state of this tool has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public void Apply()
		{				
			if (this.ContextBase is IRegistrationWorkflowItemToolContext)
			{
				IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
			    OpenViewer((WorklistItemSummaryBase)context.Selection.Item, context.DesktopWindow);
			}
			else if (this.ContextBase is IReportingWorkflowItemToolContext)
			{
			    IReportingWorkflowItemToolContext context = (IReportingWorkflowItemToolContext)this.ContextBase;
			    OpenViewer((WorklistItemSummaryBase)context.Selection.Item, context.DesktopWindow);
			}
			else if (this.ContextBase is IOrderNoteboxItemToolContext)
			{
				IOrderNoteboxItemToolContext context = (IOrderNoteboxItemToolContext)this.ContextBase;
				OpenViewer((OrderNoteboxItemSummary)context.Selection.Item, context.DesktopWindow);
			}
		}

		private static void OpenViewer(WorklistItemSummaryBase item, IDesktopWindow desktopWindow)
		{
			if (item != null)
				OpenViewer(item.AccessionNumber, desktopWindow);
		}

		private static void OpenViewer(OrderNoteboxItemSummary item, IDesktopWindow desktopWindow)
		{
			if(item != null)
				OpenViewer(item.AccessionNumber, desktopWindow);
		}

		private static void OpenViewer(string accession, IDesktopWindow desktopWindow)
		{
			if (!ViewImagesHelper.IsSupported)
			{
				// this should not happen because the tool will be invisible
				desktopWindow.ShowMessageBox("No image viewing support.", MessageBoxActions.Ok);
				return;
			}

			try
			{
				ViewImagesHelper.Open(accession);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				desktopWindow.ShowMessageBox("Images cannot be opened.", MessageBoxActions.Ok);
			}
		}
	}
}
