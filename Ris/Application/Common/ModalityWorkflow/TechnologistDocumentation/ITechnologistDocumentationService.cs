using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [ServiceContract]
    public interface ITechnologistDocumentationService
    {
        [OperationContract]
        GetProcedureStepsForWorklistItemResponse GetProcedureStepsForWorklistItem(GetProcedureStepsForWorklistItemRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        DocumentProceduresResponse DocumentProcedures(DocumentProceduresRequest request);
    }
}