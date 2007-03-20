using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [ServiceContract]
    public interface IRegistrationWorkflowService
    {
        [OperationContract]
        GetWorklistResponse GetWorklist(GetWorklistRequest request);

        [OperationContract]
        LoadWorklistPreviewResponse LoadWorklistPreview(LoadWorklistPreviewRequest request);

        [OperationContract]
        GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request);

        [OperationContract]
        void ExecuteOperation(ExecuteOperationRequest request);

        [OperationContract]
        GetDataForCheckInTableResponse GetDataForCheckInTable(GetDataForCheckInTableRequest request);

        [OperationContract]
        void CheckInProcedure(CheckInProcedureRequest request);

        //RequestedProcedure LoadRequestedProcedure(EntityRef rpRef, bool loadDetail);
        //void UpdateRequestedProcedure(RequestedProcedure rp);
        //void AddCheckInProcedureStep(CheckInProcedureStep cps);

        [OperationContract]
        LoadPatientSearchComponentFormDataResponse LoadPatientSearchComponentFormData(LoadPatientSearchComponentFormDataRequest request);

    }
}
