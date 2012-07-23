#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="OrderEditorComponent"/>
	/// </summary>
	public partial class OrderEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly OrderEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public OrderEditorComponentControl(OrderEditorComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_overviewLayoutPanel.RowStyles[0].Height = _component.BannerHeight;

			var banner = (Control)_component.BannerComponentHost.ComponentView.GuiElement;
			banner.Dock = DockStyle.Fill;
			_bannerPanel.Controls.Add(banner);

			var rightHandTabPages = (Control)_component.RightHandComponentContainerHost.ComponentView.GuiElement;
			rightHandTabPages.Dock = DockStyle.Fill;
			_rightHandPanel.Controls.Add(rightHandTabPages);

			var orderNoteSummary = (Control)_component.OrderNoteSummaryHost.ComponentView.GuiElement;
			orderNoteSummary.Dock = DockStyle.Fill;
			_orderNotesTab.Controls.Add(orderNoteSummary);

			// force toolbars to be displayed (VS designer seems to have a bug with this)
			_proceduresTableView.ShowToolbar = true;
			_recipientsTableView.ShowToolbar = true;

			_diagnosticService.LookupHandler = _component.DiagnosticServiceLookupHandler;
			_diagnosticService.DataBindings.Add("Value", _component, "SelectedDiagnosticService", true, DataSourceUpdateMode.OnPropertyChanged);
			_diagnosticService.DataBindings.Add("Enabled", _component, "IsDiagnosticServiceEditable");

			_indication.DataBindings.Add("Value", _component, "Indication", true, DataSourceUpdateMode.OnPropertyChanged);
			_indication.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");

			_proceduresTableView.Table = _component.Procedures;
			_proceduresTableView.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_proceduresTableView.MenuModel = _component.ProceduresActionModel;
			_proceduresTableView.ToolbarModel = _component.ProceduresActionModel;
			_proceduresTableView.DataBindings.Add("Selection", _component, "SelectedProcedures", true, DataSourceUpdateMode.OnPropertyChanged);

			_recipientsTableView.Table = _component.Recipients;
			_recipientsTableView.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_recipientsTableView.MenuModel = _component.RecipientsActionModel;
			_recipientsTableView.ToolbarModel = _component.RecipientsActionModel;
			_recipientsTableView.DataBindings.Add("Selection", _component, "SelectedRecipient", true, DataSourceUpdateMode.OnPropertyChanged);

			_addConsultantButton.DataBindings.Add("Enabled", _component.RecipientsActionModel.Add, "Enabled");

			_consultantLookup.LookupHandler = _component.RecipientsLookupHandler;
			_consultantLookup.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_consultantLookup.DataBindings.Add("Value", _component, "RecipientToAdd", true, DataSourceUpdateMode.OnPropertyChanged);

			_consultantContactPoint.DataBindings.Add("DataSource", _component, "RecipientContactPointChoices", true, DataSourceUpdateMode.Never);
			_consultantContactPoint.DataBindings.Add("Value", _component, "RecipientContactPointToAdd", true, DataSourceUpdateMode.OnPropertyChanged);
			_consultantContactPoint.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_consultantContactPoint.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatContactPoint(e.ListItem); };

			_visit.DataSource = _component.ActiveVisits;
			_visit.DataBindings.Add("Value", _component, "SelectedVisit", true, DataSourceUpdateMode.OnPropertyChanged);
			_visit.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_visit.DataBindings.Add("Visible", _component, "VisitVisible");
			_visit.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatVisit(e.ListItem); };
			_visitSummaryButton.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_visitSummaryButton.DataBindings.Add("Visible", _component, "VisitVisible");

			_priority.DataSource = _component.PriorityChoices;
			_priority.DataBindings.Add("Value", _component, "SelectedPriority", true, DataSourceUpdateMode.OnPropertyChanged);
			_priority.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");

			_orderingFacility.DataBindings.Add("Value", _component, "OrderingFacility", true, DataSourceUpdateMode.OnPropertyChanged);
			// Ordering Facility's Enabled is not bound since it is always readonly (via designer)

			_orderingPractitioner.LookupHandler = _component.OrderingPractitionerLookupHandler;
			_orderingPractitioner.DataBindings.Add("Value", _component, "SelectedOrderingPractitioner", true, DataSourceUpdateMode.OnPropertyChanged);
			_orderingPractitioner.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");

			_orderingPractitionerContactPoint.DataBindings.Add("DataSource", _component, "OrderingPractitionerContactPointChoices", true, DataSourceUpdateMode.Never);
			_orderingPractitionerContactPoint.DataBindings.Add("Value", _component, "SelectedOrderingPractitionerContactPoint", true, DataSourceUpdateMode.OnPropertyChanged);
			_orderingPractitionerContactPoint.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_orderingPractitionerContactPoint.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatContactPoint(e.ListItem); };

			// bind date and time to same property
			_schedulingRequestDate.DataBindings.Add("Value", _component, "SchedulingRequestTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_schedulingRequestDate.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_schedulingRequestTime.DataBindings.Add("Value", _component, "SchedulingRequestTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_schedulingRequestTime.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");

			_reorderReason.DataSource = _component.CancelReasonChoices;
			_reorderReason.DataBindings.Add("Value", _component, "SelectedCancelReason", true, DataSourceUpdateMode.OnPropertyChanged);
			_reorderReason.DataBindings.Add("Visible", _component, "IsCancelReasonVisible");

			_downtimeAccession.DataBindings.Add("Visible", _component, "IsDowntimeAccessionNumberVisible");
			_downtimeAccession.DataBindings.Add("Value", _component, "DowntimeAccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);

			_component.PropertyChanged += _component_PropertyChanged;
		}

		private void _component_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ActiveVisits")
			{
				_visit.DataSource = _component.ActiveVisits;
			}
			else if (e.PropertyName == "PriorityChoices")
			{
				_priority.DataSource = _component.PriorityChoices;
			}
			else if (e.PropertyName == "CancelReasonChoices")
			{
				_reorderReason.DataSource = _component.CancelReasonChoices;
			}
			else if (e.PropertyName == "OrderingPractitionerContactPointChoices")
			{
				_orderingPractitionerContactPoint.DataSource = _component.OrderingPractitionerContactPointChoices;
			}
			else if (e.PropertyName == "RecipientContactPointChoices")
			{
				_consultantContactPoint.DataSource = _component.RecipientContactPointChoices;
			}
		}

		private void _placeOrderButton_Click(object sender, EventArgs e)
		{
			// bug #7781: switch back to this tab prior to validation
			_lowerLeftTabControl.SelectedTab = _proceduresTab;

			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Accept();
			}
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _addConsultantButton_Click(object sender, EventArgs e)
		{
			_component.AddRecipient();
		}

		private void _proceduresTableView_ItemDoubleClicked(object sender, EventArgs e)
		{
			_component.EditSelectedProcedures();
		}

		private void _visitSummaryButton_Click(object sender, EventArgs e)
		{
			_component.ShowVisitSummary();
		}

		private void OrderEditorComponentControl_Load(object sender, EventArgs e)
		{
			_downtimeAccession.Mask = _component.AccessionNumberMask;
		}
	}
}
