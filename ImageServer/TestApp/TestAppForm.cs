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
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Database;

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
            try
            {

                FilesystemMonitor monitor = new FilesystemMonitor();

                monitor.Load();


                TypeEnum t = new TypeEnum();
                t.SetEnum(200);

                IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();

                IInsertStudyStorage insert = ctx.GetBroker<IInsertStudyStorage>();

                StudyStorageInsertParameters criteria = new StudyStorageInsertParameters();

                criteria.StudyInstanceUid = "1.2.3.4";
                criteria.FilesystemKey = monitor.Filesystems[0].GetKey();
                criteria.Folder = "20070101";
                criteria.ServerPartitionKey = monitor.Partitions[0].GetKey();

                IList<StudyStorageLocation> storage = insert.Execute(criteria);

                StudyStorageLocation storageEntry = storage[0];
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x);
            }
        }
    }
}