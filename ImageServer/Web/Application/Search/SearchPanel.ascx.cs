using System;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    public partial class SearchPanel : System.Web.UI.UserControl
    {
        private ServerPartition _serverPartition;
        private SearchController _searchController;

        public ServerPartition Partition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }


        public void LoadStudies()
        {
            SearchFilterPanel.SearchFilterSettings filters = SearchFilterPanel.Filters;
            StudySelectCriteria criteria = new StudySelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

            if (!String.IsNullOrEmpty(filters.PatientId))
            {
                string key = filters.PatientId + "%";
                key = key.Replace("*", "%");
                criteria.PatientId.Like(key);
            }
            if (!String.IsNullOrEmpty(filters.PatientName))
            {
                string key = filters.PatientName + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.PatientName.Like(key);
            }
            criteria.PatientName.SortAsc(0);

            if (!String.IsNullOrEmpty(filters.AccessionNumber))
            {
                string key = filters.AccessionNumber + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.AccessionNumber.Like(key);
            }
            if (!String.IsNullOrEmpty(filters.StudyDescription))
            {
                string key = filters.StudyDescription + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.StudyDescription.Like(key);
            }

            SearchGridViewControl.Studies = _searchController.GetStudies(criteria);
            SearchGridViewControl.DataBind();
        }

        /// <summary>
        /// Set up event handlers for the child controls.
        /// </summary>
        protected void SetUpEventHandlers()
        {
            SearchToolBarControl.OnRefreshButtonClick += delegate
                                                             {
                                                                 // Clear all filters and reload the data
                                                                 SearchFilterPanel.Clear();
                                                                 LoadStudies();
                                                             };

            SearchFilterPanel.ApplyFiltersClicked += delegate
                                                         {
                                                             // reload the data
                                                             LoadStudies();
                                                             SearchGridViewControl.TheGrid.PageIndex = 0;
                                                         };

            GridPager1.GetRecordCountMethod = delegate { return SearchGridViewControl.Studies.Count; };
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _searchController = new SearchController();


            // setup child controls
            GridPager1.ItemName = "Study";
            GridPager1.PuralItemName = "Studies";
            GridPager1.Grid = SearchGridViewControl.TheGrid;


            // setup event handler for child controls
            SetUpEventHandlers();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SearchGridViewControl.IsPostBack)
                LoadStudies();
        }
    }
}