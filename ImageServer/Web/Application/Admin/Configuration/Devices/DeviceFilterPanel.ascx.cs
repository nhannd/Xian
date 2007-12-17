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

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices
{
    /// <summary>
    /// Device Filtering Panel.
    /// </summary>
    public partial class DeviceFilterPanel : System.Web.UI.UserControl
    {
        /// <summary>
        /// Used to store the device filter settings.
        /// </summary>
        public class FilterSettings
        {
            #region private members
            private string _AETitle;
            private string _IPAddress;
            private bool _enabledOnly;
            private bool _dhcpOnly;
            #endregion

            #region public properties
            /// <summary>
            /// The AE Title filter
            /// </summary>
            public string AETitle
            {
                get { return _AETitle; }
                set { _AETitle = value; }
            }
            /// <summary>
            ///  The IP Address filter
            /// </summary>
            public string IPAddress 
            {
                get { return _IPAddress; }
                set { _IPAddress = value; }
            }
            /// <summary>
            /// Device active filter.
            /// </summary>
            public bool EnabledOnly
            {
                get { return _enabledOnly; }
                set { _enabledOnly = value; }
            }
            /// <summary>
            /// Device DHCP filter
            /// </summary>
            public bool DhcpOnly
            {
                get { return _dhcpOnly; }
                set { _dhcpOnly = value; }
            }
            #endregion
        }

        #region protected methods
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handle user clicking the "Apply Filter" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FilterButton_Click(object sender, ImageClickEventArgs e)
        {
            if (ApplyFiltersClicked != null)
                ApplyFiltersClicked(Filters);
        }

        /// <summary>
        /// Determines if filters are being specified.
        /// </summary>
        /// <returns></returns>
        protected bool HasFilters()
        {
            if (AETitleFilter.Text.Length > 0 || IPAddressFilter.Text.Length > 0 || EnabledOnlyFilter.Checked || DHCPOnlyFilter.Checked)
                return true;
            else
                return false;
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
        #endregion protected methods


        #region public members

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            AETitleFilter.Text = "";
            IPAddressFilter.Text = "";
            EnabledOnlyFilter.Checked = false;
            DHCPOnlyFilter.Checked = false;

        }
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
                settings.IPAddress = IPAddressFilter.Text;
                settings.EnabledOnly = EnabledOnlyFilter.Checked;
                settings.DhcpOnly = DHCPOnlyFilter.Checked;

                return settings;
            }
        }

        #endregion // public properties

        #region Events
        /// <summary>
        /// Defines the event handler for <seealso cref="ApplyFiltersClicked"/>
        /// </summary>
        /// <param name="filters">The current settings.</param>
        /// <remarks>
        /// </remarks>
        public delegate void OnApplyFilterSettingsClickedEventHandler(FilterSettings filters);
        
        /// <summary>
        /// Occurs when the filter settings users click on "Apply" on the filter panel.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event OnApplyFilterSettingsClickedEventHandler ApplyFiltersClicked;
        #endregion // Events

       

    }

}

