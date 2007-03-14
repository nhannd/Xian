using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [ServiceContract]
    public interface IModalityWorkflowService
    {
        [OperationContract]
        GetWorklistResponse GetWorklist(GetWorklistRequest request);

        [OperationContract]
        GetWorklistItemResponse GetWorklistItem(GetWorklistItemRequest request);

        [OperationContract]
        LoadWorklistItemPreviewResponse LoadWorklistItemPreview(LoadWorklistItemPreviewRequest request);

        [OperationContract]
        GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request);

        [OperationContract(IsOneWayt=true)]
        void ExecuteOperation(ExecuteOperationRequest request);
    }
}
