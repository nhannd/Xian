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
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.ImageViewer.Dashboard.LocalDataStore
{
    public partial class MasterViewControl : UserControl
    {
        public MasterViewControl()
        {
            InitializeComponent();
        }

        ~MasterViewControl()
        {
        }

        public String ConnectionString
        {
            get { return ""; }
        }

        private void _textHost_Leave(object sender, EventArgs e)
        {

        }

        private void _textName_Leave(object sender, EventArgs e)
        {

        }

        private void _textDB_Leave(object sender, EventArgs e)
        {

        }

        private void _textUser_Leave(object sender, EventArgs e)
        {

        }

        private void _textPassword_Leave(object sender, EventArgs e)
        {

        }

        private void MasterViewControl_Leave(object sender, EventArgs e)
        {

        }

        private void UpdateConnectionString()
        {

        }
    }
}
