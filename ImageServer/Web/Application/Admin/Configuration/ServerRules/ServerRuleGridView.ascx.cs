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

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules
{
    public partial class ServerRuleGridView : System.Web.UI.UserControl
    {
        #region private members
         // list of devices to display
        private IList<ServerRule> _serverRules;
        private ServerRulePanel _serverRulePanel;
     
        #endregion Private members

     
        #region public properties

     
        public ServerRulePanel ServerRulePanel
        {
            get { return _serverRulePanel; }
            set { _serverRulePanel = value; }
        }

        public GridView TheGrid
        {
            get { return this.GridView; }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public ServerRule SelectedRule
        {
            get
            {
                if (ServerRules.Count == 0 || GridView.SelectedIndex < 0)
                    return null;

                // SelectedIndex is for the current page. Must convert to the index of the entire list
                int index = GridView.PageIndex * GridView.PageSize + GridView.SelectedIndex;

                if (index < 0 || index > ServerRules.Count - 1)
                    return null;

                return ServerRules[index];
            }
            set
            {

                GridView.SelectedIndex = ServerRules.IndexOf(value);
              //  if (OnDeviceSelectionChanged != null)
               //     OnDeviceSelectionChanged(this, value);
            }
        }

        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
        /// </summary>
        public IList<ServerRule> ServerRules
        {
            get
            {
                return _serverRules;
            }
            set
            {
                _serverRules = value;
                GridView.DataSource = _serverRules; // must manually call DataBind() later
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            GridView.DataBind();
        }

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (GridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Add OnClick attribute to each row to make javascript call "Select$###" (where ### is the selected row)
                    // This method when posted back will be handled by the grid
                    e.Row.Attributes["OnClick"] =
                        Page.ClientScript.GetPostBackEventReference(GridView, "Select$" + e.Row.RowIndex);
                    e.Row.Style["cursor"] = "hand";

                    // For some reason, double-click won't work if single-click is used
                    // e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackEventReference(GridView1, "Edit$" + e.Row.RowIndex);

                    CustomizeColumns(e);
                }
            }
        }
        protected void CustomizeColumns(GridViewRowEventArgs e)
        {
            ServerRule fs = e.Row.DataItem as ServerRule;
            Label lbl = e.Row.FindControl("ServerRuleApplyTimeEnum") as Label; // The label is added in the template
            lbl.Text = fs.ServerRuleApplyTimeEnum.Description;

            lbl = e.Row.FindControl("ServerRuleTypeEnum") as Label; // The label is added in the template
            lbl.Text = fs.ServerRuleTypeEnum.Description;

      
            Image img = ((Image)e.Row.FindControl("EnabledImage"));
            if (img != null)
            {
                if (fs.Enabled)
                {
                    img.ImageUrl = "~/images/checked_small.gif";
                }
                else
                {
                    img.ImageUrl = "~/images/unchecked_small.gif";
                }
            }

            img = ((Image)e.Row.FindControl("DefaultImage"));
            if (img != null)
            {
                if (fs.DefaultRule)
                {
                    img.ImageUrl = "~/images/checked_small.gif";
                }
                else
                {
                    img.ImageUrl = "~/images/unchecked_small.gif";
                }
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void GridView_DataBound(object sender, EventArgs e)
        {

        }

        protected void GridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void GridView_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            ServerRulePanel.OnRowSelected(e.NewSelectedIndex);
        }

        #region public methods
        /// <summary>
        /// Binds the list to the control.
        /// </summary>
        /// <remarks>
        /// This method must be called after setting <seeaslo cref="Devices"/> to update the grid with the list.
        /// </remarks>
        public override void DataBind()
        {
            GridView.DataBind();

            GridView.PagerSettings.Visible = false;

        }

        #endregion // public methods

  
    }
}