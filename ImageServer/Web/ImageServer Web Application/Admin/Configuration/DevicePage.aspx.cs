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
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Web.Common;

namespace ImageServerWebApplication.Admin.Configuration
{
    public partial class DevicePage : System.Web.UI.Page
    {
        
        protected override void  OnInit(EventArgs e)
        {
 	         base.OnInit(e);

             TabContainer1.Tabs.Clear();
             int n = 0;
             ServerPartition[] partitions = GetPartitions();
             foreach (ServerPartition part in partitions)
             {
                 
                 TabPanel tab = new TabPanel();
                 tab.HeaderText = part.Description;
                 tab.ID = "Tab_" + n;

                 UpdatePanel panel = new UpdatePanel();

                 DevicePanel fm = LoadControl("DevicePanel.ascx") as DevicePanel;
                 
                 fm.Partition = part;
                 fm.ID = "DevicePanel_" + n;


                 //fm.DeviceToolBarControl.OnAddDeviceButtonClick += new Admin_Configuration_DeviceToolBarControl.AddDeviceButtonClick(OnAddDeviceButtonClick);
                 //fm.DeviceToolBarControl.OnEditDeviceButtonClick+=new Admin_Configuration_DeviceToolBarControl.EditDeviceButtonClick(OnEditDeviceButtonClick);
                 //AddDeviceControl1.DeviceAdded += fm.OnDeviceAdded;

                 tab.Controls.Add(fm);

                 panel.ContentTemplateContainer.Controls.Add(fm);

                 tab.Controls.Add(panel);

                 TabContainer1.Tabs.Add(tab);
                 //break;
                 n++;
             }


        }

        private ServerPartition[] GetPartitions()
        {
            ServerPartitionDataAdapter adapter = new ServerPartitionDataAdapter();
            IList<ServerPartition> list = adapter.GetServerPartitions();

            ServerPartition[] partitions = new ServerPartition[list.Count] ;

            list.CopyTo(partitions, 0);
            return partitions;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            

        }

        
    }
}
