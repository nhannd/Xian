using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace SampleShred2
{
    [ServiceContract]
    public interface ISampleShred2Interface
    {
        [OperationContract]
        string GetLastPiFound();
    }
}
