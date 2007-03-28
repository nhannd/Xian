using System;
using System.ServiceModel;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.Alert
{
    [ServiceContract]
    public interface IAlertService
    {
        [OperationContract]
        GetAlertsByPatientProfileResponse GetAlertsByPatientProfile(GetAlertsByPatientProfileRequest request);

        [OperationContract]
        GetAlertsByPatientResponse GetAlertsByPatient(GetAlertsByPatientRequest request);
    }
}
