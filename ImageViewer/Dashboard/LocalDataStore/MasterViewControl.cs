namespace ClearCanvas.ImageViewer.Dashboard.LocalDataStore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Data;
    using System.Text;
    using System.Windows.Forms;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using ClearCanvas.DataStore;

    public partial class MasterViewControl : UserControl
    {
        public MasterViewControl()
        {
            InitializeComponent();
            _connectionString.Load();
            _textHost.Text = _connectionString.DataSourceHost;
            _textName.Text = _connectionString.DataSourceServerName;
            _textDB.Text = _connectionString.DatabaseName;
            _textUser.Text = _connectionString.UserId;
            _textPassword.Text = _connectionString.Password;
            _textConnectionString.Text = _connectionString.ToMaskedString();
        }

        ~MasterViewControl()
        {
            _connectionString.Save();
        }

        public String ConnectionString
        {
            get { return _connectionString.ToString(); }
        }

        private void _textHost_Leave(object sender, EventArgs e)
        {
            UpdateConnectionString();
        }

        private void _textName_Leave(object sender, EventArgs e)
        {
            UpdateConnectionString();
        }

        private void _textDB_Leave(object sender, EventArgs e)
        {
            UpdateConnectionString();
        }

        private void _textUser_Leave(object sender, EventArgs e)
        {
            UpdateConnectionString();
        }

        private void _textPassword_Leave(object sender, EventArgs e)
        {
            UpdateConnectionString();
        }

        private void MasterViewControl_Leave(object sender, EventArgs e)
        {
            _connectionString.Save();
        }

        private void UpdateConnectionString()
        {
            _connectionString.Set(_textHost.Text,
                _textName.Text,
                _textDB.Text,
                _textUser.Text,
                _textPassword.Text);

            _textConnectionString.Text = _connectionString.ToMaskedString();
        }

        private ApplicationConnectionString _connectionString = new ApplicationConnectionString();
    }
}
