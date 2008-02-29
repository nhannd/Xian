using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.StudyDetails
{
    public partial class StudyDetailsView : System.Web.UI.UserControl
    {
        private IList<Model.Study> _studies = new List<Model.Study>();

        public IList<Study> Studies
        {
            get { return _studies; }
            set { _studies = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DetailsView1.DataSource = Studies;
            DetailsView1.DataBind();
        }
    }
}