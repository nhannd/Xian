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
                    FilterButton.ImageUrl = "~/images/icons/QueryEnabled.png";
                else
                    FilterButton.ImageUrl = "~/images/icons/QueryEnabled.png";
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

