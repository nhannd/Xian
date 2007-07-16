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
    /// Provides a Windows Forms user-interface for <see cref="TestComponent"/>
    /// </summary>
    public partial class TestComponentControl : ApplicationComponentUserControl
    {
        private TestComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public TestComponentControl(TestComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
            _label.DataBindings.Add("Text", _component, "Name");
            _text.DataBindings.Add("Text", _component, "Text", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _showMessageBox_Click(object sender, EventArgs e)
        {
            _component.ShowMessageBox();
        }

        private void _showDialogBox_Click(object sender, EventArgs e)
        {
            _component.ShowDialogBox();
        }

        private void _close_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _setTitle_Click(object sender, EventArgs e)
        {
            _component.SetTitle();
        }

        private void _modify_Click(object sender, EventArgs e)
        {
            _component.Modify();
        }
    }
}
