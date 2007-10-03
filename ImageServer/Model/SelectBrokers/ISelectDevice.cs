using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.Criteria;

namespace ClearCanvas.ImageServer.Model.SelectBrokers
{
    public interface ISelectDevice : ISelectBroker<DeviceSelectCriteria, Device>
    {
    }
}
