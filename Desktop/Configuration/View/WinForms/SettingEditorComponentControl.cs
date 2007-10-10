using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="SettingEditorComponent"/>
    /// </summary>
    public partial class SettingEditorComponentControl : ApplicationComponentUserControl
    {
        private SettingEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingEditorComponentControl(SettingEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _defaultValue.DataBindings.Add("Value", _component, "DefaultValue", true, DataSourceUpdateMode.Never);
            _editableValue.DataBindings.Add("Value", _component, "CurrentValue", true, DataSourceUpdateMode.OnPropertyChanged);
            _okButton.DataBindings.Add("Enabled", _component, "Modified", true, DataSourceUpdateMode.Never);
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
