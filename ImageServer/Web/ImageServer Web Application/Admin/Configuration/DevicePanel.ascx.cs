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
using ClearCanvas.ImageServer.Web.Common;
using ImageServerWebApplication.Common;

namespace ImageServerWebApplication.Admin.Configuration
{
    
    public partial class DevicePanel : System.Web.UI.UserControl
    {
            
        private DeviceConfigurationController _theController;
        private ServerPartition _partition;
        private DeviceDataAdapter _adapter;

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public DeviceToolBar DeviceToolBarControl
        {
            get { return DeviceToolBarControl1; }
        }

        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            
            _theController = new DeviceConfigurationController();

            
            
            //DeviceToolBarControl1.Partition = Partition;
            DeviceToolBarControl1.OnAddDeviceButtonClick += new DeviceToolBar.AddDeviceButtonClick(DeviceToolBarControl1_OnAddDeviceButtonClick);


            DeviceToolBarControl1.OnEditDeviceButtonClick += new DeviceToolBar.EditDeviceButtonClick(DeviceToolBarControl1_OnEditDeviceButtonClick);
            EditDeviceControl1.OnUpdateDevice = delegate(Device device)
                                                    {
                                                        _theController.UpdateDevice(device);
                                                        LoadDevices();
                                                    };

            DeviceToolBarControl1.OnRefreshButtonClick += new DeviceToolBar.RefreshButtonClick(DeviceToolBarControl1_OnRefreshButtonClick);
            DeviceToolBarControl1.OnDeleteDeviceButtonClick += new DeviceToolBar.DeleteDeviceButtonClick(DeviceToolBarControl1_OnDeleteDeviceButtonClick);

            ConfirmDialog1.OnConfirmed = delegate()
                        {
                            Device dev = DeviceGridViewControl1.SelectedDevice;
                            _theController.DeleteDevice(dev);

                            LoadDevices();
                            
                        };

            AddDeviceControl1.Partitions = new List<ServerPartition>();
            AddDeviceControl1.Partitions.Add(_partition);
            AddDeviceControl1.OnAddDevice = delegate(Device dev)
                                                {
                                                    _theController.AddDevice(dev);

                                                    LoadDevices();
                                                };

            DeviceToolBarControl.GetSelectedDevice = delegate
                                                         {
                                                             return DeviceGridViewControl1.SelectedDevice;
                                                         };


            DeviceFilterPanel1.FilterChanged += delegate(DeviceFilterPanel.FilterSettings filters)
                                                    {
                                                        LoadDevices();
                                                    };

        }   

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadDevices();

        }


        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }
        protected void LoadDevices()
        {
            DeviceFilterPanel.FilterSettings filters = DeviceFilterPanel1.Filters;

            DeviceGridViewControl1.Devices = _theController.GetDevices(filters.AETitle, filters.IPAddress, filters.EnabledOnly, filters.DhcpOnly, Partition.GetKey());
            DeviceGridViewControl1.DataBind();
        }

        public void OnDeviceAdded(Device device)
        {
            if (device.ServerPartition.GetKey().Equals(this._partition.GetKey()))
            {
                // Reload data
                LoadDevices();

                
            }
        }

        public void DeviceToolBarControl1_OnRefreshButtonClick(object sender, ImageClickEventArgs ev)
        {
            // Reload data
            LoadDevices();
        }
        

        protected void DeviceToolBarControl1_OnAddDeviceButtonClick(object sender, ImageClickEventArgs ev)
        {
            AddDeviceControl1.Show();
            
        }

        protected void DeviceToolBarControl1_OnEditDeviceButtonClick(object sender, ImageClickEventArgs ev)
        {
            Device dev = DeviceGridViewControl1.SelectedDevice;
            if (dev!=null)
            {
                EditDeviceControl1.Partitions = _theController.GetServerPartitions();

                EditDeviceControl1.Device = dev;
                EditDeviceControl1.Show();
            }
            
        }

        protected void DeviceToolBarControl1_OnDeleteDeviceButtonClick(object sender, ImageClickEventArgs ev)
        {
            
            Device dev = DeviceGridViewControl1.SelectedDevice;

            if (dev!=null)
            {
                ConfirmDialog1.Message = string.Format("Are you sure to delete {0}?", dev.AeTitle);

                ConfirmDialog1.MessageType = ConfirmDialog.MessageTypeEnum.WARNING;

                ConfirmDialog1.Show();
            }
        }

        

        public void DeviceUpdated(Device device)
        {
            // Reload data
            LoadDevices();

        }
    }
}