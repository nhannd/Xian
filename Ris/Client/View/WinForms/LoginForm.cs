using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public string UserName
        {
            get { return _userName.Text; }
        }

        private void _loginButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void _userName_TextChanged(object sender, EventArgs e)
        {
            _loginButton.Enabled = !string.IsNullOrEmpty(_userName.Text);
        }
    }
}