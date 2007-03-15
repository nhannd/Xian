using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class DiskspaceManagerShredServiceData : IDiskspaceManagementShredInterface
    {
        public DiskspaceManagerShredServiceData()
        {
            Platform.Log("[" + AppDomain.CurrentDomain.FriendlyName + "]: DicomServerShredServiceType Constructor");
        }

        #region IDiskspaceManagementShredInterface Members

        public string GetServerInfo()
        {
            string logMessage = "[" + AppDomain.CurrentDomain.FriendlyName + "] SampleShredServiceType: GetServerInfo() called and returning " + "???";
            Platform.Log(logMessage);

            return "???";
        }

        #endregion
    }
}
