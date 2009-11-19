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
using System.Globalization;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Security;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{


    public partial class EditStudyDetailsDialog : System.Web.UI.UserControl
    {
        private const string REASON_CANNEDTEXT_CATEGORY = "EditStudyReason";


        #region Public Properties

        /// <summary>
        /// Sets/Gets the current editing device.
        /// </summary>
        public Study Study
        {
            set
            {
                // put into viewstate to retrieve later
                ViewState["loadedStudy"] = value;
            }
            get
            { 
                // put into viewstate to retrieve later
                return ViewState["loadedStudy"] as Study;
            }
        }

        #endregion
        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="EditStudyDetailsDialog.StudyEdited"/>.
        /// </summary>
        public delegate void OnOKClickedEventHandler();

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler StudyEdited;

        #endregion Events
        #region Private Methods

        private void SetupJavascript()
        {
            ClearStudyDateTimeButton.OnClientClick = "document.getElementById('" + StudyDate.ClientID + "').value='';" +
                                         "document.getElementById('" + StudyTimeHours.ClientID + "').value='';" +
                                         "document.getElementById('" + StudyTimeMinutes.ClientID + "').value='';" +
                                         "document.getElementById('" + StudyTimeSeconds.ClientID + "').value='';" +
                                         " return false;";

            ClearPatientBirthDateButton.OnClientClick = "document.getElementById('" + PatientBirthDate.ClientID +
                                                        "').value=''; return false;";

        }

        private static bool AreDifferent(String v1, String v2)
        {
            if (v1 == null)
                return String.IsNullOrEmpty(v2) == false;

            //v1 is not null
            return !v1.Equals(v2);
        }

        private List<UpdateItem> GetChanges()
        {
            var changes = new List<UpdateItem>();
            var oldPatientName = new PersonName(Study.PatientsName);
            var newPatientName = PatientNamePanel.PersonName;

            if (!oldPatientName.AreSame(newPatientName, PersonNameComparisonOptions.CaseInsensitive))
            {
                var item = new UpdateItem(DicomTags.PatientsName, Study.PatientsName, PatientNamePanel.PersonName);
                changes.Add(item);
            }

            String dicomBirthDate = !(string.IsNullOrEmpty(PatientBirthDate.Text))
                                        ? DateTime.ParseExact(PatientBirthDate.Text, UISettings.Default.InputDateFormat, null).ToString(DicomConstants.DicomDate)
                                        : "";
            if (AreDifferent(Study.PatientsBirthDate, dicomBirthDate))
            {
                var item = new UpdateItem(DicomTags.PatientsBirthDate, Study.PatientsBirthDate, dicomBirthDate);
                changes.Add(item);
            }

            string newPatientAge = String.IsNullOrEmpty(PatientAge.Text)? String.Empty:String.Format("{0}{1}", PatientAge.Text.PadLeft(3, '0'), PatientAgePeriod.SelectedValue);

            if (AreDifferent(Study.PatientsAge, newPatientAge))
            {
                var item = new UpdateItem(DicomTags.PatientsAge, Study.PatientsAge, newPatientAge);
                changes.Add(item);
            }

            // PatientGender is a required field.
            if (AreDifferent(Study.PatientsSex, PatientGender.Text))
            {
                var item = new UpdateItem(DicomTags.PatientsSex, Study.PatientsSex, PatientGender.Text);
                changes.Add(item);
            }

            //PatientID.Text is a required field.
            if (AreDifferent(Study.PatientId, PatientID.Text))
            {
                var item = new UpdateItem(DicomTags.PatientId, Study.PatientId, PatientID.Text);
                changes.Add(item);
            }

            if (AreDifferent(Study.StudyDescription, StudyDescription.Text))
            {
                var item = new UpdateItem(DicomTags.StudyDescription, Study.StudyDescription, StudyDescription.Text);
                changes.Add(item);
            }

            if (AreDifferent(Study.StudyId, StudyID.Text))
            {
                var item = new UpdateItem(DicomTags.StudyId, Study.StudyId, StudyID.Text);
                changes.Add(item);
            }

            if (AreDifferent(Study.AccessionNumber, AccessionNumber.Text))
            {
                var item = new UpdateItem(DicomTags.AccessionNumber, Study.AccessionNumber, AccessionNumber.Text);
                changes.Add(item);
            }

            var oldPhysicianName = new PersonName(Study.ReferringPhysiciansName);
            var newPhysicianName = ReferringPhysicianNamePanel.PersonName;

            if (!newPhysicianName.AreSame(oldPhysicianName, PersonNameComparisonOptions.CaseInsensitive))
            {
                var item = new UpdateItem(DicomTags.ReferringPhysiciansName, Study.ReferringPhysiciansName, ReferringPhysicianNamePanel.PersonName.ToString());
                changes.Add(item);
            }

            var newDicomStudyDate = !(string.IsNullOrEmpty(StudyDate.Text))
                                        ? DateTime.Parse(StudyDate.Text).ToString(DicomConstants.DicomDate)
                                        : "";

            if (AreDifferent(Study.StudyDate, newDicomStudyDate))
            {
                var item = new UpdateItem(DicomTags.StudyDate, Study.StudyDate, newDicomStudyDate);
                changes.Add(item);
            }

            int hh = int.Parse(StudyTimeHours.Text);
            int mm = int.Parse(StudyTimeMinutes.Text);
            int ss = int.Parse(StudyTimeSeconds.Text);
            String dicomStudyTime = String.Format("{0:00}{1:00}{2:00}", hh, mm, ss);

            if (AreDifferent(Study.StudyTime, dicomStudyTime))
            {
                var item = new UpdateItem(DicomTags.StudyTime, Study.StudyTime, dicomStudyTime);
                changes.Add(item);
            }

            return changes;
        }

        private void UpdateFields()
        {
            if(Study == null) return;

            var patientName = new PersonName(Study.PatientsName);
            var physicianName = new PersonName(Study.ReferringPhysiciansName);
            PatientNamePanel.PersonName = patientName;
            ReferringPhysicianNamePanel.PersonName = physicianName;
            
            // Patient Information
            if (!string.IsNullOrEmpty(Study.PatientsSex))
            {
                switch(Study.PatientsSex)
                {
                    case "M":
                        PatientGender.SelectedIndex = 1;
                        break;
                    case "F":
                        PatientGender.SelectedIndex = 2;
                        break;
                    case "O":
                        PatientGender.SelectedIndex = 3;
                        break;
                    default:
                        PatientGender.SelectedIndex = 0;
                        break;
                }
            } else
            {
                PatientGender.SelectedIndex = 0;
            }

            PatientID.Text = Study.PatientId;
            DateTime? birthDate = String.IsNullOrEmpty(Study.PatientsBirthDate)? null:DateParser.Parse(Study.PatientsBirthDate);
            PatientBirthDateCalendarExtender.SelectedDate = birthDate;
            if (birthDate == null)
                PatientBirthDate.Text = String.Empty; // calendar fills in the default date if it's null, we don't want that to happen.

            if (!String.IsNullOrEmpty(Study.PatientsAge))
            {
                PatientAge.Text = Study.PatientsAge.Substring(0, 3).TrimStart('0');
                switch (Study.PatientsAge.Substring(3))
                {
                    case "Y":
                        PatientAgePeriod.SelectedIndex = 0;
                        break;
                    case "M":
                        PatientAgePeriod.SelectedIndex = 1;
                        break;
                    case "W":
                        PatientAgePeriod.SelectedIndex = 2;
                        break;
                    default:
                        PatientAgePeriod.SelectedIndex = 3;
                        break;
                }
            }
            else
            {
                PatientAge.Text = string.Empty;
                PatientAgePeriod.SelectedIndex = 0;
            }

            // Study Information
            StudyDescription.Text = Study.StudyDescription;            
            StudyID.Text = Study.StudyId;
            AccessionNumber.Text = Study.AccessionNumber;

            if (!string.IsNullOrEmpty(Study.StudyDate))
            {
                DateTime? studyDate = DateParser.Parse(Study.StudyDate);
                StudyDate.Text = studyDate!=null ? studyDate.Value.ToString(DateTimeFormatter.DefaultDateFormat) : String.Empty;
            }
            else
            {
                StudyDate.Text = String.Empty;
            }

            if (!string.IsNullOrEmpty(Study.StudyTime))
            {
                DateTime? studyTime = TimeParser.Parse(Study.StudyTime);
                if (studyTime!=null)
                {
                    StudyTimeHours.Text = studyTime.Value.Hour == 0 ? "12" : String.Format("{0:00}",studyTime.Value.Hour <= 12 ? studyTime.Value.Hour : studyTime.Value.Hour - 12);

                    StudyTimeMinutes.Text = String.Format("{0:00}", studyTime.Value.Minute);
                    StudyTimeSeconds.Text = String.Format("{0:00}", studyTime.Value.Second);
                }
                else
                {
                    // The time is invalid, display it in the boxes
                    StudyTimeHours.Text = "";
                    StudyTimeMinutes.Text = "";
                    StudyTimeSeconds.Text = "";
                }

            }
            else
            {
                StudyTimeHours.Text = "12";
                StudyTimeMinutes.Text = "00";
                StudyTimeSeconds.Text = "00";
            }

            DataBind();
        }

        private void EnsurePredefinedReasonsLoaded()
        {
            if (ReasonListBox.Items.Count == 0)
            {
                var broker = HttpContextData.Current.ReadContext.GetBroker<ICannedTextEntityBroker>();
                var criteria = new CannedTextSelectCriteria();
                criteria.Category.EqualTo(REASON_CANNEDTEXT_CATEGORY);
                IList<CannedText> list = broker.Find(criteria);

                ReasonListBox.Items.Add(new ListItem("-- Select one --", ""));
                foreach (CannedText text in list)
                {
                    ReasonListBox.Items.Add(new ListItem(text.Label, text.Text));
                }

            }
        }

        private static void AuditLog(Study study, List<UpdateItem> fields)
        {
            Platform.CheckForNullReference(study, "study");
            Platform.CheckForNullReference(fields, "fields");

            var helper =
                new DicomInstancesAccessedAuditHelper(ServerPlatform.AuditSource,
                                                      EventIdentificationTypeEventOutcomeIndicator.Success,
                                                      EventIdentificationTypeEventActionCode.U);
            helper.AddUser(new AuditPersonActiveParticipant(
                               SessionManager.Current.Credentials.UserName,
                               null,
                               SessionManager.Current.Credentials.DisplayName));

            var participant = new AuditStudyParticipantObject(study.StudyInstanceUid, study.AccessionNumber);

            string updateDescription = StringUtilities.Combine(
                fields, ";",
                item => String.Format("Tag=\"{0}\" Value=\"{1}\"", item.DicomTag.Name, item.Value)
                );

            participant.ParticipantObjectDetail = updateDescription;
            helper.AddStudyParticipantObject(participant);
            ServerPlatform.LogAuditMessage(helper);
        }

        private void SaveCustomReason()
        {
            if (ReasonListBox.Items.FindByText(SaveReasonAsName.Text) != null)
            {
                // update
                var adaptor = new StudyEditReasonAdaptor();
                var criteria = new CannedTextSelectCriteria();
                criteria.Label.EqualTo(SaveReasonAsName.Text);
                criteria.Category.EqualTo(REASON_CANNEDTEXT_CATEGORY);
                IList<CannedText> reasons = adaptor.Get(criteria);
                foreach (CannedText reason in reasons)
                {
                    var rowColumns = new CannedTextUpdateColumns {Text = Reason.Text};
                    adaptor.Update(reason.Key, rowColumns);
                }

            }
            else
            {
                // add 
                var adaptor = new StudyDeleteReasonAdaptor();
                var rowColumns = new CannedTextUpdateColumns
                                     {
                                         Category = REASON_CANNEDTEXT_CATEGORY,
                                         Label = SaveReasonAsName.Text,
                                         Text = Reason.Text
                                     };
                adaptor.Add(rowColumns);
            }

        }

        #endregion
        #region Protected Methods
       
        protected override void OnInit(EventArgs e)
        {
            SetupJavascript();
            EditStudyDetailsValidationSummary.HeaderText = App_GlobalResources.ErrorMessages.EditStudyValidationError;
            CalendarLink.ImageUrl = ImageServerConstants.ImageURLs.CalendarIcon;
            EnsurePredefinedReasonsLoaded();

            if (!SessionManager.Current.User.IsInRole(Enterprise.Authentication.AuthorityTokens.Study.SaveReason))
            {
                ReasonSavePanel.Visible = false;
            }

            //The mask doesn't work well if the Date separator isn't "/", so disable it.
            //DateValidator will handle invalid date values if the mask is disabled.
            if(!UISettings.Default.InputDateFormat.Contains("/"))
            {
                PatientBirthDateMaskExtender.Enabled = false;
            } else
            {
                //Set the mask to be the format of the ShortDatePattern, but with 9's.
                PatientBirthDateMaskExtender.Mask = UISettings.Default.InputDateFormat.Replace("d", "9").Replace("M", "9").Replace(
                        "y", "9");
                PatientBirthDateMaskExtender.MaskType = AjaxControlToolkit.MaskedEditType.Date;
            }
            
            PatientBirthDateCalendarExtender.Format = UISettings.Default.InputDateFormat;
            StudyDateValidator.DateFormat = UISettings.Default.DateFormat;
        }

        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (!String.IsNullOrEmpty(SaveReasonAsName.Text))
                {
                    SaveCustomReason();
                }
                
                if (StudyEdited != null)
                {
                    List<UpdateItem> modifiedFields = GetChanges();
                    if (modifiedFields!=null && modifiedFields.Count > 0)
                    {
                        var studyController = new StudyController();
                        studyController.EditStudy(Study, modifiedFields, ReasonListBox.SelectedItem.Text + "::" + Reason.Text);
                        AuditLog(Study, modifiedFields);
                        StudyEdited();
                    }
                }

                Close();
            }
            else
            {
                EnsureDialogVisible();
            }
        }

        internal void EnsureDialogVisible()
        {
            EditStudyModalDialog.Show();
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

        #endregion 

        #region Public Methods

        /// <summary>
        /// Displays the edit Study Details dialog box.
        /// </summary>
        public void Show(bool updateFields)
        {
            if(updateFields) UpdateFields();
            EditStudyModalDialog.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            EditStudyModalDialog.Hide();
        }

        #endregion
    }
}