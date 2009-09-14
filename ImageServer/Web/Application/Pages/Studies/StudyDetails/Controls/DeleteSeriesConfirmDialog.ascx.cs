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
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    [Serializable]
    public class DeleteSeriesInfo
    {
        private Study _study;
        private Series _series;
        private ServerEntityKey _studyKey;
        private string _serverPartitionAE;
        private string _seriesNumber;
        private string _modality;
        private string _description;
        private int _numberOfRelatedSeries;
        private string _seriesInstanceUid;
        private string _performedProcedureStepStartDate;
        private string _performedProcedureStepStartTime;

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        public string SeriesNumber
        {
            get { return _seriesNumber; }
            set { _seriesNumber = value; }
        }

        public string Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public int NumberOfSeriesRelatedInstances
        {
            get { return _numberOfRelatedSeries; }
            set { _numberOfRelatedSeries = value; }
        }

        public string PerformedProcedureStepStartDate
        {
            get { return _performedProcedureStepStartDate; }
            set { _performedProcedureStepStartDate = value; }
        }

        public string PerformedProcedureStepStartTime
        {
            get { return _performedProcedureStepStartTime; }
            set { _performedProcedureStepStartTime = value; }
        }

        public ServerEntityKey StudyKey
        {
            get { return _studyKey; }
            set { _studyKey = value; }
        }

        public string ServerPartitionAE
        {
            get { return _serverPartitionAE; }
            set { _serverPartitionAE = value; }
        }
    }

    public class DeleteSeriesConfirmDialogSeriesDeletingEventArgs : EventArgs
    {
        private IList<DeleteSeriesInfo> _deletingSeries;
        private string _reasonForDeletion;

        public IList<DeleteSeriesInfo> DeletingSeries
        {
            get { return _deletingSeries; }
            set { _deletingSeries = value; }
        }

        public string ReasonForDeletion
        {
            get { return _reasonForDeletion; }
            set { _reasonForDeletion = value; }
        }
    }

    public class DeleteSeriesConfirmDialogSeriesDeletedEventArgs:EventArgs
    {
        private IList<DeleteSeriesInfo> _deletedSeries;
        private string _reasonForDeletion;

        public IList<DeleteSeriesInfo> DeletedSeries
        {
            get { return _deletedSeries; }
            set { _deletedSeries = value; }
        }

        public string ReasonForDeletion
        {
            get { return _reasonForDeletion; }
            set { _reasonForDeletion = value; }
        }
    }

    public partial class DeleteSeriesConfirmDialog : UserControl
    {
        private const string REASON_CANNEDTEXT_CATEGORY = "DeleteSeriesReason";
        private EventHandler<DeleteSeriesConfirmDialogSeriesDeletingEventArgs> _seriesDeletingHandler;
        private EventHandler<DeleteSeriesConfirmDialogSeriesDeletedEventArgs> _seriesDeletedHandler;

        public event EventHandler<DeleteSeriesConfirmDialogSeriesDeletingEventArgs> SeriesDeleting
        {
            add { _seriesDeletingHandler += value; }
            remove { _seriesDeletingHandler -= value; }
        }
        
        public event EventHandler<DeleteSeriesConfirmDialogSeriesDeletedEventArgs> SeriesDeleted
        {
            add { _seriesDeletedHandler += value; }
            remove { _seriesDeletedHandler -= value; }
        }
        
        public IList<DeleteSeriesInfo> DeletingSeries
        {
            get
            {
                return ViewState["DeletedSeries"] as IList<DeleteSeriesInfo>;
            }
            set { ViewState["DeletedSeries"] = value; }
        }

        public bool DeleteEntireStudy
        {
            get
            {
                if (ViewState["DeleteEntireStudy"] == null) return false;
                bool deleteEntireStudy;
                Boolean.TryParse(ViewState["DeleteEntireStudy"].ToString(), out deleteEntireStudy);
                return deleteEntireStudy;
            }
            set { ViewState["DeleteEntireStudy"] = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (DeleteEntireStudy) DeleteEntireStudyLabel.Visible = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!SessionManager.Current.User.IsInRole(Enterprise.Authentication.AuthorityTokens.Study.SaveReason))
            {
                ReasonSavePanel.Visible = false;
            }
        }

        private void ClearInputs()
        {
            Reason.Text = "";
            SaveReasonAsName.Text = "";
            ReasonListBox.Items.Clear();
        }

        public override void DataBind()
        {
            SeriesListing.DataSource = DeletingSeries;

            EnsurePredefinedReasonsLoaded();
            
            base.DataBind();
        }

        private void EnsurePredefinedReasonsLoaded()
        {
            if (ReasonListBox.Items.Count == 0)
            {
                IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

                ICannedTextEntityBroker broker = HttpContextData.Current.ReadContext.GetBroker<ICannedTextEntityBroker>();
                CannedTextSelectCriteria criteria = new CannedTextSelectCriteria();
                criteria.Category.EqualTo(REASON_CANNEDTEXT_CATEGORY);
                IList<CannedText> list = broker.Find(criteria);

                ReasonListBox.Items.Add(new ListItem("-- Select one --", ""));
                foreach (CannedText text in list)
                {
                    ReasonListBox.Items.Add(new ListItem(text.Label, text.Text));
                }
                
            }
        }

        protected void DeleteSeriesButton_Clicked(object sender, ImageClickEventArgs e)
        {

            if (Page.IsValid)
            {
                try
                {
                    if (!String.IsNullOrEmpty(SaveReasonAsName.Text))
                    {
                        SaveCustomReason();
                    }

                    OnDeletingSeries();
                    StudyController controller = new StudyController();

                    if(DeleteEntireStudy)
                    {
                        try
                        {
                            Study study = DeletingSeries[0].Study;

                            controller.DeleteStudy(DeletingSeries[0].StudyKey, ReasonListBox.SelectedItem.Text + "::" + Reason.Text);

                            // Audit log
                            DicomStudyDeletedAuditHelper helper =
                                new DicomStudyDeletedAuditHelper(
                                    ServerPlatform.AuditSource,
                                    EventIdentificationTypeEventOutcomeIndicator.Success);
                            helper.AddUserParticipant(new AuditPersonActiveParticipant(
                                                          SessionManager.Current.Credentials.
                                                              UserName,
                                                          null,
                                                          SessionManager.Current.Credentials.
                                                              DisplayName));
                            helper.AddStudyParticipantObject(new AuditStudyParticipantObject(
                                                                 study.StudyInstanceUid,
                                                                 study.AccessionNumber ??
                                                                 string.Empty));
                            ServerPlatform.LogAuditMessage(helper);
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, ex, "DeletSerieseClicked failed: Unable to delete studies");
                            throw;
                        }

                    } 
                    else
                    {
                        try
                        {
                            IList<Series> series = new List<Series>();
                            foreach (DeleteSeriesInfo seriesInfo in DeletingSeries)
                            {
                                series.Add((seriesInfo.Series));
                            }
                            controller.DeleteSeries(DeletingSeries[0].Study, series, Reason.Text);
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, ex, "DeletSerieseClicked failed: Unable to delete studies");
                            throw;
                        }
                    }

                    OnSeriesDeleted();
                }
                finally
                {
                    Close();
                }
            }
            else
            {
                EnsureDialogVisible();
            }
        }

        private void SaveCustomReason()
        {
            if (ReasonListBox.Items.FindByText(SaveReasonAsName.Text)!=null)
            {
                // update
                StudyDeleteReasonAdaptor adaptor = new StudyDeleteReasonAdaptor();
                CannedTextSelectCriteria criteria = new CannedTextSelectCriteria();
                criteria.Label.EqualTo(SaveReasonAsName.Text);
                criteria.Category.EqualTo(REASON_CANNEDTEXT_CATEGORY);
                IList<CannedText> reasons = adaptor.Get(criteria);
                foreach(CannedText reason in reasons)
                {
                    CannedTextUpdateColumns rowColumns = new CannedTextUpdateColumns();
                    rowColumns.Text = Reason.Text;
                    adaptor.Update(reason.Key, rowColumns);
                }
                
            }
            else
            {
                // add 
                StudyDeleteReasonAdaptor adaptor = new StudyDeleteReasonAdaptor();
                CannedTextUpdateColumns rowColumns = new CannedTextUpdateColumns();
                rowColumns.Category = REASON_CANNEDTEXT_CATEGORY;
                rowColumns.Label = SaveReasonAsName.Text;
                rowColumns.Text = Reason.Text;
                adaptor.Add(rowColumns);
            }
            
        }

        protected void CancelButton_Clicked(object sender, ImageClickEventArgs e)
        {
            Close();
        }

        private void OnSeriesDeleted()
        {
            DeleteSeriesConfirmDialogSeriesDeletedEventArgs args = new DeleteSeriesConfirmDialogSeriesDeletedEventArgs();
            args.DeletedSeries = DeletingSeries;
            args.ReasonForDeletion = Reason.Text;
            EventsHelper.Fire(_seriesDeletedHandler, this, args);
        }

        private void OnDeletingSeries()
        {
            DeleteSeriesConfirmDialogSeriesDeletingEventArgs args = new DeleteSeriesConfirmDialogSeriesDeletingEventArgs();
            args.DeletingSeries = DeletingSeries;
            args.ReasonForDeletion = Reason.Text;
            EventsHelper.Fire(_seriesDeletingHandler, this, args);
        }

        internal void EnsureDialogVisible()
        {
            ModalDialog.Show();
        }

        public void Close()
        {
            ModalDialog.Hide();
        }

        public void Initialize(List<DeleteSeriesInfo> list)
        {
            ClearInputs(); 
            DeletingSeries = list;
        }

        internal void Show()
        {
            DataBind();
            EnsureDialogVisible();
        }
    }
}