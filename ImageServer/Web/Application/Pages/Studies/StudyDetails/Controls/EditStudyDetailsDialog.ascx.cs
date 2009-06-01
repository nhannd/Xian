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
using System.Xml;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Security;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class EditStudyDetailsDialog : System.Web.UI.UserControl
    {

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

        private XmlNode createChildNode(XmlNode setNode, string tagName, string value)
        {
            XmlNode clone = setNode.CloneNode(true);
            clone.Attributes.GetNamedItem("TagPath").InnerXml = tagName;
            clone.Attributes.GetNamedItem("Value").InnerXml = value;
            return clone;
        }

        private XmlDocument getChanges(out string description)
        {
        	description = string.Empty;
            XmlDocument changes = new XmlDocument();

            XmlElement rootNode = changes.CreateElement("editstudy");
            XmlElement setNode = changes.CreateElement("SetTag");
            setNode.SetAttribute("TagPath", "");
            setNode.SetAttribute("Value","");

            PersonName oldPatientName = new PersonName(Study.PatientsName);
            PersonName newPatientName = PatientNamePanel.PersonName;

            if (!oldPatientName.AreSame(newPatientName, PersonNameComparisonOptions.CaseInsensitive))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientsName, newPatientName.ToString()));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";", 
											 DicomTagDictionary.GetDicomTag(DicomTags.PatientsName).Name, 
											 newPatientName);
            }

            String dicomBirthDate = !(string.IsNullOrEmpty(PatientBirthDate.Text))
                                        ? DateTime.Parse(PatientBirthDate.Text).ToString(DicomConstants.DicomDate)
                                        : "";
            if (!Study.PatientsBirthDate.Equals(dicomBirthDate))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientsBirthDate, dicomBirthDate));
				description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
											 DicomTagDictionary.GetDicomTag(DicomTags.PatientsBirthDate).Name,
											 dicomBirthDate);
			}

            if(Study.PatientsAge == null || !PatientAge.Text.Equals(Study.PatientsAge)) {
                string patientAge = PatientAge.Text.PadLeft(3,'0');
                patientAge += PatientAgePeriod.SelectedValue;

                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientsAge, patientAge));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
            	                             DicomTagDictionary.GetDicomTag(DicomTags.PatientsAge).Name,
            	                             patientAge);

            }

            if (!Study.PatientsSex.Equals(PatientGender.Text))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientsSex, PatientGender.Text));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
            	                             DicomTagDictionary.GetDicomTag(DicomTags.PatientsSex).Name,
            	                             PatientGender.Text);
            }

            if (!Study.PatientId.Equals(PatientID.Text))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientID, PatientID.Text));
				description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
									 DicomTagDictionary.GetDicomTag(DicomTags.PatientId).Name,
									 PatientID.Text);
			}

            if(String.IsNullOrEmpty(Study.StudyDescription)
				|| !Study.StudyDescription.Equals((StudyDescription.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.StudyDescription, StudyDescription.Text));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
            	                             DicomTagDictionary.GetDicomTag(DicomTags.StudyDescription).Name,
            	                             StudyDescription.Text);
			}

			if (String.IsNullOrEmpty(Study.StudyId)
				|| !Study.StudyId.Equals((StudyID.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.StudyID, StudyID.Text));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
            	                             DicomTagDictionary.GetDicomTag(DicomTags.StudyId).Name,
            	                             StudyID.Text);
			}

			if (String.IsNullOrEmpty(Study.AccessionNumber)
				|| !Study.AccessionNumber.Equals((AccessionNumber.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.AccessionNumber, AccessionNumber.Text));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
            	                             DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber).Name,
            	                             AccessionNumber.Text);
			}

            PersonName oldPhysicianName = new PersonName(Study.ReferringPhysiciansName);
            PersonName newPhysicianName = ReferringPhysicianNamePanel.PersonName;

            if (!newPhysicianName.AreSame(oldPhysicianName, PersonNameComparisonOptions.CaseInsensitive))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.ReferringPhysician, newPhysicianName.ToString()));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
            	                             DicomTagDictionary.GetDicomTag(DicomTags.ReferringPhysiciansName).Name,
            	                             newPhysicianName);
			}

            String dicomStudyDate = !(string.IsNullOrEmpty(StudyDate.Text))
                                        ? DateTime.Parse(StudyDate.Text).ToString(DicomConstants.DicomDate)
                                        : "";

            if(!Study.StudyDate.Equals(dicomStudyDate))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.StudyDate, dicomStudyDate));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
            	                             DicomTagDictionary.GetDicomTag(DicomTags.StudyDate).Name,
            	                             dicomStudyDate);
			}

            int hh = StudyTimeAmPm.SelectedValue=="AM"? int.Parse(StudyTimeHours.Text)%12: 12+(int.Parse(StudyTimeHours.Text)%12) ;
            int mm = int.Parse(StudyTimeMinutes.Text);
            int ss = int.Parse(StudyTimeSeconds.Text);
            String dicomStudyTime = String.Format("{0:00}{1:00}{2:00}", hh, mm, ss);
            
            if(!Study.StudyTime.Equals(dicomStudyTime))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.StudyTime, dicomStudyTime));
				description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
											 DicomTagDictionary.GetDicomTag(DicomTags.StudyTime).Name,
											 dicomStudyTime);
			}

            changes.AppendChild(rootNode);

            return changes;
        }

        private void UpdateFields()
        {
            if(Study == null) return;

            PersonName patientName = new PersonName(Study.PatientsName);
            PersonName physicianName = new PersonName(Study.ReferringPhysiciansName);
            PatientNamePanel.PersonName = patientName;
            ReferringPhysicianNamePanel.PersonName = physicianName;
            
            // Patient Information
            if (!Study.PatientsSex.Equals(string.Empty))
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
                if (studyDate!=null)
                {
                    StudyDate.Text = studyDate.Value.ToString(DateTimeFormatter.DefaultDateFormat);
                }
                else
                {
                    StudyDate.Text = String.Empty;
                }
            } else
            {
                StudyDate.Text = String.Empty;
            }

            if (!string.IsNullOrEmpty(Study.StudyTime))
            {
                DateTime? studyTime = TimeParser.Parse(Study.StudyTime);
                if (studyTime!=null)
                {
                    if (studyTime.Value.Hour == 0)
                        StudyTimeHours.Text = "12";
                    else
                        StudyTimeHours.Text =
                            String.Format("{0:00}",studyTime.Value.Hour <= 12 ? studyTime.Value.Hour : studyTime.Value.Hour - 12);


                    StudyTimeMinutes.Text = String.Format("{0:00}", studyTime.Value.Minute);
                    StudyTimeSeconds.Text = String.Format("{0:00}", studyTime.Value.Second);

                    if (studyTime.Value.Hour < 12)
                        StudyTimeAmPm.SelectedValue = "AM";
                    else
                        StudyTimeAmPm.SelectedValue = "PM";
                }
                else
                {
                    // The time is invalid, display it in the boxes
                    StudyTimeHours.Text = "";
                    StudyTimeMinutes.Text = "";
                    StudyTimeSeconds.Text = "";
                    StudyTimeAmPm.SelectedValue = "AM";
                }

            }
            else
            {
                StudyTimeHours.Text = "12";
                StudyTimeMinutes.Text = "00";
                StudyTimeSeconds.Text = "00";
                StudyTimeAmPm.SelectedValue = "AM";
            }

            DataBind();
        }

        #endregion
        #region Protected Methods
        
        protected override void OnInit(EventArgs e)
        {
            SetupJavascript();
            EditStudyDetailsValidationSummary.HeaderText = App_GlobalResources.ErrorMessages.EditStudyValidationError;
            CalendarLink.ImageUrl = ImageServerConstants.ImageURLs.CalendarIcon;
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
                if (StudyEdited != null)
                {
                	string description;
                    XmlDocument modifiedFields = getChanges(out description);

                    if (modifiedFields.HasChildNodes)
                    {
                        StudyController studyController = new StudyController();
                        studyController.EditStudy(Study, modifiedFields);

                    	DicomInstancesAccessedAuditHelper helper =
                    		new DicomInstancesAccessedAuditHelper(ServerPlatform.AuditSource,
                    		                                      EventIdentificationTypeEventOutcomeIndicator.Success,
                    		                                      EventIdentificationTypeEventActionCode.U);
						helper.AddUser(new AuditPersonActiveParticipant(
									SessionManager.Current.Credentials.UserName,
									null,
									SessionManager.Current.Credentials.DisplayName));

                    	AuditStudyParticipantObject participant =
                    		new AuditStudyParticipantObject(Study.StudyInstanceUid, Study.AccessionNumber);
                    	participant.ParticipantObjectDetail = description;
						helper.AddStudyParticipantObject(participant);
                    	ServerPlatform.LogAuditMessage(helper);
                    }

                    StudyEdited();
                }

                Close();
            }
            else
            {
                Show(false);
            }
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