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
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls;

namespace ClearCanvas.ImageServer.Web.Application.StudyDetails
{
    public partial class SeriesGridView : System.Web.UI.UserControl
    {
        private Study _study;
        private IList<Model.Series> _series;

        public IList<Model.Series> Series
        {
            get { return _series; }
            set { _series = value; }
        }

        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Series!=null && Study!=null)
            {
                ServerPartitionDataAdapter adaptor = new ServerPartitionDataAdapter();
                ServerPartition partition = adaptor.Get(Study.ServerPartitionKey); 

                if (partition!=null)
                {
                    //ScriptTemplate template = new ScriptTemplate(typeof (SeriesGridView).Assembly,
                    //                           "ClearCanvas.ImageServer.Web.Application.StudyDetails.SeriesGridView.js");
                    //template.Replace("@@GRIDVIEW_CONTROL_JS_OBJECT@@", GridView1.ControlJSObjectVariable);
 
                    //template.Replace("@@SERIES_DETAILS_PAGE_URL@@",
                    //                 Page.ResolveClientUrl("~/SeriesDetails/SeriesDetailsPage.aspx"));

                    //template.Replace("@@PARTITION_AE@@", partition.AeTitle);
                    //template.Replace("@@STUDY_INSTANCE_UID@@", Study.StudyInstanceUid);

                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), ClientID + "_OpenSeriesScript",
                    //                                        template.Script, true);
                }
                
            }
            
            
            GridView1.DataSource = Series;
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Series series = e.Row.DataItem as Series;

                Label performedDateTime = e.Row.FindControl("SeriesPerformedDateTime") as Label;
                performedDateTime.Text = series.PerformedProcedureStepStartDate != null
                                             ?
                                                 series.PerformedProcedureStepStartDate.ToString()
                                             : "";


                e.Row.Attributes["seriesuid"] = series.SeriesInstanceUid;


            }
        }

        protected void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DataBind();
        }
    }
}