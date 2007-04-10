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
    /// Provides a Windows Forms user-interface for <see cref="CancelOrderComponent"/>
    /// </summary>
    public partial class CancelOrderComponentControl : ApplicationComponentUserControl
    {
        private CancelOrderComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public CancelOrderComponentControl(CancelOrderComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _cancelReason.DataSource = _component.CancelReasonChoices;
            _cancelReason.DataBindings.Add("Value", _component, "SelectedCancelReason", true, DataSourceUpdateMode.OnPropertyChanged); 
            _cancelOrderTableView.Table = _component.CancelOrderTable;
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
