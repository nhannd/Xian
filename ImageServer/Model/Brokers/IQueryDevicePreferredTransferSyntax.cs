using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model.Brokers
{
    public interface IQueryDevicePreferredTransferSyntax : IProcedureQueryBroker<DevicePreferredTransferSyntaxQueryParameters, DevicePreferredTransferSyntax>
    {
    }
}
