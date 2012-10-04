#region License (non-CC)

// Copyright (c) 2007, Paul Wheeler
//
// This work is licensed under a Creative Commons Attribution 3.0 Unported License.
// For the complete license, see http://creativecommons.org/licenses/by/3.0/
// Or, you may send a letter to: 
//    Creative Commons
//    171 Second Street, Suite 300
//    San Francisco, California, 94105, USA.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HttpNamespaceManager.UI
{
    public partial class UsageForm : Form
    {
        public UsageForm()
        {
            InitializeComponent();
        }

        private void UsageForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(labelUsage.Right + 18, labelUsage.Bottom + 62);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}