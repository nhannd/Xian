using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="OrderEntryComponent"/>
    /// </summary>
    public partial class OrderEntryComponentControl : CustomUserControl
    {
        private OrderEntryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderEntryComponentControl(OrderEntryComponent component)
        {
            InitializeComponent();

            _component = component;

            _visitTable.Table = _component.VisitChoices;

            _diagnosticService.DataSource = _component.DiagnosticServiceChoices;
            _diagnosticService.DataBindings.Add("Value", _component, "DiagnosticService", true, DataSourceUpdateMode.OnPropertyChanged);
            _component.DiagnosticServiceChanged += new EventHandler(DiagnosticServiceChangedEventHandler);

            _priority.DataSource = _component.PriorityChoices;
            _orderingFacility.DataSource = _component.FacilityChoices;
            _orderingPhysician.DataSource = _component.OrderingPhysicianChoices;
        }

        private void DiagnosticServiceChangedEventHandler(object sender, EventArgs e)
        {
            _diagnosticServiceBreakdown.RootNodes = _component.DiagnosticServiceBreakdown;
            _diagnosticServiceBreakdown.ExpandAll();
        }
    }
}
