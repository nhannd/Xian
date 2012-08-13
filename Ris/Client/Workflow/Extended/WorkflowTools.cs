#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Extended.Common.OrderNotes;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Images.View)]
	[MenuAction("apply", "folderexplorer-items-contextmenu/View Images", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Verify", "Apply")]
	[ButtonAction("apply", "biography-reports-toolbar/View Images", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.ViewImagesSmall.png", "Icons.ViewImagesMedium.png", "Icons.ViewImagesLarge.png")]
	[Tooltip("apply", "View Images")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("apply", "Visible", "VisibleChanged")]
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

			if (this.ContextBase is IRegistrationWorkflowItemToolContext)
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
			add { }
			remove { }
		}

		private bool DetermineEnablement()
		{
			if (this.ContextBase is IRegistrationWorkflowItemToolContext)
			{
				return (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
						&& ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
			}

			if (this.ContextBase is IOrderNoteboxItemToolContext)
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
				var context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
				OpenViewer((WorklistItemSummaryBase)context.Selection.Item, context.DesktopWindow);
			}
			else if (this.ContextBase is IOrderNoteboxItemToolContext)
			{
				var context = (IOrderNoteboxItemToolContext)this.ContextBase;
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
			if (item != null)
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
				ExceptionHandler.Report(e, desktopWindow);
			}
		}
	}

	[ButtonAction("view", "folderexplorer-items-toolbar/Patient Biography", "View")]
	[MenuAction("view", "folderexplorer-items-contextmenu/Patient Biography", "View")]
	[ButtonAction("view", "patientsearch-items-toolbar/Patient Biography", "View")]
	[MenuAction("view", "patientsearch-items-contextmenu/Patient Biography", "View")]
	[EnabledStateObserver("view", "Enabled", "EnabledChanged")]
	[Tooltip("view", "Open patient biography")]
	[IconSet("view", IconScheme.Colour, "PatientDetailsToolSmall.png", "PatientDetailsToolMedium.png", "PatientDetailsToolLarge.png")]
	[ActionPermission("view", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.PatientBiography.View)]
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(EmergencyWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(OrderNoteboxItemToolExtensionPoint))]
	public class PatientBiographyTool : Tool<IToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public override void Initialize()
		{
			base.Initialize();

			if (this.ContextBase is IWorkflowItemToolContext)
			{
				((IWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
		}

		private bool DetermineEnablement()
		{
			if (this.ContextBase is IWorkflowItemToolContext)
			{
				var ctx = (IWorkflowItemToolContext)this.ContextBase;
				return ctx.Selection != null && ctx.Selection.Items.Length == 1;
			}

			return false;
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
				if (_enabled == value)
					return;

				_enabled = value;
				EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public void View()
		{
			if (this.ContextBase is IWorkflowItemToolContext)
			{
				var context = (IWorkflowItemToolContext)ContextBase;
				if (this.Context is IOrderNoteboxItemToolContext)
				{
					var item = (OrderNoteboxItemSummary)context.Selection.Item;
					OpenPatient(item.PatientRef, item.PatientProfileRef, item.OrderRef, context.DesktopWindow);
				}
				else
				{
					var item = (WorklistItemSummaryBase)context.Selection.Item;
					OpenPatient(item.PatientRef, item.PatientProfileRef, item.OrderRef, context.DesktopWindow);
				}
			}
		}

		protected static void OpenPatient(EntityRef patientRef, EntityRef profileRef, EntityRef orderRef, IDesktopWindow window)
		{
			try
			{
				var document = DocumentManager.Get<PatientBiographyDocument>(patientRef);
				if (document == null)
				{
					document = new PatientBiographyDocument(patientRef, profileRef, orderRef, window);
					document.Open();
				}
				else
				{
					document.Open();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, window);
			}
		}
	}

}
