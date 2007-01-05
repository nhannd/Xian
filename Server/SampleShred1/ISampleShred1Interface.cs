using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace SampleShred1
{
    [ServiceContract]
    public interface ISampleShred1Interface
    {
        [OperationContract]
        int GetLastPrimeFound();
    }
}
