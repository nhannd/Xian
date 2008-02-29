using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ClearCanvas.ImageServer.Web.Application.StudyDetails
{
    public partial class PatientSummaryPanel : UserControl
    {
        private PatientSummary _summary;

        public PatientSummary Summary
        {
            get { return _summary; }
            set { _summary = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_summary!=null)
            {
                if (_summary.Birthdate != null)
                {
                    try
                    {
                        TimeSpan age = DateTime.Now - _summary.Birthdate.Value;
                        if (age > TimeSpan.FromDays(365))
                        {
                            PatientAge.Text = String.Format("Age: {0:0} yrs old", age.TotalDays / 365);
                        }
                        else if (age > TimeSpan.FromDays(30))
                        {
                            PatientAge.Text = String.Format("Age: {0:0} month(s)", age.TotalDays / 30);
                        }
                        else
                        {
                            PatientAge.Text = String.Format("Age: {0:0} day(s)", age.TotalDays);
                        }

                        PatientBirthDate.Text = String.Format("Birthdate: {0}", _summary.Birthdate.Value.ToString("MMM dd, yyyy"));

                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    PatientAge.Text = "Age: Unknown";
                    PatientBirthDate.Text = "Birthdate: Unknown";
                }

                PatientName.Text = _summary.PatientName;


                if (String.IsNullOrEmpty(_summary.Sex))
                    PatientSex.Text = "Gender: Unknown";
                else
                {
                    PatientSex.Text = String.Format("Gender: {0}", PatientSex.Text == "F" ? "Female" : "Male");
                }


                PatientId.Text = String.Format("Patient ID {0}: {1}", String.IsNullOrEmpty(_summary.IssuerOfPatientId) ? "" : "(" + _summary.IssuerOfPatientId + ")", _summary.PatientId);

            }
            
        }
    }
}