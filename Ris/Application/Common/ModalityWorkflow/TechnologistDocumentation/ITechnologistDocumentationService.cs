using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [ServiceContract]
    public interface ITechnologistDocumentationService
    {
        /// <summary>
        /// OBSOLETE - part of first pass documentation page
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetProcedureStepsForWorklistItemResponse GetProcedureStepsForWorklistItem(GetProcedureStepsForWorklistItemRequest request);

        /// <summary>
        /// OBSOLETE - part of first pass documentation page
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        DocumentProceduresResponse DocumentProcedures(DocumentProceduresRequest request);

        /// <summary>
        /// Returns a summary of the procedure plan for a specified modality procedure step
        /// </summary>
        /// <param name="request"><see cref="GetProcedurePlanForWorklistItemRequest"/></param>
        /// <returns><see cref="GetProcedurePlanForWorklistItemResponse"/></returns>
        [OperationContract]
        GetProcedurePlanForWorklistItemResponse GetProcedurePlanForWorklistItem(GetProcedurePlanForWorklistItemRequest request);

        /// <summary>
        /// Returns a list of all modality performed procedure steps for a particular order
        /// </summary>
        /// <param name="request"><see cref="ListPerformedProcedureStepsRequest"/></param>
        /// <returns><see cref="ListPerformedProcedureStepsResponse"/></returns>
        [OperationContract]
        ListPerformedProcedureStepsResponse ListPerformedProcedureSteps(ListPerformedProcedureStepsRequest request);

        /// <summary>
        /// Starts a specified set of modality procedure steps with a single modality performed procedure step
        /// </summary>
        /// <param name="request"><see cref="StartModalityProcedureStepRequest"/></param>
        /// <returns><see cref="StartModalityProcedureStepResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        StartModalityProcedureStepResponse StartModalityProcedureStep(StartModalityProcedureStepRequest request);

        /// <summary>
        /// Stops/completes a specified modality performed procedure step
        /// </summary>
        /// <param name="request"><see cref="StopModalityPerformedProcedureStepRequest"/></param>
        /// <returns><see cref="StopModalityPerformedProcedureStepResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        StopModalityPerformedProcedureStepResponse StopModalityPerformedProcedureStep(StopModalityPerformedProcedureStepRequest request);

        /// <summary>
        /// Discontinues a specified modality performed procedure step
        /// </summary>
        /// <param name="request"><see cref="DiscontinueModalityPerformedProcedureStepRequest"/></param>
        /// <returns><see cref="DiscontinueModalityPerformedProcedureStepResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        DiscontinueModalityPerformedProcedureStepResponse DiscontinueModalityPerformedProcedureStep(DiscontinueModalityPerformedProcedureStepRequest request);

        /// <summary>
        /// Attempts to complete any modality procedure steps for which all of its modality performed procedure steps are completed.
        /// If successful, adds an interpretation step to the associated requested procedure
        /// </summary>
        /// <param name="request"><see cref="CompleteModalityProcedureStepsRequest"/></param>
        /// <returns><see cref="CompleteModalityProcedureStepsResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        CompleteModalityProcedureStepsResponse CompleteModalityProcedureSteps(CompleteModalityProcedureStepsRequest request);

        /// <summary>
        /// Discontinues a specified modality procedure step or a specified requested procedure and all of its modality procedure steps
        /// </summary>
        /// <param name="request"><see cref="DiscontinueRequestedProcedureOrModalityProcedureStepRequest"/></param>
        /// <returns><see cref="DiscontinueRequestedProcedureOrModalityProcedureStepResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        DiscontinueRequestedProcedureOrModalityProcedureStepResponse DiscontinueRequestedProcedureOrModalityProcedureStep(DiscontinueRequestedProcedureOrModalityProcedureStepRequest request);
    }
}