using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common
{
    [ServiceContract]
    public interface ITimeService : ICoreServiceLayer
    {
        [OperationContract]
        DateTime GetTime();
    }
}
