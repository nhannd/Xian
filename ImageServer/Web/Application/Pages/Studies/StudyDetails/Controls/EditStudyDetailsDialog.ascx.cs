using System;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Xml;
using AjaxControlToolkit;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Security;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using ValueType=ClearCanvas.Dicom.Iod.ValueType;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class EditStudyDetailsDialog : System.Web.UI.UserControl
    {

        #region Public Properties

        /// <summary>
        /// Sets/Gets the current editing device.
        /// </summary>
        public Study study
        {
            set
            {
                // put into viewstate to retrieve later
                ViewState[ClientID + "_loadedStudy"] = value;
            }
            get
            { 
                // put into viewstate to retrieve later
                return ViewState[ClientID + "_loadedStudy"] as Study;
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

            PersonName oldPatientName = new PersonName(study.PatientsName);
            PersonName newPatientName = PatientNamePanel.PersonName;

            if (!oldPatientName.AreSame(newPatientName, PersonNameComparisonOptions.CaseInsensitive))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientsName, newPatientName.ToString()));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";", 
											 DicomTagDictionary.GetDicomTag(DicomTags.PatientsName).Name, 
											 newPatientName.ToString());
            }

            String dicomBirthDate = !(string.IsNullOrEmpty(PatientBirthDate.Text))
                                        ? DateTime.Parse(PatientBirthDate.Text).ToString(DicomConstants.DicomDate)
                                        : "";
            if (!study.PatientsBirthDate.Equals(dicomBirthDate))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientsBirthDate, dicomBirthDate));
				description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
											 DicomTagDictionary.GetDicomTag(DicomTags.PatientsBirthDate).Name,
											 dicomBirthDate);
			}

            if(study.PatientsAge == null || !PatientAge.Text.Equals(study.PatientsAge)) {
                string patientAge = PatientAge.Text.PadLeft(3,'0');
                patientAge += PatientAgePeriod.SelectedValue;

                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientsAge, patientAge));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
            	                             DicomTagDictionary.GetDicomTag(DicomTags.PatientsAge).Name,
            	                             patientAge);

            }

            if (!study.PatientsSex.Equals(PatientGender.Text))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientsSex, PatientGender.Text));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
            	                             DicomTagDictionary.GetDicomTag(DicomTags.PatientsSex).Name,
            	                             PatientGender.Text);
            }

            if (!study.PatientId.Equals(PatientID.Text))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientID, PatientID.Text));
				description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
									 DicomTagDictionary.GetDicomTag(DicomTags.PatientId).Name,
									 PatientID.Text);
			}

            if(String.IsNullOrEmpty(study.StudyDescription)
				|| !study.StudyDescription.Equals((StudyDescription.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.StudyDescription, StudyDescription.Text));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
            	                             DicomTagDictionary.GetDicomTag(DicomTags.StudyDescription).Name,
            	                             StudyDescription.Text);
			}

			if (String.IsNullOrEmpty(study.StudyId)
				|| !study.StudyId.Equals((StudyID.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.StudyID, StudyID.Text));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
            	                             DicomTagDictionary.GetDicomTag(DicomTags.StudyId).Name,
            	                             StudyID.Text);
			}

			if (String.IsNullOrEmpty(study.AccessionNumber)
				|| !study.AccessionNumber.Equals((AccessionNumber.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.AccessionNumber, AccessionNumber.Text));
            	description += string.Format("Tag=\"{0}\" Value=\"{1}\";",
            	                             DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber).Name,
            	                             AccessionNumber.Text);
			}

            PersonName oldPhysicianName = new PersonName(study.ReferringPhysiciansName);
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

            if(!study.StudyDate.Equals(dicomStudyDate))
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
            
            if(!study.StudyTime.Equals(dicomStudyTime))
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
            if(study == null) return;

            EditStudyModalDialog.Title = App_GlobalResources.Titles.EditStudyDialog;

            // Patient Information
            string dicomName = study.PatientsName;

            string[] splitDicomName = dicomName.Split('^');

            if (!study.PatientsSex.Equals(string.Empty))
            {
                switch(study.PatientsSex)
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

            PatientID.Text = study.PatientId;

            if (!string.IsNullOrEmpty(study.PatientsBirthDate))
            {
                DateTime? birthDate = DateParser.Parse(study.PatientsBirthDate);
                if (birthDate!=null)
                {
                    PatientBirthDate.Text = birthDate.Value.ToString(DateTimeFormatter.DefaultDateFormat);
                }
            }
            else
            {
                PatientBirthDate.Text = string.Empty;
            }


            if (!String.IsNullOrEmpty(study.PatientsAge))
            {
                PatientAge.Text = study.PatientsAge.Substring(0, 3).TrimStart('0');
                switch (study.PatientsAge.Substring(3))
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

            StudyDescription.Text = study.StudyDescription;            
            StudyID.Text = study.StudyId;
            AccessionNumber.Text = study.AccessionNumber;

            if (!string.IsNullOrEmpty(study.StudyDate))
            {
                DateTime? studyDate = DateParser.Parse(study.StudyDate);
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

            if (!string.IsNullOrEmpty(study.StudyTime))
            {
                DateTime? studyTime = TimeParser.Parse(study.StudyTime);
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

            PersonName patientName = new PersonName(study.PatientsName);
            PersonName physicianName = new PersonName(study.ReferringPhysiciansName);
            PatientNamePanel.PersonName = patientName;
            ReferringPhysicianNamePanel.PersonName = physicianName;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            //Set the culture settings
            CultureInfo oCulture = CultureInfo.CurrentCulture;
            PatientBirthDateMaskExtender.CultureName = oCulture.Name;
            PatientBirthDateCalendarExtender.Format = oCulture.DateTimeFormat.ShortDatePattern;

            //CalendarMaskExtender expects something like "99/99/9999"
            string Mask = PatientBirthDateCalendarExtender.Format.Replace(oCulture.DateTimeFormat.DateSeparator, "/");
            if (Mask.IndexOf("d") != Mask.LastIndexOf("d"))
            {
                Mask = Mask.Replace("d", "9");
            }
            else
            {
                Mask = Mask.Replace("d", "99");
            }
            if (Mask.IndexOf("M") != Mask.LastIndexOf("M"))
            {
                Mask = Mask.Replace("M", "9");
            }
            else
            {
                Mask = Mask.Replace("M", "99");
            }
            Mask = Mask.Replace("y", "9");
            PatientBirthDateMaskExtender.Mask = Mask;

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
                        studyController.EditStudy(study, modifiedFields);

                    	DicomInstancesAccessedAuditHelper helper =
                    		new DicomInstancesAccessedAuditHelper(ServerPlatform.AuditSource,
                    		                                      EventIdentificationTypeEventOutcomeIndicator.Success,
                    		                                      EventIdentificationTypeEventActionCode.U);
						helper.AddUser(new AuditPersonActiveParticipant(
									SessionManager.Current.Credentials.UserName,
									null,
									SessionManager.Current.Credentials.DisplayName));

                    	AuditStudyParticipantObject participant =
                    		new AuditStudyParticipantObject(study.StudyInstanceUid, study.AccessionNumber);
                    	participant.ParticipantObjectDetail = description;
						helper.AddStudyParticipantObject(participant);
                    	ServerPlatform.LogAuditMessage("DicomInstancesAccessedUpdate", helper);
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