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
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions
{
    /// <summary>
    /// Partition list view panel.
    /// </summary>
    public partial class ServerPartitionGridPanel : System.Web.UI.UserControl
    {
        #region Private Members
        /// <summary>
        /// list of partitions rendered on the screen.
        /// </summary>
        private IList<ServerPartition> _partitions;

        #endregion private Members

        #region Public Properties
        /// <summary>
        /// Sets/Gets the list of partitions rendered on the screen.
        /// </summary>
        public IList<ServerPartition> Partitions
        {
            get { return _partitions; }
            set { 
                _partitions = value;
                PartitionGridView.DataSource = _partitions;
            }
        }

        /// <summary>
        /// Retrieves a reference to the embedded grid.
        /// </summary>
        public GridView TheGrid
        {
            get { return PartitionGridView; }
        }

        /// <summary>
        /// Retrieve the current selected partition.
        /// </summary>
        public ServerPartition SelectedPartition
        {
            get
            {
                int index = TheGrid.PageIndex * TheGrid.PageSize + TheGrid.SelectedIndex;

                if (index < 0 || index >= Partitions.Count)
                    return null;

                return Partitions[index];
            }
            
        }
        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
 	        base.OnInit(e);

            TheGrid.PagerSettings.Visible = false; // hide the paging control... we use a custom one outside this panel
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }

        protected void PartitionGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (PartitionGridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Add OnClick attribute to each row to make javascript call "Select$###" (where ### is the selected row)
                    // This method when posted back will be handled by the grid
                    e.Row.Attributes["OnClick"] = Page.ClientScript.GetPostBackEventReference(PartitionGridView, "Select$" + e.Row.RowIndex);
                    e.Row.Style["cursor"] = "hand";

                    // For some reason, double-click won't work if single-click is used
                    // e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackEventReference(GridView1, "Edit$" + e.Row.RowIndex);

                    CustomizeActiveColumn(e);

                }

            }

        
        }

        protected void CustomizeActiveColumn(GridViewRowEventArgs e)
        {
            Image img = ((Image)e.Row.FindControl("ActiveImage"));

            if (img != null)
            {
                bool active = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Enabled"));
                if (active)
                    img.ImageUrl = "~/images/checked_small.gif";
                else
                {
                    img.ImageUrl = "~/images/unchecked_small.gif";
                }
            }
        }

        #endregion Protected methods

        #region Public methods
        public void UpdateUI()
        {
            DataBind();

            UpdatePanel1.Update(); // force refresh
        }
        #endregion Public methods
    }
}