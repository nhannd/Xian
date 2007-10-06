using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Database.SqlServer2005;
  
namespace ClearCanvas.ImageServer.Model.SqlServer2005.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class UpdateDevice : ProcedureUpdateBroker<DeviceUpdateParameters>, IUpdateDevice
    {
        public UpdateDevice()
            : base("UpdateDevice")
        {
        }
    }
}
