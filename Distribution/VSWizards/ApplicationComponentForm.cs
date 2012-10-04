#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.VSWizards
{
    public partial class ApplicationComponentForm : Form
    {
        public ApplicationComponentForm(IList<string> classNames, IList<string> projectNames, string defaultViewProject)
        {
            InitializeComponent();
            _classList.DataSource = classNames;
            _viewProject.DataSource = projectNames;
            _viewProject.SelectedItem = defaultViewProject;
        }

        public string ViewProject
        {
            get { return (string)_viewProject.SelectedItem; }
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}