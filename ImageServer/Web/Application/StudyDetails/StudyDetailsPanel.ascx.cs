using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.StudyDetails
{
    public partial class StudyDetailsPanel : System.Web.UI.UserControl
    {
        private Model.Study _study;
        private Model.ServerPartition _partition;

        public Model.Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ConfirmDialog1.Confirmed += new ConfirmDialog.ConfirmedEventHandler(ConfirmDialog1_Confirmed);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            StudyDetailsView1.Studies.Add(Study);

            SeriesSearchAdaptor seriesAdaptor = new SeriesSearchAdaptor();
            SeriesSelectCriteria criteria = new SeriesSelectCriteria();
            criteria.StudyKey.EqualTo(Study.GetKey());
            criteria.ServerPartitionKey.EqualTo(Partition.GetKey());
            SeriesGridView1.Series = seriesAdaptor.Get(criteria);
            SeriesGridView1.Study = Study;

            //TODO: make Delete button enabled/disabled based on user permission

            StudyController controller = new StudyController();

            bool scheduledForDelete = controller.ScheduledForDelete(Study);

            DeleteToolbarButton.Enabled = !scheduledForDelete;

            if (scheduledForDelete)
            {
                ShowScheduledForDeleteAlert();
            }
            else
            {
                MessagePanel.Visible = false;
            }
        }



        protected void DeleteToolbarButton_Click(object sender, ImageClickEventArgs e)
        {
            ConfirmDialog1.MessageType  = ConfirmDialog.MessageTypeEnum.WARNING;
            ConfirmDialog1.Message = "Are you sure to delete this study?";
            ConfirmDialog1.Data = Study;
            
            ConfirmDialog1.Show();
        }

        void ConfirmDialog1_Confirmed(object data)
        {
            StudyController controller = new StudyController();

            Study study = ConfirmDialog1.Data as Study;
            if (controller.DeleteStudy(study))
            {
                DeleteToolbarButton.Enabled = false;

                ShowScheduledForDeleteAlert();
                
            }
            else
            {
                throw new Exception("Unable to delete the study. See server log for more details");

            }

            
        }

        private void ShowScheduledForDeleteAlert()
        {
            MessagePanel.Visible = true;
            ConfirmationMessage.Text = "This study has been scheduled for delete !";
            ConfirmationMessage.ForeColor = Color.Red;
                
            UpdatePanel1.Update();
        }
    }
}