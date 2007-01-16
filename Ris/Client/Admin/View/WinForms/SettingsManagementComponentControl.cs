using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
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
            _settingsGroupTreeView.Tree = _component.SettingsGroupTree;
            _settingsGroupTreeView.DataBindings.Add("Selection", _component, "SelectedSettingsGroup", true, DataSourceUpdateMode.OnPropertyChanged);

            _valueTableView.Table = _component.SettingsPropertiesTable;
        }
    }
}
