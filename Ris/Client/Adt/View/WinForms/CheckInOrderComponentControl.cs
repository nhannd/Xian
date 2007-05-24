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
    /// Provides a Windows Forms user-interface for <see cref="CheckInOrderComponent"/>
    /// </summary>
    public partial class CheckInOrderComponentControl : ApplicationComponentUserControl
    {
        private CheckInOrderComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public CheckInOrderComponentControl(CheckInOrderComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _orderTableView.Table = _component.OrderTable;
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
