using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="RadiologistSelectionComponent"/>
    /// </summary>
    public partial class RadiologistSelectionComponentControl : ApplicationComponentUserControl
    {
        private readonly RadiologistSelectionComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public RadiologistSelectionComponentControl(RadiologistSelectionComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

            _radiologistTable.Table = _component.Radiologists;
            _radiologistTable.DataBindings.Add("Selection", _component, "RadiologistSelection", true, DataSourceUpdateMode.OnPropertyChanged);
            _makeDefault.DataBindings.Add("Checked", _component, "MakeDefaultChecked", true, DataSourceUpdateMode.OnPropertyChanged);
            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _radiologistTable_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
