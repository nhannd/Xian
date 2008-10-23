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
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class StudyStateAlertPanel : System.Web.UI.UserControl
    {
        private StudySummary _studySummary;

        /// <summary>
        /// Message displayed
        /// </summary>
        protected Label Message;

        public StudySummary Study
        {
            get { return _studySummary; }
            set { _studySummary = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            Visible = false;
            base.OnInit(e);
        }

        public override void DataBind()
        {
            if (_studySummary!=null)
            {
                if (_studySummary.IsProcessing)
                {
                    ShowAlert(App_GlobalResources.SR.StudyBeingProcessed);
                }
                else if (_studySummary.IsLocked)
                {
                    ShowAlert(_studySummary.QueueStudyStateEnum.LongDescription);
                }
                else if (_studySummary.IsNearline)
                {
                    ShowAlert(App_GlobalResources.SR.StudyIsNearline);
                }
                else if (_studySummary.IsReconcileRequired)
                {
                    ShowAlert(App_GlobalResources.SR.StudyRequiresReconcilie);
                }
            }

            base.DataBind();
        }

        private void ShowAlert(string message)
        {
            Message.Text = message;
            Visible = true;
        }
    }
}