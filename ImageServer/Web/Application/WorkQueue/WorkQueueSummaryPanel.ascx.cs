using System;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue
{
    /// <summary>
    /// WorkQueue Summary Panel 
    /// </summary>
    public partial class WorkQueueSummaryPanel : System.Web.UI.UserControl
    {
        #region Private members
        private WorkQueueSummary _workqueueSummary;
        #endregion Private members

        #region Public Properties
        
        public WorkQueueSummary WorkQueueSummary
        {
            get { return _workqueueSummary; }
            set { _workqueueSummary = value; }
        }
        
        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {

            this.WorkQueueType.Text = _workqueueSummary.Type.Description;
            this.WorkQueueStatus.Text = _workqueueSummary.Status.Description;
            this.ScheduledTime.Text = _workqueueSummary.ScheduledDateTime.ToString();

            this.PatientID.Text = _workqueueSummary.PatientID;
            this.PatientsName.Text = _workqueueSummary.PatientName;

        }

        #endregion Protected Methods
    }
}