using System;
using System.Web.UI.WebControls;
using System.Xml;
using AjaxControlToolkit;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class EditStudyDetailsDialog : System.Web.UI.UserControl
    {
        private Study _study;

        #region Public Properties

        /// <summary>
        /// Sets/Gets the current editing device.
        /// </summary>
        public Study study
        {
            set
            {
                _study = value;
                // put into viewstate to retrieve later
                ViewState[ClientID + "_loadedStudy"] = _study;
            }
            get { return _study; }
        }

        #endregion
        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        public delegate void OnOKClickedEventHandler();

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler OKClicked;

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
                                                        "').value='';" +
                                                        "document.getElementById('" + PatientAge.ClientID +
                                                        "').value=''; return false;";

            ChangeStudyInstanceUIDButton.OnClientClick = "document.getElementById('" + StudyInstanceUID.ClientID +
                                                        "').value='" + DicomUid.GenerateUid().UID + "'; return false;";

            Page.ClientScript.RegisterStartupScript(GetType(), "changeAge", @"function changeAge() {" +
                                                                            "var today=new Date();" +
                                                                            "var birthDate=new Date(document.getElementById('" + PatientBirthDate.ClientID + "').value);" +
                                                                            "var diff=today-birthDate;" +
                                                                            "diff=Math.round(diff/1000/60/60/24);" +
                                                                            "if(diff < 365) { " +
                                                                            "var iMonths = Math.round(diff/30);" +
                                                                            "if(iMonths < 12 && iMonths > 0) { " +
                                                                            "document.getElementById('" + PatientAge.ClientID + "').value=iMonths + ' Month(s)'; " +
                                                                            "} else { " +
                                                                            "document.getElementById('" + PatientAge.ClientID + "').value=diff + ' Day(s)'; " +
                                                                            "} } else { var iYear = Math.floor(diff/365);" +
                                                                            "document.getElementById('" + PatientAge.ClientID + "').value=iYear + ' Year(s)';} }", true);

        }

        private XmlNode createChildNode(XmlNode setNode, string tagName, string value)
        {
            XmlNode clone = setNode.CloneNode(true);
            clone.Attributes.GetNamedItem("TagPath").InnerXml = tagName;
            clone.Attributes.GetNamedItem("Value").InnerXml = value;
            return clone;
        }

        private XmlDocument getChanges()
        {
            XmlDocument changes = new XmlDocument();

            XmlElement rootNode = changes.CreateElement("editstudy");
            XmlElement setNode = changes.CreateElement("SetTag");
            setNode.SetAttribute("TagPath", "");
            setNode.SetAttribute("Value","");

            PersonName oldPatientName = new PersonName(_study.PatientsName);
            PersonName newPatientName = PatientNamePanel.PersonName;

            if (!oldPatientName.AreSame(newPatientName, PersonNameComparisonOptions.CaseInsensitive))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientsName, newPatientName.ToString()));
            }

            String dicomBirthDate = !(string.IsNullOrEmpty(PatientBirthDate.Text))
                                        ? DateTime.Parse(PatientBirthDate.Text).ToString(DicomConstants.DicomDate)
                                        : "";
            if (!study.PatientsBirthDate.Equals(dicomBirthDate))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientsBirthDate, dicomBirthDate));
            }

            if (!study.PatientsSex.Equals(PatientGender.Text))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientsSex, PatientGender.Text));
            }

            if (!study.PatientId.Equals(PatientID.Text))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.PatientID, PatientID.Text));
            }

            if(String.IsNullOrEmpty(study.StudyDescription)
				|| !study.StudyDescription.Equals((StudyDescription.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.StudyDescription, StudyDescription.Text));
            }

            if(!study.StudyInstanceUid.Equals((StudyInstanceUID.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.StudyInstanceUID, StudyInstanceUID.Text));
            }

			if (String.IsNullOrEmpty(study.StudyId)
				|| !study.StudyId.Equals((StudyID.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.StudyID, StudyID.Text));
            }

			if (String.IsNullOrEmpty(study.AccessionNumber)
				|| !study.AccessionNumber.Equals((AccessionNumber.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.AccessionNumber, AccessionNumber.Text));
            }

            PersonName oldPhysicianName = new PersonName(_study.ReferringPhysiciansName);
            PersonName newPhysicianName = ReferringPhysicianNamePanel.PersonName;

            if (!newPhysicianName.AreSame(oldPhysicianName, PersonNameComparisonOptions.CaseInsensitive))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.ReferringPhysician, newPhysicianName.ToString()));
            }

            String dicomStudyDate = !(string.IsNullOrEmpty(StudyDate.Text))
                                        ? DateTime.Parse(StudyDate.Text).ToString(DicomConstants.DicomDate)
                                        : "";

            if(!study.StudyDate.Equals(dicomStudyDate))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.StudyDate, dicomStudyDate));
            }


            if (StudyTimeAmPm.SelectedValue.Equals("PM") || (StudyTimeAmPm.SelectedValue.Equals("AM") && StudyTimeHours.Text.Equals("12"))) StudyTimeHours.Text = (Int16.Parse(StudyTimeHours.Text) + 12).ToString();
            string dicomStudyTime = StudyTimeHours.Text.PadLeft(2, '0') + StudyTimeMinutes.Text.PadLeft(2, '0') + StudyTimeSeconds.Text.PadLeft(2, '0');
            if(!study.StudyTime.Equals(dicomStudyTime))
            {
                rootNode.AppendChild(createChildNode(setNode, DicomConstants.DicomTags.StudyTime, dicomStudyTime));
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

                    TimeSpan age = DateTime.Now - birthDate.Value;
                    if (age > TimeSpan.FromDays(365))
                    {
                        PatientAge.Text = String.Format("{0:0}", age.TotalDays / 365);
                    }
                    else if (age > TimeSpan.FromDays(30))
                    {
                        PatientAge.Text = String.Format("{0:0} month(s)", age.TotalDays / 30);
                    }
                    else
                    {
                        PatientAge.Text = String.Format("{0:0} day(s)", age.TotalDays);
                    }
                }
            }
            else
            {
                PatientBirthDate.Text = string.Empty;
                PatientAge.Text = string.Empty;
            }
            
            // Study Information

            StudyDescription.Text = study.StudyDescription;
            StudyInstanceUID.Text = study.StudyInstanceUid;
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

            PersonName patientName = new PersonName(_study.PatientsName);
            PersonName physicianName = new PersonName(_study.ReferringPhysiciansName);
            PatientNamePanel.PersonName = patientName;
            ReferringPhysicianNamePanel.PersonName = physicianName;
            DataBind();
        }

        #endregion
        #region Protected Methods
        
        protected override void OnInit(EventArgs e)
        {
            SetupJavascript();
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
                if (OKClicked != null)
                {
                    OKClicked();

                    XmlDocument modifiedFields = getChanges();

                    if (modifiedFields.HasChildNodes)
                    {
                        StudyController studyController = new StudyController();
                        studyController.EditStudy(study, modifiedFields);                        
                    }
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