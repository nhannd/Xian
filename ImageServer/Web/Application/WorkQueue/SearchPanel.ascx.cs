using System;
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

        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Gets the <see cref="ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition Partition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Set up event handlers for the child controls.
        /// </summary>
        protected void SetUpEventHandlers()
        {
            SearchFilterPanel.ApplyFiltersClicked += delegate
                                                         {
                                                             // reload the data
                                                             // LoadWorkQueues(); NOTE: This line is commented out because the event is fired after the page and the list have been reloaded. There's no point of loading the lists again since the data is not changed in this scenario
                                                             searchResultAccordianControl.PageIndex = 0;
                                                         };
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _searchController = new WorkQueueController();

            // setup event handler for child controls
            SetUpEventHandlers();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (searchResultAccordianControl.IsPostBack)
                LoadWorkQueues();
        }

        protected void LoadWorkQueues()
        {
            SearchFilterPanel.SearchFilterSettings filters = SearchFilterPanel.Filters;
            WebWorkQueueQueryParameters parameters = new WebWorkQueueQueryParameters();
            parameters.ServerPartitionKey = Partition.GetKey();
            parameters.Accession = filters.AccessionNumber;
            parameters.PatientID = filters.PatientId;
            parameters.ScheduledTime = filters.ScheduledTime;
            parameters.Accession = filters.AccessionNumber;
            parameters.StudyDescription = filters.StudyDescription;
            parameters.Type = filters.Type;
            parameters.Status = filters.Status;

            searchResultAccordianControl.WorkQueues = _searchController.FindWorkQueue(parameters);
            searchResultAccordianControl.DataBind();
        }

        #endregion Protected Methods
    }
}