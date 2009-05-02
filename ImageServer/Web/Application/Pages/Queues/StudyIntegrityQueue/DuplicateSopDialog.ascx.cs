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
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Utilities;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    
    //
    // Dialog for adding a new device or editting an existing device.
    //
    public partial class DuplicateSopDialog : UserControl
    {
        private const string HighlightCssClass = " ConflictField ";

        private delegate void ComparisonCallback(bool different);

        #region private variables

        // The server partitions that the new device can be associated with
        // This list will be determined by the user level permission.
        private IList<ServerPartition> _partitions = new List<ServerPartition>();

        private Model.StudyIntegrityQueue _item = null;
        private ReconcileDetails _details = null;

        #endregion

        #region public members

        /// <summary>
        /// Sets or gets the StudyIntegrity Item Value
        /// </summary>
        public Model.StudyIntegrityQueue StudyIntegrityQueueItem
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                ViewState[ "StudyIntegrityQueueItem"] = _item.GetKey();
            }
        }

        /// <summary>
        /// Sets or gets the Reconcile Item Value
        /// </summary>
        public ReconcileDetails ReconcileDetails
        {
            get { return _details; }
            set { 
                  _details = value;

                }
        }


        #endregion // public members

        #region Events

        #endregion Events

        #region Public delegates

        #endregion // public delegates

        #region Protected methods

        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            ServerEntityKey itemKey = ViewState[ "StudyIntegrityQueueItem"] as ServerEntityKey;
            StudyIntegrityQueueController controller = new StudyIntegrityQueueController();

            Close();
        }

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        

        #endregion Protected methods

        #region Public methods

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show()
        {
            DataBind();
            HighlightDifferences();
            Page.Validate();
            DuplicateSopReconcileModalDialog.Show();
        }

        public override void DataBind()
        {
            ExistingPatientSeriesGridView.DataSource = ReconcileDetails.ExistingStudy.Series;
            ConflictingPatientSeriesGridView.DataSource = ReconcileDetails.ConflictingImageSet.StudyInfo.Series;
            StudyStorage storage =
                StudyStorage.Load(HttpContextData.Current.ReadContext, this.StudyIntegrityQueueItem.StudyStorageKey);

            IList<StudyStorageLocation> studyLocations = StudyStorageLocation.FindStorageLocations(storage);
            StudyLocation.Text = studyLocations[0].GetStudyPath();

            String path = studyLocations[0].FilesystemPath;
            path = Path.Combine(path, studyLocations[0].PartitionFolder);
            path = Path.Combine(path, "Duplicate");
            path = Path.Combine(path, studyLocations[0].StudyInstanceUid);
            path = Path.Combine(path, StudyIntegrityQueueItem.GetKey().Key.ToString());
            
            DuplicateSopLocation.Text = path;
            base.DataBind();
        }

        private void HighlightDifferences()
        {
            if (ReconcileDetails!=null)
            {
                Compare(ReconcileDetails.ExistingStudy.Patient.Name, ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.Name, 
                   delegate(bool different)
                       {
                           Highlight(ConflictingNameLabel, different);
                       });

                Compare(ReconcileDetails.ExistingStudy.Patient.PatientID, ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientId,
                    delegate(bool different) { Highlight(ConflictingPatientIDLabel, different); });
                Compare(ReconcileDetails.ExistingStudy.Patient.IssuerOfPatientID, ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.IssuerOfPatientId,
                    delegate(bool different) { Highlight(ConflictingPatientIssuerOfPatientID, different); });
                Compare(ReconcileDetails.ExistingStudy.Patient.BirthDate, ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientsBirthdate,
                    delegate(bool different) { Highlight(ConflictingPatientBirthDate, different); });
                Compare(ReconcileDetails.ExistingStudy.Patient.Sex, ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.Sex,
                    delegate(bool different) { Highlight(ConflictingPatientSex, different); });
                Compare(ReconcileDetails.ExistingStudy.StudyDate, ReconcileDetails.ConflictingImageSet.StudyInfo.StudyDate,
                    delegate(bool different) { Highlight(ConflictingStudyDate, different); });
                Compare(ReconcileDetails.ExistingStudy.AccessionNumber, ReconcileDetails.ConflictingImageSet.StudyInfo.AccessionNumber,
                    delegate(bool different) { Highlight(ConflictingAccessionNumberLabel, different); });

            }
        }

        private void Highlight(WebControl control, bool highlight)
        {
            if (highlight)
                HtmlUtility.AddCssClass(control, HighlightCssClass);
            else
                HtmlUtility.RemoveCssClass(control, HighlightCssClass);
        }

        private static void Compare(string value1, string value2, ComparisonCallback del)
        {
            if (!StringUtils.AreEqual(value1, value2, StringComparison.InvariantCultureIgnoreCase))
                del(true);
            else
                del(false);
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            DuplicateSopReconcileModalDialog.Hide();
        }

        #endregion Public methods
    }
}
