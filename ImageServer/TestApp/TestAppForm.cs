using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class TestAppForm : Form
    {
        public TestAppForm()
        {
            
            InitializeComponent();
        }

        private void checkBoxLoadTest_CheckedChanged(object sender, EventArgs e)
        {
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

            FileSystemMonitor monitor = new FileSystemMonitor(store);

            monitor.Load();
        }
    }
}