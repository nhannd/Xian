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
        private bool _pageCountVisible;
        private bool _itemCountVisible;

        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Sets/Gets whether or not the page count label is visible
        /// </summary>
        public bool PageCountVisible
        {
            get { return _pageCountVisible; }
            set { _pageCountVisible = value; }
        }

        /// <summary>
        /// Sets/Gets the item count label is visible
        /// </summary>
        public bool ItemCountVisible
        {
            get { return _itemCountVisible; }
            set { _itemCountVisible = value; }
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
                case "first":
                    Target.PageIndex = 0;
                    break;
                case "prev":
                    Target.PageIndex = intCurIndex - 1;
                    break;
                case "next":
                    Target.PageIndex = intCurIndex + 1;
                    break;
                case "last":
                    Target.PageIndex = Target.PageCount;
                    break;
            }

            Target.DataBind();
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

                PageCountLabel.Text =
                    string.Format("Page {0} of {1}", _target.PageIndex + 1, _target.PageCount == 0 ? 1 : _target.PageCount);

                PageCountLabel.Visible = _pageCountVisible;
                ItemCountLabel.Visible = _itemCountVisible;

                if (_target.PageIndex > 0)
                {
                    PrevPageButton.Enabled = true;
                    PrevPageButton.CssClass = "GlobalGridPagerLink";
                }
                else
                {
                    PrevPageButton.Enabled = false;
                    PrevPageButton.CssClass = "GlobalGridPagerLinkDisabled";
                }


                if (_target.PageIndex < _target.PageCount - 1)
                {
                    NextPageButton.Enabled = true;
                    NextPageButton.CssClass = "GlobalGridPagerLink";
                }
                else
                {
                    NextPageButton.Enabled = false;
                    NextPageButton.CssClass = "GlobalGridPagerLinkDisabled";
                }

                NextPageButton.Text = App_GlobalResources.SR.GridPagerNext;
                PrevPageButton.Text = App_GlobalResources.SR.GridPagerPrevious;
                if (PrevPageButton.Enabled || NextPageButton.Enabled)
                {
                    LineSpacerLabel.CssClass = "GlobalGridPagerLinkDisabled";
                }
                else
                {
                    LineSpacerLabel.CssClass = "GlobalGridPagerLinkDisabled";
                }
            }
        }

        #endregion Public methods
    }
}