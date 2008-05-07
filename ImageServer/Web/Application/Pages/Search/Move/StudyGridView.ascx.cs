using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Search.Move
{
    public partial class StudyGridView : System.Web.UI.UserControl
    {
        private IList<Study> _studyList = new List<Study>();
        private ServerPartition _partition;

        public IList<Study> StudyList
        {
            get { return _studyList; }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }


        public override void DataBind()
        {
            IList<StudySummary> studySummaries = new List<StudySummary>();
            foreach (Study study in StudyList)
            {
                studySummaries.Add(StudySummaryAssembler.CreateStudySummary(study));
            }

            StudyListControl.DataSource = studySummaries;
            StudyListControl.DataBind();
            StudyListControl.PagerSettings.Visible = false;

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
            StudyListControl.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (GridViewRow row in StudyListControl.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int index = StudyListControl.PageIndex * StudyListControl.PageSize + row.RowIndex;
                    Study study = StudyList[index];

                    if (study != null)
                    {

                        row.Attributes.Add("instanceuid", study.StudyInstanceUid);
                        row.Attributes.Add("serverae", Partition.AeTitle);
                        StudyController controller = new StudyController();
                        bool deleted = controller.IsScheduledForDelete(study);
                        if (deleted)
                            row.Attributes.Add("deleted", "true");

                    }

                }

            }
        }

        protected void StudyListControl_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            
        }

        protected void StudyListControl_DataBound(object sender, EventArgs e)
        {
       //     DataBind();
        }

        protected void StudyListControl_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void StudyListControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void StudyListControl_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}