#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;

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
            _component.Cancel();
        }

        private void _setTitle_Click(object sender, EventArgs e)
        {
            _component.SetTitle();
        }

        private void _modify_Click(object sender, EventArgs e)
        {
            _component.Modify();
        }

        private void _accept_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

		private void _showWorkspaceDialogBox_Click(object sender, EventArgs e)
		{
			_component.ShowWorkspaceDialogBox();
		}
    }
}
