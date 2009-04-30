#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            this.studyStorageTableAdapter.Fill(this.imageServerDataSet.StudyStorage);
            this.studyTableAdapter.Fill(this.imageServerDataSet.Study);

            dataGridView1.DataSource = this.imageServerDataSet;
            dataGridView1.DataMember = "Study";
            dataGridView1.Update();

        }

        private void Apply_Click(object sender, EventArgs e)
        {
            DataRowView view = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
            if (view != null)
            {
                Guid guid = (Guid) view.Row["GUID"];

                IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

                using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    IStudyEntityBroker studyBroker = ctx.GetBroker<IStudyEntityBroker>();
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
                    columns.WorkQueuePriorityEnum = Model.WorkQueuePriorityEnum.Medium;
                    columns.WorkQueueStatusEnum = Model.WorkQueueStatusEnum.Pending;
                    columns.WorkQueueTypeEnum = Model.WorkQueueTypeEnum.WebEditStudy;

                    XmlDocument doc = new XmlDocument();
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
            if (dataGridView1.SelectedRows != null)
            {
                DataRowView view = dataGridView1.SelectedRows[0].DataBoundItem as DataRowView;
                if (view!=null)
                {
                    guid = (Guid) view["GUID"];
                    Trace.WriteLine(guid);
                }
                
            }
            this.studyStorageTableAdapter.Fill(this.imageServerDataSet.StudyStorage);
            this.studyTableAdapter.Fill(this.imageServerDataSet.Study);

            if (guid!=Guid.Empty)
            {
                int index = studyBindingSource.Find("GUID", guid);
                Trace.WriteLine(index);
                this.studyBindingSource.Position = index;
                dataGridView1.Rows[index].Selected = true;
            }
            
        }

    }
}