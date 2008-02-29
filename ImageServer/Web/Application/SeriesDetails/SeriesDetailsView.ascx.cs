using System;
using System.Collections;
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

namespace ClearCanvas.ImageServer.Web.Application.SeriesDetails
{
    public partial class SeriesDetailsView : System.Web.UI.UserControl
    {
        private Model.Series _series;

        public Model.Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Series!=null)
            {
                IList<SeriesDetails> seriesDetails = new List<SeriesDetails>();
                seriesDetails.Add(SeriesDetailsAssembler.CreateSeriesDetails(Series));
                DetailsView1.DataSource = seriesDetails;
                DetailsView1.DataBind();
            }
        }
    }
}