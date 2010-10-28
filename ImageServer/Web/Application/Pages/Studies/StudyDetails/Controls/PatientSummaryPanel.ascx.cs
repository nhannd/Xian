#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    /// <summary>
    /// Patient summary information panel within the <see cref="StudyDetailsPanel"/> 
    /// </summary>
    public partial class PatientSummaryPanel : UserControl
    {
        #region private members
        private PatientSummary _patientSummary;
        #endregion private members


        #region Public Properties
        /// <summary>
        /// Gets or sets the <see cref="PatientSummary"/> object used by the panel.
        /// </summary>
        public PatientSummary PatientSummary
        {
            get { return _patientSummary; }
            set { _patientSummary = value; }
        }

        #endregion Public Properties


        #region Protected methods

        public override void DataBind()
        {
            base.DataBind();
            if (_patientSummary != null)
            {

                personName.PersonName = _patientSummary.PatientName;
                PatientDOB.Value = _patientSummary.Birthdate;
				if (!String.IsNullOrEmpty(_patientSummary.PatientsAge))
				{
                     string patientAge = _patientSummary.PatientsAge.Substring(0, 3).TrimStart('0');

                    switch (_patientSummary.PatientsAge.Substring(3))
                    {
                        case "Y":
                            patientAge += " Years";
                            break;
                        case "M":
                            patientAge += " Months";
                            break;
                        case "W":
                            patientAge += " Weeks";
                            break;
                        default:
                            patientAge += " Days";
                            break;
                    }

                    if (_patientSummary.PatientsAge.Substring(0, 3).Equals("001"))
                        patientAge = patientAge.TrimEnd('s');

                    PatientAge.Text = patientAge;
				}
				else
				{
    				PatientAge.Text = "Unknown";
				}

            	if (String.IsNullOrEmpty(_patientSummary.Sex))
                    PatientSex.Text = "Unknown";
                else
                {
                    if (_patientSummary.Sex.StartsWith("F"))
                        PatientSex.Text = "Female";
                    else if (_patientSummary.Sex.StartsWith("M"))
                        PatientSex.Text = "Male";
                    else if (_patientSummary.Sex.StartsWith("O"))
                        PatientSex.Text = "Other";
                    else
                        PatientSex.Text = "Unknown";
                }


                PatientId.Text = _patientSummary.PatientId;

            }

        }

        #endregion Protected methods
    }
}