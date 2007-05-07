using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="OrderEntryComponent"/>
    /// </summary>
    public partial class OrderEntryComponentControl : ApplicationComponentUserControl
    {
        private OrderEntryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderEntryComponentControl(OrderEntryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _visitTable.Table = _component.VisitTable;
            _visitTable.DataBindings.Add("Selection", _component, "SelectedVisit", true, DataSourceUpdateMode.OnPropertyChanged);
            this.ErrorProvider.SetIconAlignment(_visitTable, ErrorIconAlignment.TopRight);

            _diagnosticService.DataSource = _component.DiagnosticServiceChoices;
            _diagnosticService.DataBindings.Add("Value", _component, "SelectedDiagnosticService", true, DataSourceUpdateMode.OnPropertyChanged);
            _component.DiagnosticServiceChanged += new EventHandler(DiagnosticServiceChangedEventHandler);

            _diagnosticServiceBreakdown.DataBindings.Add("Selection", _component, "SelectedDiagnosticServiceBreakdownItem", true, DataSourceUpdateMode.OnPropertyChanged);

            _priority.DataSource = _component.PriorityChoices;
            _priority.DataBindings.Add("Value", _component, "SelectedPriority", true, DataSourceUpdateMode.OnPropertyChanged);

            _orderingFacility.DataSource = _component.FacilityChoices;
            _orderingFacility.DataBindings.Add("Value", _component, "SelectedFacility", true, DataSourceUpdateMode.OnPropertyChanged);

            _orderingPhysician.DataSource = _component.OrderingPhysicianChoices;
            _orderingPhysician.DataBindings.Add("Value", _component, "SelectedOrderingPhysician", true, DataSourceUpdateMode.OnPropertyChanged);

            _scheduleOrder.DataBindings.Add("Checked", _component, "ScheduleOrder", true, DataSourceUpdateMode.OnPropertyChanged);
            _schedulingRequestDateTime.DataBindings.Add("Value", _component, "SchedulingRequestDateTime", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void DiagnosticServiceChangedEventHandler(object sender, EventArgs e)
        {
            _diagnosticServiceBreakdown.Tree = _component.DiagnosticServiceBreakdown;
            _diagnosticServiceBreakdown.ExpandAll();
        }

        private void _placeOrderButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.PlaceOrder();
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
