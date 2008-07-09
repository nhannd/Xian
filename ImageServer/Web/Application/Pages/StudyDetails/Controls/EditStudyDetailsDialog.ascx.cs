using System;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls
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
        /// <param name="device">The device being added.</param>
        public delegate void OnOKClickedEventHandler(Device device);

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events
        #region Private Methods

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

            PatientID.Text = study.PatientId;

            if (!string.IsNullOrEmpty(study.PatientsBirthDate))
            {
                DateTime birthDate = DateTime.ParseExact(study.StudyDate, ImageServerConstants.DicomDate, null);

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
                PatientBirthDate.Text = App_GlobalResources.SR.Unknown;
                PatientAge.Text = App_GlobalResources.SR.Unknown;
            }

            PatientGender.SelectedIndex = PatientGender.Items.IndexOf(new ListItem(study.PatientsSex));
            
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
                StudyDate.Text = App_GlobalResources.SR.Unknown;
            }

            if (!string.IsNullOrEmpty(study.StudyTime))
            {
                StudyTimeHours.Text = study.StudyTime.Substring(0, 2);
                StudyTimeMinutes.Text = study.StudyTime.Substring(2, 2);
                StudyTimeSeconds.Text = study.StudyTime.Substring(4, 2);
                StudyTimeAmPm.SelectedIndex = int.Parse(StudyTimeHours.Text) > 11 ? 1 : 0;
            }
            else
            {
                StudyTimeHours.Text = "00";
                StudyTimeMinutes.Text = "00";
                StudyTimeSeconds.Text = "00";
                StudyTimeAmPm.SelectedIndex = 0;
            }
        }

        #endregion
        #region Protected Methods

        [ExtenderControlProperty]
        [ClientPropertyName("PatientBirthDateClientID")]
        public string PatientBirthDateClientID
        {
            get { return PatientBirthDate.ClientID; }
        }
        
        protected override void OnInit(EventArgs e)
        {
            ClearStudyDateTimeButton.OnClientClick = "document.getElementById('" + StudyTimeHours.ClientID + "').value='';" +
                                                     "document.getElementById('" + StudyTimeMinutes.ClientID + "').value='';" +
                                                     "document.getElementById('" + StudyTimeSeconds.ClientID + "').value='';" +
                                                     " return false;";

            ClearPatientBirthDateButton.OnClientClick = "document.getElementById('" + PatientBirthDate.ClientID +
                                                        "').value='';" +
                                                        "document.getElementById('" + PatientAge.ClientID +
                                                        "').value=''; return false;";

            ChangeStudyInstanceUIDButton.OnClientClick = "document.getElementById('" + StudyInstanceUID.ClientID +
                                                        "').value='" +  DicomUid.GenerateUid().UID + "'; return false;";

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

        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
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