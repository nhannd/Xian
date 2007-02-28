using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;

namespace ClearCanvas.Server.DicomServerShred
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class DicomServerShredServiceType : IDicomServerShredInterface
    {
        public DicomServerShredServiceType()
        {
            Platform.Log("[" + AppDomain.CurrentDomain.FriendlyName + "]: DicomServerShredServiceType Constructor");
        }

        #region IDicomServerShredInterface Members

        public string GetServerInfo()
        {
            string logMessage = "[" + AppDomain.CurrentDomain.FriendlyName + "] SampleShredServiceType: GetServerInfo() called and returning " + "???";
            Platform.Log(logMessage);

            return "???";
        }

        #endregion
    }
}
