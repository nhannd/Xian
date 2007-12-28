using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue
{
    /// <summary>
    /// WorkQueue Search Panel
    /// </summary>
    public partial class SearchPanel : System.Web.UI.UserControl
    {
        #region Private Members

        private ServerPartition _serverPartition;
        private WorkQueueController _searchController;
        private SearchPage _enclosingPage;

        #endregion Private Members

        #region Static Members

        private static readonly IList<WorkQueueTypeEnum> WorkQueueTypes;
        private static readonly IList<WorkQueueStatusEnum> WorkQueueStatuses;

        #endregion Static Members

        #region Static Class Initializer

        static SearchPanel()
        {
            // cache these lists
            WorkQueueTypes = WorkQueueTypeEnum.GetAll();
            WorkQueueStatuses = WorkQueueStatusEnum.GetAll();
        }

        #endregion Static Class Initializer

        #region Public Properties

        /// <summary>
        /// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition ServerPartition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }

        public SearchPage EnclosingPage
        {
            get { return _enclosingPage; }
            set { _enclosingPage = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _searchController = new WorkQueueController();
        }

        /// <summary>
        /// Handle user clicking the "Apply Filter" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FilterButton_Click(object sender, ImageClickEventArgs e)
        {
            // reload the data
            // LoadWorkQueues(); NOTE: This line is commented out because the event is fired after the page and the list have been reloaded. There's no point of loading the lists again since the data is not changed in this scenario
            searchResultAccordianControl.PageIndex = 0;
                                                    
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // first time load
                ScheduleDate.Text = DateTime.Today.ToString("MM/dd/yyyy");
            }

            // re-populate the drop down lists and restore their states
            int prevSelectedIndex = TypeDropDownList.SelectedIndex;
            TypeDropDownList.Items.Clear();
            TypeDropDownList.Items.Add(new ListItem("-- Any --", ""));
            foreach (WorkQueueTypeEnum t in WorkQueueTypes)
                TypeDropDownList.Items.Add(new ListItem(t.Description, t.Lookup));
            TypeDropDownList.SelectedIndex = prevSelectedIndex;

            prevSelectedIndex = StatusDropDownList.SelectedIndex;
            StatusDropDownList.Items.Clear();
            StatusDropDownList.Items.Add(new ListItem("-- Any --", ""));
            foreach (WorkQueueStatusEnum s in WorkQueueStatuses)
                StatusDropDownList.Items.Add(new ListItem(s.Description, s.Lookup));
            StatusDropDownList.SelectedIndex = prevSelectedIndex;

            if (searchResultAccordianControl.IsPostBack)
                LoadWorkQueues();
        }

        protected void LoadWorkQueues()
        {
            WebWorkQueueQueryParameters parameters = new WebWorkQueueQueryParameters();
            parameters.ServerPartitionKey = ServerPartition.GetKey();
            parameters.Accession = this.AccessionNumber.Text;
            parameters.PatientID = PatientId.Text;
            if (ScheduleDate.Text == "")
                parameters.ScheduledTime = null;
            else
                parameters.ScheduledTime = DateTime.Parse(ScheduleDate.Text);

            parameters.Accession = AccessionNumber.Text;
            parameters.StudyDescription = StudyDescription.Text;
            parameters.Type = (TypeDropDownList.SelectedValue == "")
                                ? null
                                : WorkQueueTypeEnum.GetEnum(TypeDropDownList.SelectedValue);
            parameters.Status = (StatusDropDownList.SelectedValue == "")
                                    ? null
                                    : WorkQueueStatusEnum.GetEnum(StatusDropDownList.SelectedValue);

            searchResultAccordianControl.WorkQueues = _searchController.FindWorkQueue(parameters);
            searchResultAccordianControl.DataBind();
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientId.Text = "";
            AccessionNumber.Text = "";
        }

        #endregion
    }
}