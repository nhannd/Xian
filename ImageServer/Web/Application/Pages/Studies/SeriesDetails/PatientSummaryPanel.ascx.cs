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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    /// <summary>
    /// Patient summary information panel within the <see cref="SeriesDetailsPanel"/> 
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
            if (_patientSummary != null)
            {
                personName.PersonName = _patientSummary.PatientName;
                PatientDOB.Value = _patientSummary.Birthdate;

                DateTime? bdate = DateParser.Parse(_patientSummary.Birthdate);

                if (bdate!=null)
                {
                    TimeSpan age = Platform.Time - bdate.Value;
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
                    else
                        PatientSex.Text = "Unknown";
                }


                PatientId.Text = _patientSummary.PatientId;

            }

            base.DataBind();
        }

        #endregion Protected methods
    }
}