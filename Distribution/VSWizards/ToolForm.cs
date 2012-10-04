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
    public partial class ToolForm : Form
    {
        private IList<ToolExtPoint> _extPoints;

        public ToolForm(IList<string> classNames, IList<ToolTemplate> templates, IList<ToolExtPoint> extPoints)
        {
            InitializeComponent();
            _classList.DataSource = classNames;
            _templateChoices.DataSource = templates;
            _toolExtPointChoices.DataSource = _extPoints = extPoints;

            ResetExtPointChoice();
            ResetContextInterface();
        }

        public ToolTemplate SelectedTemplate
        {
            get { return (ToolTemplate)_templateChoices.SelectedItem; }
        }

        public ToolExtPoint SelectedExtensionPoint
        {
            get { return (ToolExtPoint)_toolExtPointChoices.SelectedItem; }
        }

        public string CustomExtensionPoint
        {
            get { return _toolExtPointChoices.Text; }
        }

        public string CustomToolContext
        {
            get { return _contextInterface.Text; }
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void _templateChoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetExtPointChoice();
        }

        private void _toolExtPointChoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetContextInterface();
        }

        private void _toolExtPointChoices_TextChanged(object sender, EventArgs e)
        {
            ResetContextInterface();
        }


        private void ResetExtPointChoice()
        {
            ToolTemplate t = this.SelectedTemplate;
            if (t.ToolExtPointClass != null)
            {
                foreach (ToolExtPoint xp in _extPoints)
                {
                    if (xp.ExtensionPoint == t.ToolExtPointClass)
                    {
                        _toolExtPointChoices.SelectedItem = xp;
                    }
                }
            }
            _toolExtPointChoices.Enabled = (t.ToolExtPointClass == null);
        }

        private void ResetContextInterface()
        {
            ToolExtPoint xp = this.SelectedExtensionPoint;
            if (xp != null)
            {
                _contextInterface.Text = xp.ContextInterface;
            }
            _contextInterface.Enabled = (this.SelectedTemplate.ToolContextInterface == null && xp == null);
        }

    }
}