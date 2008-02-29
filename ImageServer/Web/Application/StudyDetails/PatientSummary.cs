using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ClearCanvas.ImageServer.Web.Application.StudyDetails
{
    /// <summary>
    /// Model object behind the <see cref="PatientSummaryPanel"/>
    /// </summary>
    public class PatientSummary
    {
        private string _patientId;
        private string _issuerOfPatientId;
        private string _patientName;
        private DateTime? _birthdate;
        private string _sex;

        public string PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public string PatientName
        {
            get { return _patientName; }
            set { _patientName = value; }
        }

        public DateTime? Birthdate
        {
            get { return _birthdate; }
            set { _birthdate = value; }
        }

        public string Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        public string IssuerOfPatientId
        {
            get { return _issuerOfPatientId; }
            set { _issuerOfPatientId = value; }
        }
    }
}
