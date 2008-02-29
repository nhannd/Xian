#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Web.UI;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls;

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    public partial class SearchPanel : UserControl
    {
        #region Private members
        private ServerPartition _serverPartition;
        private StudyController _controller;

        #endregion Private members

        #region Public Properties

        public ServerPartition ServerPartition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientId.Text = "";
            PatientName.Text = "";
            AccessionNumber.Text = "";
            StudyDescription.Text = "";
        }

        public override void DataBind()
        {
            StudyListGridView1.Partition = ServerPartition;
            base.DataBind();
            StudyListGridView1.DataBind();
        }



        #endregion Public Methods

        #region Protected Methods

        protected void LoadStudies()
        {
            StudySelectCriteria criteria = new StudySelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(ServerPartition.GetKey());

            if (!String.IsNullOrEmpty(PatientId.Text))
            {
                string key = PatientId.Text + "%";
                key = key.Replace("*", "%");
                criteria.PatientId.Like(key);
            }
            if (!String.IsNullOrEmpty(PatientName.Text))
            {
                string key = PatientName.Text + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.PatientsName.Like(key);
            }
            criteria.PatientsName.SortAsc(0);

            if (!String.IsNullOrEmpty(AccessionNumber.Text))
            {
                string key = AccessionNumber.Text + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.AccessionNumber.Like(key);
            }
            if (!String.IsNullOrEmpty(StudyDescription.Text))
            {
                string key = StudyDescription.Text + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.StudyDescription.Like(key);
            }

            StudyListGridView1.Studies = _controller.GetStudies(criteria);
            
            DataBind();
        }

        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            
            // initialize the controller
            _controller = new StudyController();


            // setup child controls
            GridPager1.ItemName = "Study";
            GridPager1.PuralItemName = "Studies";
            GridPager1.Target = StudyListGridView1.TheGrid;
            GridPager1.GetRecordCountMethod = delegate
                                                  {
                                                      return StudyListGridView1.Studies== null ? 0:StudyListGridView1.Studies.Count;
                                                  };


            ConfirmDialog1.Confirmed += delegate(object data)
                                {
                                    if (data is IList<Study>)
                                    {
                                        IList<Study> studies = data as IList<Study>;
                                        foreach (Study study in studies)
                                        {
                                            _controller.DeleteStudy(study);
                                        }
                                    }
                                    else if (data is Study)
                                    {
                                        Study study = data as Study;
                                        _controller.DeleteStudy(study);
                                    }

                                    LoadStudies();
                                };
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (StudyListGridView1.IsPostBack)
            {
                LoadStudies();
            }
            
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // Register a script that setup the client-sider event handlers
            ScriptTemplate template =
                new ScriptTemplate(typeof(SearchPanel).Assembly, "ClearCanvas.ImageServer.Web.Application.Search.SearchPanel.js");

            template.Replace("@@DELETE_BUTTON_CLIENTID@@", DeleteToolbarButton.ClientID);
            template.Replace("@@STUDYLIST_GRIDVIEW_CLIENTID@@", StudyListGridView1.TheGrid.ClientID);
            template.Replace("@@OPEN_STUDY_BASE_URL@@", Page.ResolveClientUrl("~/StudyDetails/StudyDetailsPage.aspx"));
            template.Replace("@@PARTITION_AE@@", ServerPartition.AeTitle);
            template.Replace("@@OPEN_STUDY_BUTTON_CLIENTID@@", ToolbarButton1.ClientID);
            
            ScriptManager.RegisterStartupScript(
                                    this,
                                    typeof(SearchPanel),
                                    ClientID + "_OnLoad",
                                    template.Script,
                                    true);

            UpdateUI();
        }

        protected void UpdateUI()
        {
            UpdateToolbarButtonState();
            UpdatePanel.Update(); // force refresh
        }
        
        protected void FilterButton_Click(object sender, ImageClickEventArgs e)
        {
            // reload the data
            // LoadStudies(); this call is redundant since we have reloaded the data in OnInit()
        }

        protected void OnDeleteToolbarButtonClick(object sender, ImageClickEventArgs e)
        {
            IList<Study> studies = StudyListGridView1.SelectedStudies;

            if (studies != null && studies.Count>0)
            {
                ConfirmDialog1.Message = string.Format("Are you sure to remove the following studies?<BR/>");
                ConfirmDialog1.Message += "<table>";
                foreach (Study study in studies)
                {
                    String text = String.Format("<tr align='left'><td>Patient:{0}&nbsp;&nbsp;</td><td>Accession:{1}&nbsp;&nbsp;</td><td>Description:{2}</td></tr>", 
                                    study.PatientsName, study.AccessionNumber, study.StudyDescription);
                    ConfirmDialog1.Message += text;
                }
                ConfirmDialog1.Message += "</table>";

                ConfirmDialog1.MessageType = ConfirmDialog.MessageTypeEnum.WARNING;
                ConfirmDialog1.Data = studies;
                ConfirmDialog1.Show();
            }
        }

        protected void UpdateToolbarButtonState()
        {
            IList<Study> studies = StudyListGridView1.SelectedStudies;
            if (studies != null)
            {
                StudyController controller = new StudyController();
                DeleteToolbarButton.Enabled = true;
                foreach (Study study in studies)
                {
                    if (controller.IsScheduledForDelete(study))
                    {
                        DeleteToolbarButton.Enabled = false;
                        break;
                    }
                }

            }
            else
            {
                DeleteToolbarButton.Enabled = false;
            }
        }


        #endregion Protected Methods


    }
}