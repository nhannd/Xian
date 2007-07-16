using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CloseHelperComponent"/>
    /// </summary>
    public partial class CloseHelperComponentControl : ApplicationComponentUserControl
    {
        private CloseHelperComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public CloseHelperComponentControl(CloseHelperComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
            _workspaceTableView.Table = _component.Workspaces;
            _workspaceTableView.DataBindings.Add("Selection", _component, "SelectedWorkspace", true, DataSourceUpdateMode.OnPropertyChanged);
            // TODO add .NET databindings to _component
        }
    }
}
