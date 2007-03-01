using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Services
{
    [ServiceContract]
    public interface ITestService : IHealthcareServiceLayer
    {
        [OperationContract]
        PatientProfilePreview GetPatientProfilePreview(EntityRef profileRef);

        [OperationContract]
        string GetName(int i);

        [OperationContract]
        EntityRef Echo(EntityRef profileRef);

        [OperationContract]
        List<PatientProfilePreview.Address> GetAddresses(EntityRef profileRef);
    }
}
