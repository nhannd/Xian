#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    /// <summary>
    /// Control to display the summary information of a grid
    /// </summary>
    public partial class GridPager : UserControl
    {
        #region Private Members

        private GridView _target;
        private string _itemName;
        private string _puralItemName;
        private ImageServerConstants.GridViewPagerPosition _position;

        #endregion Private Members

        #region Public Properties

        public ImageServerConstants.GridViewPagerPosition PagerPosition
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Sets/Gets the grid associated with this control
        /// </summary>
        public GridView Target
        {
            get { return _target; }
            set { _target = value; }
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
        /// The number of records may be different than the value reported by <seealso cref="GridPager.Target.Rows.Count"/>
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
            if (!IsPostBack)
            {
                string script =
                    "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" +
                    ChangePageButton.ClientID + "').click();return false;}} else {return true}; ";
                
                CurrentPage.Attributes.Add("onkeydown", script);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            UpdateUI();
        }

        protected void PageButtonClick(object sender, CommandEventArgs e)
        {
            // get the current page selected
            int intCurIndex = Target.PageIndex;

            switch (e.CommandArgument.ToString().ToLower())
            {
                case ImageServerConstants.Prev:
                    Target.PageIndex = intCurIndex - 1;
                    break;
                case ImageServerConstants.Next:
                    Target.PageIndex = intCurIndex + 1;
                    break;
                default:
                    int newPage = Convert.ToInt32(CurrentPage.Text);

                    //Adjust page to match 0..n, and handle boundary conditions.
                    if (newPage > Target.PageCount) newPage = _target.PageCount -1;
                    else if (newPage != 0) newPage -= 1;
                    
                    _target.PageIndex =  newPage;
                    
                    break;
            }

            Target.DataBind();
        }

        private int AdjustCurrentPageForDisplay(int page)
        {
            if (_target.PageCount == 0)
            {
                page = 0;
            } else if (page == 0 )
            {
                page = 1;
            } else if (page >= _target.PageCount)
            {
                page = _target.PageCount;
            } else
            {
                page += 1;
            }

            return page;
        }

        #endregion Protected methods

        #region Public methods

        /// <summary>
        /// Update the UI contents
        /// </summary>
        public void UpdateUI()
        {
            if (_target != null)
            {
                if (GetRecordCountMethod != null)
                {
                    int numRows = GetRecordCountMethod();
                    ItemCountLabel.Text = string.Format("{0} {1}", numRows, numRows == 1 ? ItemName : PuralItemName);
                }

                CurrentPage.Text = AdjustCurrentPageForDisplay(_target.PageIndex).ToString();

                PageCountLabel.Text =
                    string.Format(" of {0}", AdjustCurrentPageForDisplay(_target.PageCount));

                if (_target.PageIndex > 0)
                {
                    PrevPageButton.Enabled = true;
                    PrevPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerPreviousEnabled;
                }
                else
                {
                    PrevPageButton.Enabled = false;
                    PrevPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerPreviousDisabled;
                }


                if (_target.PageIndex < _target.PageCount - 1)
                {
                    NextPageButton.Enabled = true;
                    NextPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerNextEnabled;
                }
                else
                {
                    NextPageButton.Enabled = false;
                    NextPageButton.ImageUrl = ImageServerConstants.ImageURLs.GridPagerNextDisabled;
                }
            }
        }

        public void InitializeGridPager(string singleItemLabel, string multipleItemLabel, GridView grid, ImageServerConstants.GridViewPagerPosition position)
        {
            _position = position;
            ItemName = singleItemLabel;
            PuralItemName = multipleItemLabel;
            Target = grid;
        }

        #endregion Public methods
    }
}