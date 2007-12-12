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
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    public partial class StudySummary : System.Web.UI.UserControl
    {
        private Study _study;

        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PatientName.Text = _study.PatientName;
            this.PatientId.Text = _study.PatientId;
            this.AccessionNumber.Text = _study.AccessionNumber;
            this.StudyDescription.Text = _study.StudyDescription;
            this.StudyDate.Text = _study.StudyDate;
        }
    }
}