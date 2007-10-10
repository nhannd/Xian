using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="SettingsManagementComponent"/>
    /// </summary>
    public partial class SettingsManagementComponentControl : ApplicationComponentUserControl
    {
        private SettingsManagementComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsManagementComponentControl(SettingsManagementComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
            _settingsGroupTableView.Table = _component.SettingsGroupTable;
            _settingsGroupTableView.DataBindings.Add("Selection", _component, "SelectedSettingsGroup", true, DataSourceUpdateMode.OnPropertyChanged);

            _valueTableView.Table = _component.SettingsPropertiesTable;
            _valueTableView.DataBindings.Add("Selection", _component, "SelectedSettingsProperty", true, DataSourceUpdateMode.OnPropertyChanged);
            _valueTableView.ToolbarModel = _component.SettingsPropertiesActionModel;

            _valueTableView.ItemDoubleClicked += new EventHandler(ValueTableItemDoubleClicked);
        }

        private void ValueTableItemDoubleClicked(object sender, EventArgs e)
        {
            _component.SettingsPropertyDoubleClicked();
        }
    }
}
