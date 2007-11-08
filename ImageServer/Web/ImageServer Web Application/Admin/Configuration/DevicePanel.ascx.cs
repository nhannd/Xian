using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Web.Common;
using ImageServerWebApplication.Common;

namespace ImageServerWebApplication.Admin.Configuration
{
     /// <summary>
     /// Panel to display list of devices for a particular server partition.
     /// </summary>
    public partial class DevicePanel : System.Web.UI.UserControl
    {
        #region Private members
        // the controller used for interaction with the database.
        private DeviceConfigurationController _theController;
        // the partition whose information will be displayed in this panel
        private ServerPartition _partition;
        #endregion Private members

        #region Public Properties
        /// <summary>
        /// Sets/Gets the partition whose information is displayed in this panel.
        /// </summary>
        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        #endregion

        #region Public Delegates

        /// <summary>
        /// Defines the method which this panel will call to add a new device.
        /// </summary>
        /// <param name="controller">The controller used by this panel for database interaction.</param>
        /// <param name="parition">The partition displayed in this panel.</param>
        public delegate void AddDeviceMethodDelegate(DeviceConfigurationController controller, ServerPartition parition);

        /// <summary>
        /// Sets/Retrieves the delegate which this panel will call to add a new device.
        /// </summary>
        public AddDeviceMethodDelegate AddDeviceDelegate;

        /// <summary>
        /// Defines the method which this panel will call to edit a new device.
        /// </summary>
        /// <param name="controller">The controller used by this panel for database interaction.</param>
        /// <param name="partition">The partition displayed in this panel.</param>
        /// <param name="device">The device to be editted. This device belongs to partition specified in <paramref name="partition"/></param>
        public delegate void EditDeviceMethodDelegate(DeviceConfigurationController controller, ServerPartition partition, Device device);

        /// <summary>
        /// Sets/Retrieves the delegate which this panel will call to edit a new device.
        /// </summary>
        public EditDeviceMethodDelegate EditDeviceDelegate;

        /// <summary>
        /// Defines the method which this panel will call to delete a new device.
        /// </summary>
        /// <param name="controller">The controller used by this panel for database interaction.</param>
        /// <param name="partition">The partition displayed in this panel.</param>
        /// <param name="device">The device is being deleted. This device belongs to partition specified in <paramref name="partition"/></param>
        public delegate void DeleteDeviceMethodDelegate(DeviceConfigurationController controller, ServerPartition partition, Device device);

        /// <summary>
        /// Sets/Retrieves the delegate which this panel will call to delete a new device.
        /// </summary>
        public DeleteDeviceMethodDelegate DeleteDeviceDelegate;
        
        #endregion

        #region protected methods
        /// <summary>
        /// Set up event handlers for the child controls.
        /// </summary>
        protected void SetUpEventHandlers()
        {
            DeviceToolBarControl1.OnAddDeviceButtonClick += delegate
                                                                {
                                                                    // Call the add device delegate 
                                                                    if (AddDeviceDelegate != null)
                                                                        AddDeviceDelegate(_theController, Partition);
                                                                };


            DeviceToolBarControl1.OnEditDeviceButtonClick += delegate
                                                                 {
                                                                     // Call the edit device delegate 
                                                                     if (EditDeviceDelegate != null)
                                                                     {
                                                                         Device dev = DeviceGridViewControl1.SelectedDevice;
                                                                         if (dev != null)
                                                                         {
                                                                             EditDeviceDelegate(_theController, Partition, dev);
                                                                         }
                                                                     }
                                                                 };

            DeviceToolBarControl1.OnRefreshButtonClick += delegate
                                                              {
                                                                  // Clear all filters and reload the data
                                                                  DeviceFilterPanel1.Clear();
                                                                  LoadDevices();
                                                              };

            DeviceToolBarControl1.OnDeleteDeviceButtonClick += delegate
                                                                   {
                                                                       // Call the delete device delegate 
                                                                       if (DeleteDeviceDelegate != null)
                                                                       {
                                                                           Device dev = DeviceGridViewControl1.SelectedDevice;
                                                                           if (dev != null)
                                                                           {
                                                                               DeleteDeviceDelegate(_theController,Partition, dev);
                                                                           }
                                                                       }
                                                                   };


            DeviceToolBarControl1.GetSelectedDevice = delegate
                                                         {
                                                             return DeviceGridViewControl1.SelectedDevice;
                                                         };


            DeviceFilterPanel1.ApplyFiltersClicked += delegate(DeviceFilterPanel.FilterSettings filters)
                                                    {
                                                        // reload the data
                                                        LoadDevices();
                                                    };


            GridPager1.GetRecordCountMethod = delegate()
                                                          {
                                                              return DeviceGridViewControl1.Devices.Count;
                                                          };
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new DeviceConfigurationController();


            // setup child controls
            GridPager1.ItemName = "Device";
            GridPager1.PuralItemName = "Devices";
            GridPager1.Grid = DeviceGridViewControl1.TheGrid;
            

            // setup event handler for child controls
            SetUpEventHandlers();

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // This make sure we have the list to work with. 
            // the list may be out-dated if the add/update event is fired later
            // In those cases, the list must be refreshed again.
            LoadDevices();

        }

        

        #endregion Protected methods

        /// <summary>
        /// Load the devices for the partition based on the filters specified in the filter panel.
        /// </summary>
        /// <remarks>
        /// This method only reloads and binds the list bind to the internal grid. <seealso cref="UpdateUI()"/> should be called
        /// to explicit update the list in the grid. 
        /// <para>
        /// This is intentionally so that the list can be reloaded so that it is available to other controls during postback.  In
        /// some cases we may not want to refresh the list if there's no change. Calling <seealso cref="UpdateUI()"/> will
        /// give performance hit as the data will be transfered back to the browser.
        ///  
        /// </para>
        /// </remarks>
        public void LoadDevices()
        {
            DeviceFilterPanel.FilterSettings filters = DeviceFilterPanel1.Filters;
            DeviceSelectCriteria criteria = new DeviceSelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

            if (!String.IsNullOrEmpty(filters.AETitle))
            {
                string key = filters.AETitle + "%";
                key.Replace("*", "%");
                //AETitle.Replace("?", "?");
                criteria.AeTitle.Like(key);
            }

            if (!String.IsNullOrEmpty(filters.IPAddress))
            {
                string key = filters.IPAddress + "%";
                key.Replace("*", "%");
                //IP.Replace("?", "?");
                criteria.IPAddress.Like(key);
            }

            if (filters.EnabledOnly)
            {
                criteria.Active.EqualTo(true);
            }

            if (filters.DhcpOnly)
            {
                criteria.Dhcp.EqualTo(true);
            }

            DeviceGridViewControl1.Devices = _theController.GetDevices(criteria);
            DeviceGridViewControl1.DataBind();
        }

        /// <summary>
        /// Updates the device list window in the panel.
        /// </summary>
        /// <remarks>
        /// This method should only be called when necessary as the information in the list window needs to be transmitted back to the client.
        /// If the list is not changed, call <seealso cref="LoadDevices()"/> instead.
        /// </remarks>
        public void UpdateUI()
        {
            LoadDevices();

            // UpdatePanel UpdateMode must be set to "conditional"
            // Calling UpdatePanel.Update() will force the client to refresh the screen
            UpdatePanel.Update();
        }
        
    }
}