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

        [OperationContract]
        GetProcedurePlanForWorklistItemResponse GetProcedurePlanForWorklistItem(GetProcedurePlanForWorklistItemRequest request);

        [OperationContract]
        ListPerformedProcedureStepsResponse ListPerformedProcedureSteps(ListPerformedProcedureStepsRequest request);

        [OperationContract]
        StartModalityProcedureStepResponse StartModalityProcedureStep(StartModalityProcedureStepRequest request);

        [OperationContract]
        StopModalityPerformedProcedureStepResponse StopModalityPerformedProcedureStep(StopModalityPerformedProcedureStepRequest request);

        [OperationContract]
        DiscontinueModalityPerformedProcedureStepResponse DiscontinueModalityPerformedProcedureStep(DiscontinueModalityPerformedProcedureStepRequest request);

        [OperationContract]
        CompleteModalityProcedureStepsResponse CompleteModalityProcedureSteps(CompleteModalityProcedureStepsRequest request);

        [OperationContract]
        DiscontinueRequestedProcedureOrModalityProcedureStepResponse DiscontinueRequestedProcedureOrModalityProcedureStep(DiscontinueRequestedProcedureOrModalityProcedureStepRequest request);
    }
}