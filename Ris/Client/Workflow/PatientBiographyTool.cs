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
using ClearCanvas.Ris.Application.Common.OrderNotes;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ButtonAction("view", "folderexplorer-items-toolbar/Patient Biography", "View")]
	[MenuAction("view", "folderexplorer-items-contextmenu/Patient Biography", "View")]
	[ButtonAction("view", "patientsearch-items-toolbar/Patient Biography", "View")]
	[MenuAction("view", "patientsearch-items-contextmenu/Patient Biography", "View")]
	[EnabledStateObserver("view", "Enabled", "EnabledChanged")]
	[Tooltip("view", "Open patient biography")]
	[IconSet("view", IconScheme.Colour, "PatientDetailsToolSmall.png", "PatientDetailsToolMedium.png", "PatientDetailsToolLarge.png")]
	[ActionPermission("view", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.PatientBiography.View)]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(OrderNoteboxItemToolExtensionPoint))]
	[ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
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
			else if (this.ContextBase is IPatientSearchToolContext)
			{
				((IPatientSearchToolContext)this.ContextBase).SelectedProfileChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
		}

		private bool DetermineEnablement()
		{
			if (this.ContextBase is IWorkflowItemToolContext)
			{
				var ctx = (IWorkflowItemToolContext) this.ContextBase;
				return ctx.Selection != null && ctx.Selection.Items.Length == 1;
			}
			else if (this.ContextBase is IPatientSearchToolContext)
			{
				var context = (IPatientSearchToolContext)this.ContextBase;
				return (context.SelectedProfile != null && context.SelectedProfile.PatientProfileRef != null);
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
			if(this.ContextBase is IWorkflowItemToolContext)
			{
				var context = (IWorkflowItemToolContext)ContextBase;
				if (this.Context is IOrderNoteboxItemToolContext)
				{
					var item = (OrderNoteboxItemSummary)context.Selection.Item;
					OpenPatient(item.PatientRef, item.PatientProfileRef, item.OrderRef, context.DesktopWindow);
				}
				else
				{
					var item = (WorklistItemSummaryBase) context.Selection.Item;
					OpenPatient(item.PatientRef, item.PatientProfileRef, item.OrderRef, context.DesktopWindow);
				}
			}
			else if (this.ContextBase is IPatientSearchToolContext)
			{
				var context = (IPatientSearchToolContext)this.ContextBase;
				var profile = context.SelectedProfile;
				OpenPatient(profile.PatientRef, profile.PatientProfileRef, null, context.DesktopWindow);
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
