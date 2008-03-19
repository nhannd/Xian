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
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Search.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    [ClientScriptResource(ComponentType="ClearCanvas.ImageServer.Web.Application.Search.SearchPanel", ResourcePath="ClearCanvas.ImageServer.Web.Application.Search.SearchPanel.js")]
    public partial class SearchPanel : ScriptUserControl
    {
        #region Private members
        private ServerPartition _serverPartition;
        private StudyController _controller = new StudyController();

        #endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return this.DeleteToolbarButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("OpenButtonClientID")]
        public string OpenButtonClientID
        {
            get { return this.OpenStudyToolbarButton.ClientID; }
        }


        [ExtenderControlProperty]
        [ClientPropertyName("StudyListClientID")]
        public string StudyListClientID
        {
            get { return StudyListGridView.StudyListGrid.ClientID; }
        }


        [ExtenderControlProperty]
        [ClientPropertyName("OpenStudyPageUrl")]
        public string OpenStudyPageUrl
        {
            get { return Page.ResolveClientUrl("~/StudyDetails/StudyDetailsPage.aspx"); }
        }


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
            StudyListGridView.Partition = ServerPartition;
            base.DataBind();
            StudyListGridView.DataBind();
        }



        #endregion Public Methods

        #region Constructors
        public SearchPanel()
            : base(false, HtmlTextWriterTag.Div)
            {
            }

        #endregion Constructors

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

            StudyListGridView.Studies = _controller.GetStudies(criteria);
            
            DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // setup child controls
            GridPager.ItemName = "Study";
            GridPager.PuralItemName = "Studies";
            GridPager.Target = StudyListGridView.StudyListGrid;
            GridPager.GetRecordCountMethod = delegate
                                                  {
                                                      return StudyListGridView.Studies== null ? 0:StudyListGridView.Studies.Count;
                                                  };


            ConfirmationDialog.Confirmed += delegate(object data)
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
                                    UpdatePanel.Update(); // force refresh
                                };
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (StudyListGridView.IsPostBack)
            {
                LoadStudies();
            }
            
        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdateUI();
            base.OnPreRender(e);
        }

        protected void UpdateUI()
        {
            UpdateToolbarButtonState();
            
        }
        
        protected void FilterButton_Click(object sender, ImageClickEventArgs e)
        {
            StudyListGridView.StudyListGrid.ClearSelections();
        }

        protected void OnDeleteToolbarButtonClick(object sender, ImageClickEventArgs e)
        {
            IList<Study> studies = StudyListGridView.SelectedStudies;

            if (studies != null && studies.Count>0)
            {
                ConfirmationDialog.Message = string.Format("Are you sure you want to remove the following studies?<BR/>");
                ConfirmationDialog.Message += "<table>";
                foreach (Study study in studies)
                {
                    String text = String.Format("<tr align='left'><td>Patient:{0}&nbsp;&nbsp;</td><td>Accession:{1}&nbsp;&nbsp;</td><td>Description:{2}</td></tr>", 
                                    study.PatientsName, study.AccessionNumber, study.StudyDescription);
                    ConfirmationDialog.Message += text;
                }
                ConfirmationDialog.Message += "</table>";

                ConfirmationDialog.MessageType = ConfirmationDialog.MessageTypeEnum.YESNO;
                ConfirmationDialog.Data = studies;
                ConfirmationDialog.Show();
                
            }
        }

        protected void UpdateToolbarButtonState()
        {
            IList<Study> studies = StudyListGridView.SelectedStudies;
            if (studies != null)
            {
                OpenStudyToolbarButton.Enabled = true;
                
                DeleteToolbarButton.Enabled = true;
                foreach (Study study in studies)
                {
                    if (_controller.IsScheduledForDelete(study))
                    {
                        DeleteToolbarButton.Enabled = false;
                        break;
                    }
                }

            }
            else
            {
                OpenStudyToolbarButton.Enabled = false;
                DeleteToolbarButton.Enabled = false;
            }

        }


        #endregion Protected Methods
    }
}