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

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions
{
    /// <summary>
    /// Partition Filtering Pannel Control.
    /// </summary>
    public partial class ServerPartitionFilterPanel : System.Web.UI.UserControl
    {
        /// <summary>
        /// Used to store the partition filter settings.
        /// </summary>
        public class FilterSettings
        {
            #region private members
            private string _AETitle;
            private string _description;
            private string _folder;
            private bool _enabledOnly;
            
            #endregion

            #region public properties
            /// <summary>
            /// The AE Title prefix
            /// </summary>
            public string AETitle
            {
                get { return _AETitle; }
                set { _AETitle = value; }
            }
            /// <summary>
            ///  The Description prefix of the partition
            /// </summary>
            public string Description
            {
                get { return _description; }
                set { _description = value; }
            }

            /// <summary>
            ///  The Partition Folder prefix of the partition
            /// </summary>
            public string Folder
            {
                get { return _folder; }
                set { _folder = value; }
            }
            /// <summary>
            /// partition active state.
            /// </summary>
            public bool EnabledOnly
            {
                get { return _enabledOnly; }
                set { _enabledOnly = value; }
            }
            #endregion
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        #region public members
        
        #endregion

        #region public properties

        /// <summary>
        /// Retrieves the current filter settings.
        /// </summary>
        public FilterSettings Filters
        {
            get
            {
                FilterSettings settings = new FilterSettings();
                settings.AETitle = AETitleFilter.Text;
                settings.Description = DescriptionFilter.Text;
                settings.EnabledOnly = EnabledOnlyFilter.Checked;
                settings.Folder = FolderFilter.Text;
                
                return settings;
            }
        }

        #endregion // public properties

        #region Events
        /// <summary>
        /// Defines the event handler for <seealso cref="ApplyFilterClicked"/> event.
        /// </summary>
        /// <param name="filters">The current settings.</param>
        /// <remarks>
        /// </remarks>
        public delegate void OnApplyFilterClickedEventHandler(FilterSettings filters);
        
        /// <summary>
        /// Occurs when user clicks on "Apply Fillter" button.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event OnApplyFilterClickedEventHandler ApplyFilterClicked;
        #endregion // Events

        #region protected methods


        /// <summary>
        /// Determines if filters are being specified.
        /// </summary>
        /// <returns></returns>
        protected bool HasFilters()
        {
            if (AETitleFilter.Text.Length > 0 || DescriptionFilter.Text.Length > 0 || EnabledOnlyFilter.Checked)
                return true;
            else
                return false;
        }

        protected void FilterButton_Click(object sender, ImageClickEventArgs e)
        {
            if (ApplyFilterClicked != null)
                ApplyFilterClicked(Filters);
        }


        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Page.IsPostBack)
            {
                // Change the image of the "Apply Filter" button based on the filter settings
                if (HasFilters())
                    FilterButton.ImageUrl = "~/images/filter_on.gif";
                else
                    FilterButton.ImageUrl = "~/images/filter.gif";
            }
        }

        #endregion // protected methods

        #region Public methods
        public void Clear()
        {
            AETitleFilter.Text = "";
            DescriptionFilter.Text = "";
            EnabledOnlyFilter.Checked = false;
            FolderFilter.Text = "";


        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

}

