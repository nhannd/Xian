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
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client.Workflow;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
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
            :base(component)
        {
            InitializeComponent();
            _component = component;

            Control banner = (Control)_component.BannerComponentHost.ComponentView.GuiElement;
            banner.Dock = DockStyle.Fill;
            _bannerPanel.Controls.Add(banner);

            Control rightHandTabPages = (Control)_component.RightHandComponentContainerHost.ComponentView.GuiElement;
            rightHandTabPages.Dock = DockStyle.Fill;
            _rightHandPanel.Controls.Add(rightHandTabPages);

            Control orderNoteSummary = (Control)_component.OrderNoteSummaryHost.ComponentView.GuiElement;
            orderNoteSummary.Dock = DockStyle.Fill;
            _orderNotesTab.Controls.Add(orderNoteSummary);

            // force toolbars to be displayed (VS designer seems to have a bug with this)
            _proceduresTableView.ShowToolbar = true;
            _recipientsTableView.ShowToolbar = true;

            _diagnosticService.LookupHandler = _component.DiagnosticServiceLookupHandler;
            _diagnosticService.DataBindings.Add("Value", _component, "SelectedDiagnosticService", true, DataSourceUpdateMode.OnPropertyChanged);
            _diagnosticService.DataBindings.Add("Enabled", _component, "IsDiagnosticServiceEditable");

            _indication.DataBindings.Add("Value", _component, "Indication", true, DataSourceUpdateMode.OnPropertyChanged);

            _proceduresTableView.Table = _component.Procedures;
            _proceduresTableView.MenuModel = _component.ProceduresActionModel;
            _proceduresTableView.ToolbarModel = _component.ProceduresActionModel;
            _proceduresTableView.DataBindings.Add("Selection", _component, "SelectedProcedure", true, DataSourceUpdateMode.OnPropertyChanged);

            _recipientsTableView.Table = _component.Recipients;
            _recipientsTableView.MenuModel = _component.RecipientsActionModel;
            _recipientsTableView.ToolbarModel = _component.RecipientsActionModel;
            _recipientsTableView.DataBindings.Add("Selection", _component, "SelectedRecipient", true, DataSourceUpdateMode.OnPropertyChanged);
            _addConsultantButton.DataBindings.Add("Enabled", _component.RecipientsActionModel.Add, "Enabled");

            _consultantLookup.LookupHandler = _component.RecipientsLookupHandler;
            _consultantLookup.DataBindings.Add("Value", _component, "RecipientToAdd", true, DataSourceUpdateMode.OnPropertyChanged);
            _consultantContactPoint.DataBindings.Add("DataSource", _component, "RecipientContactPointChoices", true, DataSourceUpdateMode.Never);
            _consultantContactPoint.DataBindings.Add("Value", _component, "RecipientContactPointToAdd", true, DataSourceUpdateMode.OnPropertyChanged);
            _consultantContactPoint.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatContactPoint(e.ListItem); };

            _visit.DataSource = _component.ActiveVisits;
            _visit.DataBindings.Add("Value", _component, "SelectedVisit", true, DataSourceUpdateMode.OnPropertyChanged);
            _visit.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatVisit(e.ListItem); };

            _priority.DataSource = _component.PriorityChoices;
            _priority.DataBindings.Add("Value", _component, "SelectedPriority", true, DataSourceUpdateMode.OnPropertyChanged);

            _orderingFacility.DataBindings.Add("Value", _component, "OrderingFacility", true, DataSourceUpdateMode.OnPropertyChanged);

            _orderingPractitioner.LookupHandler = _component.OrderingPractitionerLookupHandler;
            _orderingPractitioner.DataBindings.Add("Value", _component, "SelectedOrderingPractitioner", true, DataSourceUpdateMode.OnPropertyChanged);
            _orderingPractitionerContactPoint.DataBindings.Add("DataSource", _component, "OrderingPractitionerContactPointChoices", true, DataSourceUpdateMode.Never);
            _orderingPractitionerContactPoint.DataBindings.Add("Value", _component, "SelectedOrderingPractitionerContactPoint", true, DataSourceUpdateMode.OnPropertyChanged);
            _orderingPractitionerContactPoint.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatContactPoint(e.ListItem); };

            // bind date and time to same property
            _schedulingRequestDate.DataBindings.Add("Value", _component, "SchedulingRequestTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _schedulingRequestTime.DataBindings.Add("Value", _component, "SchedulingRequestTime", true, DataSourceUpdateMode.OnPropertyChanged);

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
        }

        private void _placeOrderButton_Click(object sender, EventArgs e)
        {
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
            _component.EditSelectedProcedure();
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
