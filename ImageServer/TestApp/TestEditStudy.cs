using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ClearCanvas.Common;
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
            // TODO: This line of code loads data into the 'imageServerDataSet.StudyStorage' table. You can move, or remove it, as needed.
            this.studyStorageTableAdapter.Fill(this.imageServerDataSet.StudyStorage);
            // TODO: This line of code loads data into the 'imageServerDataSet.Study' table. You can move, or remove it, as needed.
            this.studyTableAdapter.Fill(this.imageServerDataSet.Study);

            dataGridView1.DataSource = this.imageServerDataSet;
            dataGridView1.DataMember = "Study";
            dataGridView1.Update();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataRowView view = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
            if (view!=null)
            {
                Guid guid = (Guid) view.Row["GUID"];

                IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
                
                using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    IStudyEntityBroker studyBroker = ctx.GetBroker<IStudyEntityBroker>();
                    StudySelectCriteria criteria = new StudySelectCriteria();
                    ServerEntityKey key = new ServerEntityKey("Study", guid);
                    Model.Study study = studyBroker.Load(key);

                    IStudyStorageEntityBroker storageBroker = ctx.GetBroker<IStudyStorageEntityBroker>();
                    StudyStorageSelectCriteria parms = new StudyStorageSelectCriteria();
                    parms.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
                    parms.StudyInstanceUid.EqualTo(study.StudyInstanceUid);

                    Model.StudyStorage storage = storageBroker.Find(parms)[0];


                    IWorkQueueEntityBroker workQueueBroker = ctx.GetBroker<IWorkQueueEntityBroker>();
                    WorkQueueUpdateColumns columns = new WorkQueueUpdateColumns();
                    columns.ServerPartitionKey = study.ServerPartitionKey;
                    columns.StudyStorageKey = storage.GetKey();
                    columns.ExpirationTime = DateTime.Now.AddHours(1);
                    columns.ScheduledTime = DateTime.Now;
                    columns.InsertTime = DateTime.Now;
                    columns.WorkQueuePriorityEnum = Model.WorkQueuePriorityEnum.GetEnum("Medium");
                    columns.WorkQueueStatusEnum = Model.WorkQueueStatusEnum.GetEnum("Pending");
                    columns.WorkQueueTypeEnum = Model.WorkQueueTypeEnum.GetEnum("WebEditStudy");

                    XmlDocument doc = new XmlDocument();
                    doc.Load(new StringReader(textBox1.Text));

                    columns.Data = doc;

                    workQueueBroker.Insert(columns);

                    ctx.Commit();

                }
                

            }
        }
    }
}