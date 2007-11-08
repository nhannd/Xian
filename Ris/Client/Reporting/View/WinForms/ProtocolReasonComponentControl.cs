using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProtocolReasonComponent"/>
    /// </summary>
    public partial class ProtocolReasonComponentControl : ApplicationComponentUserControl
    {
        private ProtocolReasonComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolReasonComponentControl(ProtocolReasonComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            _reason.DataSource = _component.ReasonChoices;
            _reason.DataBindings.Add("Value", _component, "SelectedReasonChoice", true, DataSourceUpdateMode.OnPropertyChanged);
            _btnOK.DataBindings.Add("Enabled", _component, "OkayEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            _component.Okay();
        }

        private void _btnCancel_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
