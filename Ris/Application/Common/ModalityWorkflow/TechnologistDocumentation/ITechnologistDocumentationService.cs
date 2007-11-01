#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [ServiceContract]
    public interface ITechnologistDocumentationService
    {
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
        /// <param name="request"><see cref="StartModalityProcedureStepsRequest"/></param>
        /// <returns><see cref="StartModalityProcedureStepsResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        StartModalityProcedureStepsResponse StartModalityProcedureSteps(StartModalityProcedureStepsRequest request);

        /// <summary>
        /// Discontinues a set of specified modality procedure steps
        /// </summary>
        /// <param name="request"><see cref="DiscontinueModalityProcedureStepsResponse"/></param>
        /// <returns><see cref="DiscontinueModalityProcedureStepsRequest"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        DiscontinueModalityProcedureStepsResponse DiscontinueModalityProcedureSteps(DiscontinueModalityProcedureStepsRequest request);

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

        [OperationContract]
        SaveDataResponse SaveData(SaveDataRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        CompleteOrderDocumentationResponse CompleteOrderDocumentation(CompleteOrderDocumentationRequest request);
    }
}