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
    /// Provides a Windows Forms user-interface for <see cref="JscriptComponent"/>
    /// </summary>
    public partial class JscriptComponentControl : CustomUserControl
    {
        private JscriptComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public JscriptComponentControl(JscriptComponent component)
        {
            InitializeComponent();

            _component = component;
            _script.DataBindings.Add("Text", _component, "Script", true, DataSourceUpdateMode.OnPropertyChanged);
            _result.DataBindings.Add("Text", _component, "Result", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _runButton_Click(object sender, EventArgs e)
        {
            _component.RunScript();
        }
    }
}
