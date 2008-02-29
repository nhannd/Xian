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

namespace ClearCanvas.ImageServer.Web.Application.SeriesDetails
{
    public partial class SeriesDetailsPanel : System.Web.UI.UserControl
    {
        private Model.Study _study;
        private Model.Series _series;

        public Model.Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public Model.Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Series != null)
                SeriesDetailsView1.Series = Series;
        }
    }
}