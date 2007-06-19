using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [ServiceContract]
    public interface ITechnologistDocumentationService
    {
        [OperationContract]
        [FaultContract(typeof (RequestValidationException))]
        GetProcedureStepsForWorklistItemResponse GetProcedureStepsForWorklistItem(GetProcedureStepsForWorklistItemRequest request);
    }
}