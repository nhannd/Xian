#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class TestEditStudyForm : Form
    {
        public TestEditStudyForm()
        {
            InitializeComponent();

        }

        private void TestEditStudyForm_Load(object sender, EventArgs e)
        {
            studyStorageTableAdapter.Fill(this.imageServerDataSet.StudyStorage);
            studyTableAdapter.Fill(this.imageServerDataSet.Study);

            dataGridView1.DataSource = this.imageServerDataSet;
            dataGridView1.DataMember = "Study";
            dataGridView1.Update();

        }

        private void Apply_Click(object sender, EventArgs e)
        {
            var view = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
            if (view != null)
            {
                var guid = (Guid) view.Row["GUID"];

                IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

                using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    var studyBroker = ctx.GetBroker<IStudyEntityBroker>();
                    var key = new ServerEntityKey("Study", guid);
                    Model.Study study = studyBroker.Load(key);

                    var storageBroker = ctx.GetBroker<IStudyStorageEntityBroker>();
                    var parms = new StudyStorageSelectCriteria();
                    parms.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
                    parms.StudyInstanceUid.EqualTo(study.StudyInstanceUid);

                    Model.StudyStorage storage = storageBroker.Find(parms)[0];


                    var workQueueBroker = ctx.GetBroker<IWorkQueueEntityBroker>();
                    var columns = new WorkQueueUpdateColumns
                                      {
                                          ServerPartitionKey = study.ServerPartitionKey,
                                          StudyStorageKey = storage.GetKey(),
                                          ExpirationTime = DateTime.Now.AddHours(1),
                                          ScheduledTime = DateTime.Now,
                                          InsertTime = DateTime.Now,
                                          WorkQueuePriorityEnum = Model.WorkQueuePriorityEnum.Medium,
                                          WorkQueueStatusEnum = Model.WorkQueueStatusEnum.Pending,
                                          WorkQueueTypeEnum = Model.WorkQueueTypeEnum.WebEditStudy
                                      };

                    var doc = new XmlDocument();
                    doc.Load(new StringReader(textBox1.Text));

                    columns.Data = doc;

                    workQueueBroker.Insert(columns);

                    ctx.Commit();
                }
            }
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            Guid guid = Guid.Empty;
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var view = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
                if (view!=null)
                {
                    guid = (Guid) view["GUID"];
                    Trace.WriteLine(guid);
                }                
            }

            studyStorageTableAdapter.Fill(imageServerDataSet.StudyStorage);
            studyTableAdapter.Fill(imageServerDataSet.Study);

            if (guid!=Guid.Empty)
            {
                int index = studyBindingSource.Find("GUID", guid);
                Trace.WriteLine(index);
                studyBindingSource.Position = index;
                dataGridView1.Rows[index].Selected = true;
            }
        }
    }
}