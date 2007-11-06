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

namespace ImageServerWebApplication.Admin.Configuration
{
    /// <summary>
    /// Device Filtering Pannel Control.
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
            /// The AE Title prefix
            /// </summary>
            public string AETitle
            {
                get { return _AETitle; }
                set { _AETitle = value; }
            }
            /// <summary>
            ///  The IP Address prefix of the devices
            /// </summary>
            public string IPAddress
            {
                get { return _IPAddress; }
                set { _IPAddress = value; }
            }
            /// <summary>
            /// Device active state.
            /// </summary>
            public bool EnabledOnly
            {
                get { return _enabledOnly; }
                set { _enabledOnly = value; }
            }
            /// <summary>
            /// Device DHCP state.
            /// </summary>
            public bool DhcpOnly
            {
                get { return _dhcpOnly; }
                set { _dhcpOnly = value; }
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
                settings.IPAddress = IPAddressFilter.Text;
                settings.EnabledOnly = EnabledOnlyFilter.Checked;
                settings.DhcpOnly = DHCPOnlyFilter.Checked;

                return settings;
            }
        }

        #endregion // public properties

        #region Events
        /// <summary>
        /// Defines the event handler when the filter settings on the panel has been changed.
        /// </summary>
        /// <param name="filters">The current settings.</param>
        /// <remarks>
        /// </remarks>
        public delegate void OnFilterChangedEventHandler(FilterSettings filters);
        
        /// <summary>
        /// Occurs when the filter settings on the panel has been changed.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event OnFilterChangedEventHandler FilterChanged;
        #endregion // Events

        #region protected methods

        protected void FilterButton_Click(object sender, ImageClickEventArgs e)
        {
            if (FilterChanged != null)
                FilterChanged(Filters);
        }

        #endregion // protected methods

    }

}

