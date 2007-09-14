using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class DevicePreferredTransferSyntaxQueryParameters : ProcedureParameters
    {
        public DevicePreferredTransferSyntaxQueryParameters()
            : base("QueryDevicePreferredTransferSyntaxes")
        {
        }

        /// <summary>
        /// The Primary Key of the device to query.
        /// </summary>
        public ServerEntityKey DeviceKey
        {
            set { this.SubCriteria["DeviceKey"] = new ProcedureParameter<ServerEntityKey>("DeviceKey", value); }
        }
    }
}
