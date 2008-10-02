using System;
using System.Web.UI.WebControls;
using System.Xml;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Web.Common.Data;

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

            string dicomName = PatientLastName.Text + ImageServerConstants.DicomSeparator +
                               PatientGivenName.Text + ImageServerConstants.DicomSeparator +
                               PatientMiddleName.Text + ImageServerConstants.DicomSeparator +
                               PatientTitle.Text + ImageServerConstants.DicomSeparator +
                               PatientSuffix.Text;

            dicomName = dicomName.Trim(new char[] { '^' });
            
            if(!study.PatientsName.Equals(dicomName))
            {
                rootNode.AppendChild(createChildNode(setNode, ImageServerConstants.DicomTags.PatientsName, dicomName));
            }

            String dicomBirthDate = !(string.IsNullOrEmpty(PatientBirthDate.Text))
                                        ? DateTime.Parse(PatientBirthDate.Text).ToString(ImageServerConstants.DicomDate)
                                        : "";
            if (!study.PatientsBirthDate.Equals(dicomBirthDate))
            {
                rootNode.AppendChild(createChildNode(setNode, ImageServerConstants.DicomTags.PatientsBirthDate, dicomBirthDate));
            }

            if (!study.PatientsSex.Equals(PatientGender.Text))
            {
                rootNode.AppendChild(createChildNode(setNode, ImageServerConstants.DicomTags.PatientsSex, PatientGender.Text));
            }

            if (!study.PatientId.Equals(PatientID.Text))
            {
                rootNode.AppendChild(createChildNode(setNode, ImageServerConstants.DicomTags.PatientID, PatientID.Text));
            }

            if(String.IsNullOrEmpty(study.StudyDescription)
				|| !study.StudyDescription.Equals((StudyDescription.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, ImageServerConstants.DicomTags.StudyDescription, StudyDescription.Text));
            }

            if(!study.StudyInstanceUid.Equals((StudyInstanceUID.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, ImageServerConstants.DicomTags.StudyInstanceUID, StudyInstanceUID.Text));
            }

			if (String.IsNullOrEmpty(study.StudyId)
				|| !study.StudyId.Equals((StudyID.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, ImageServerConstants.DicomTags.StudyID, StudyID.Text));
            }

			if (String.IsNullOrEmpty(study.AccessionNumber)
				|| !study.AccessionNumber.Equals((AccessionNumber.Text)))
            {
                rootNode.AppendChild(createChildNode(setNode, ImageServerConstants.DicomTags.AccessionNumber, AccessionNumber.Text));
            }

            dicomName = PhysicianLastName.Text + ImageServerConstants.DicomSeparator +
                        PhysicianGivenName.Text + ImageServerConstants.DicomSeparator +
                        PhysicianMiddleName.Text + ImageServerConstants.DicomSeparator +
                        PhysicianTitle.Text + ImageServerConstants.DicomSeparator +
                        PhysicianSuffix.Text;

            dicomName = dicomName.Trim(new char[] { '^' });

			if (String.IsNullOrEmpty(study.ReferringPhysiciansName)
				|| !study.ReferringPhysiciansName.Equals(dicomName))
            {
                rootNode.AppendChild(createChildNode(setNode, ImageServerConstants.DicomTags.ReferringPhysician, dicomName));
            }

            String dicomStudyDate = !(string.IsNullOrEmpty(StudyDate.Text))
                                        ? DateTime.Parse(StudyDate.Text).ToString(ImageServerConstants.DicomDate)
                                        : "";

            if(!study.StudyDate.Equals(dicomStudyDate))
            {
                rootNode.AppendChild(createChildNode(setNode, ImageServerConstants.DicomTags.StudyDate, dicomStudyDate));
            }


            if (StudyTimeAmPm.SelectedValue.Equals("PM") || (StudyTimeAmPm.SelectedValue.Equals("AM") && StudyTimeHours.Text.Equals("12"))) StudyTimeHours.Text = (Int16.Parse(StudyTimeHours.Text) + 12).ToString();
            string dicomStudyTime = StudyTimeHours.Text.PadLeft(2, '0') + StudyTimeMinutes.Text.PadLeft(2, '0') + StudyTimeSeconds.Text.PadLeft(2, '0');
            if(!study.StudyTime.Equals(dicomStudyTime))
            {
                rootNode.AppendChild(createChildNode(setNode, ImageServerConstants.DicomTags.StudyTime, dicomStudyTime));
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

            int i = 0;
            while(i < splitDicomName.Length)
            {
                switch(i)
                {
                    case 0:
                        PatientLastName.Text = splitDicomName[i];
                        break;
                    case 1:
                        PatientGivenName.Text = splitDicomName[i];
                        break;
                    case 2:
                        PatientMiddleName.Text = splitDicomName[i];
                        break;
                    case 3:
                        PatientTitle.Text = splitDicomName[i];
                        break;
                    default:
                        PatientSuffix.Text = splitDicomName[i];
                        break;
                }
                i++;
            }

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
                DateTime birthDate = DateTime.ParseExact(study.PatientsBirthDate, ImageServerConstants.DicomDate, null);

                PatientBirthDate.Text = birthDate.ToString(ImageServerConstants.MMDDYYY);

                TimeSpan age = DateTime.Now - birthDate;
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

            dicomName = study.ReferringPhysiciansName;
            splitDicomName = dicomName.Split('^');

            i = 0;
            while (i < splitDicomName.Length)
            {
                switch (i)
                {
                    case 0:
                        PhysicianLastName.Text = splitDicomName[i];
                        break;
                    case 1:
                        PhysicianGivenName.Text = splitDicomName[i];
                        break;
                    case 2:
                        PhysicianMiddleName.Text = splitDicomName[i];
                        break;
                    case 3:
                        PhysicianTitle.Text = splitDicomName[i];
                        break;
                    default:
                        PhysicianSuffix.Text = splitDicomName[i];
                        break;
                }
                i++;
            }

            if (!string.IsNullOrEmpty(study.StudyDate))
            {
                DateTime studyDate = DateTime.ParseExact(study.StudyDate, ImageServerConstants.DicomDate, null);
                StudyDate.Text = studyDate.ToString(ImageServerConstants.MMDDYYY);
            } else
            {
                StudyDate.Text = string.Empty;
            }

            if (!string.IsNullOrEmpty(study.StudyTime) && study.StudyTime.Length == 6)
            {
                StudyTimeHours.Text = study.StudyTime.Substring(0, 2);
                StudyTimeMinutes.Text = study.StudyTime.Substring(2, 2);
                StudyTimeSeconds.Text = study.StudyTime.Substring(4, 2);

                int hours = int.Parse(StudyTimeHours.Text);

                if(hours > 12)
                {
                    hours -= 12;
                    StudyTimeAmPm.SelectedIndex = hours == 12 ? 0 : 1;
                    StudyTimeHours.Text = hours.ToString();
                } else if(hours == 12)
                {
                    StudyTimeAmPm.SelectedIndex = 1;
                } else
                {
                    StudyTimeAmPm.SelectedIndex = 0;
                }
            }
            else
            {
                StudyTimeHours.Text = "12";
                StudyTimeMinutes.Text = "00";
                StudyTimeSeconds.Text = "00";
                StudyTimeAmPm.SelectedIndex = 0;
            }
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