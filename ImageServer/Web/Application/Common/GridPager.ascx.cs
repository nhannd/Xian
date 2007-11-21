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

namespace ClearCanvas.ImageServer.Web.Application.Common
{
    /// <summary>
    /// Control to display the summary information of a grid
    /// </summary>
    public partial class GridPager : System.Web.UI.UserControl
    {
        #region Private Members
        private GridView _grid;
        private string _itemName;
        private string _puralItemName;
        #endregion Private Members

        #region Public Properties
        /// <summary>
        /// Sets/Gets the grid associated with this control
        /// </summary>
        public GridView Grid
        {
            get { return _grid; }
            set { _grid = value; }
        }

        /// <summary>
        /// Sets/Retrieve the name of the item in the list.
        /// </summary>
        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        /// <summary>
        /// Sets/Retrieves the name for the more than one items in the list.
        /// </summary>
        public string PuralItemName
        {
            get { return _puralItemName; }
            set { _puralItemName = value; }
        }


        #endregion Public Properties

        #region Public Delegates
        /// <summary>
        /// Methods to retrieve the number of records.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The number of records may be different than the value reported by <seealso cref="Grid.Rows.Count"/>
        /// </remarks>
        public delegate int GetRecordCountMethodDelegate();

        /// <summary>
        /// Sets the method to be used by this control to retrieve the total number of records.
        /// </summary>
        public GetRecordCountMethodDelegate GetRecordCountMethod;

        #endregion Public Delegates

        #region Protected methods
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void  OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            UpdateUI(); 
            
            
        }

        protected void PageButtonClick(object sender, CommandEventArgs e)
        {
            // get the current page selected
            int intCurIndex = Grid.PageIndex;

            switch (e.CommandArgument.ToString().ToLower())
            {
                case "first":
                    Grid.PageIndex = 0;
                    break;
                case "prev":
                    Grid.PageIndex = intCurIndex - 1;
                    break;
                case "next":
                    Grid.PageIndex = intCurIndex + 1;
                    break;
                case "last":
                    Grid.PageIndex = Grid.PageCount;
                    break;
            }

            Grid.DataBind();
        }

        #endregion Protected methods

        #region Public methods
        
        /// <summary>
        /// Update the UI contents
        /// </summary>
        public void UpdateUI()
        {
            if (_grid!=null)
            {
                
                int selected = _grid.SelectedIndex;

                if (GetRecordCountMethod!=null)
                {
                    int numRows = GetRecordCountMethod();
                    ItemCountLabel.Text = string.Format("{0} {1}", numRows, numRows <= 1 ? ItemName : PuralItemName);
                }


                PageCountLabel.Text =
                    string.Format("Page {0} of {1}", _grid.PageIndex + 1, _grid.PageCount == 0 ? 1 : _grid.PageCount);

                if (_grid.PageIndex > 0)
                {
                    PrevPageButton.ImageUrl = "~/images/prev.gif";
                    PrevPageButton.Enabled = true;
                }
                else
                {
                    PrevPageButton.ImageUrl = "~/images/prev_disabled.gif";
                    PrevPageButton.Enabled = false;
                }
                    
                

                if (_grid.PageIndex < _grid.PageCount-1)
                {
                    NextPageButton.ImageUrl = "~/images/next.gif";
                    NextPageButton.Enabled = true;
                }
                else
                {
                    NextPageButton.ImageUrl = "~/images/next_disabled.gif";
                    NextPageButton.Enabled = false;
                
                    
                }


            }


        }
        #endregion Public methods

       
    }

}

