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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;


[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.StudyDetailsPanel.js", "application/x-javascript")]


namespace ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails
{
    /// <summary>
    /// Main panel within the <see cref="StudyDetailsPage"/>
    /// </summary>
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.StudyDetailsPanel",
                          ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.StudyDetailsPanel.js")]
    public partial class StudyDetailsPanel : ScriptUserControl
    {
        #region Private Members
        private Study _study;
        #endregion Private Members


        #region Public Properties

        /// <summary>
        /// Sets or gets the displayed study
        /// </summary>
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

       
        #endregion Public Properties


        #region Protected Methods

        [ExtenderControlProperty]
        [ClientPropertyName("ViewSeriesButtonClientID")]
        public string ViewSeriesButtonClientID
        {
            get { return this.ViewSeriesButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("SeriesListClientID")]
        public string SeriesListClientID
        {
            get { return SeriesGridView.SeriesListControl.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("OpenSeriesPageUrl")]
        public string OpenSeriesPageUrl
        {
            get { return Page.ResolveClientUrl("~/Pages/SeriesDetails/SeriesDetailsPage.aspx"); }
        }
        

        public StudyDetailsPanel()
            : base(false, HtmlTextWriterTag.Div)
            {
            }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ConfirmDialog.Confirmed += new ConfirmationDialog.ConfirmedEventHandler(ConfirmDialog_Confirmed);
        }

        public override void DataBind()
        {
            // setup the data for the child controls
            if (Study != null)
            {
                ServerPartitionDataAdapter adaptor = new ServerPartitionDataAdapter();
                ServerPartition partition = adaptor.Get(Study.ServerPartitionKey);

                PatientSummaryPanel.PatientSummary = PatientSummaryAssembler.CreatePatientSummary(Study);

                StudyDetailsView.Studies.Add(Study);
             
                SeriesGridView.Partition = partition;
                SeriesGridView.Study = Study;
            } 
            
            base.DataBind();

        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdateUI(); 
            
            base.OnPreRender(e);
        }

        protected void UpdateUI()
        {
            if (Study!=null)
            {
                StudyController controller = new StudyController();
                bool scheduledForDelete = controller.IsScheduledForDelete(Study);

                DeleteStudyButton.Enabled = !scheduledForDelete;

                if (scheduledForDelete)
                {
                    ShowScheduledForDeleteAlert();
                }
                else
                {
                    MessagePanel.Visible = false;
                }


                int[] selectedSeriesIndices = SeriesGridView.SeriesListControl.SelectedIndices;
                ViewSeriesButton.Enabled = selectedSeriesIndices != null && selectedSeriesIndices.Length > 0;             
            }
           
        }

        protected void DeleteStudyButton_Click(object sender, EventArgs e)
        {
            ConfirmDialog.MessageType = ConfirmationDialog.MessageTypeEnum.YESNO;
            ConfirmDialog.Message = App_GlobalResources.SR.SingleStudyDelete;
            ConfirmDialog.Data = Study;

            ConfirmDialog.Show();
        }

        #endregion Protected Methods

        #region Private Methods

        private void ConfirmDialog_Confirmed(object data)
        {
            StudyController controller = new StudyController();

            Study study = ConfirmDialog.Data as Study;
            if (controller.DeleteStudy(study))
            {
                DeleteStudyButton.Enabled = false;

                ShowScheduledForDeleteAlert();
                
            }
            else
            {
                throw new Exception("Unable to delete the study. See server log for more details");

            } 
        }

        private void ShowScheduledForDeleteAlert()
        {
            MessagePanel.Visible = true;
            ConfirmationMessage.Text = App_GlobalResources.SR.StudyScheduledForDeletion;               
            UpdatePanel1.Update();
        }


        #endregion Private Methods
    }
}