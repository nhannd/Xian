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
using System.Web.UI;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries
{
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