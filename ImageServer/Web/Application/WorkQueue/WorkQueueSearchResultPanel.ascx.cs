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
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue
{
    public partial class WorkQueueSearchResultPanel : System.Web.UI.UserControl
    {
        private IList<Model.WorkQueue> _workQueues = new List<Model.WorkQueue>();

        public IList<Model.WorkQueue> WorkQueues
        {
            get { return _workQueues; }
            set { _workQueues = value; }
        }


        public int PageIndex
        {
            set
            {
                GridView1.PageIndex = value;
            }
            get
            {
                return GridView1.PageIndex;
            }
        }

        public GridView WorkQueueListControl
        {
            get { return GridView1; }
        }


        public override void DataBind()
        {
            IList<WorkQueueSummary> list = new List<WorkQueueSummary>();
            
            if (_workQueues != null)
            {
                foreach(Model.WorkQueue wq in _workQueues)
                {
                    list.Add(WorkQueueSummaryAssembler.CreateWorkQueueSummary(wq));
                }
            }

            GridView1.DataSource = list;
            base.DataBind();
        }
    }
}