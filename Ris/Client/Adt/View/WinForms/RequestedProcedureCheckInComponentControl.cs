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
    /// Provides a Windows Forms user-interface for <see cref="RequestedProcedureCheckInComponent"/>
    /// </summary>
    public partial class RequestedProcedureCheckInComponentControl : ApplicationComponentUserControl
    {
        private RequestedProcedureCheckInComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public RequestedProcedureCheckInComponentControl(RequestedProcedureCheckInComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _requestedProcedureTableView.Table = _component.RequestedProcedureTable;
            _okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
