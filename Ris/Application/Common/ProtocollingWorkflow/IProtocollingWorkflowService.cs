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

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [ServiceContract]
    public interface IProtocollingWorkflowService
    {
        [OperationContract]
        ListProtocolGroupsForProcedureResponse ListProtocolGroupsForProcedure(ListProtocolGroupsForProcedureRequest request);

        [OperationContract]
        GetProtocolGroupDetailResponse GetProtocolGroupDetail(GetProtocolGroupDetailRequest request);

        [OperationContract]
        GetProtocolResponse GetProtocol(GetProtocolRequest request);

        [OperationContract]
        GetProcedureProtocolResponse GetProcedureProtocol(GetProcedureProtocolRequest request);

        [OperationContract]
        GetProcedurePlanForProtocollingWorklistItemResponse GetProcedurePlanForProtocollingWorklistItem(GetProcedurePlanForProtocollingWorklistItemRequest request);

        [OperationContract]
        GetProtocolOperationEnablementResponse GetProtocolOperationEnablement(GetProtocolOperationEnablementRequest request);

        [OperationContract]
        GetClericalProtocolOperationEnablementResponse GetClericalProtocolOperationEnablement(GetClericalProtocolOperationEnablementRequest request);

        [OperationContract]
        GetSuspendRejectReasonChoicesResponse GetSuspendRejectReasonChoices(GetSuspendRejectReasonChoicesRequest request);

        /// <summary>
        /// For test purposes only
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        AddOrderProtocolStepsResponse AddOrderProtocolSteps(AddOrderProtocolStepsRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        StartOrderProtocolResponse StartOrderProtocol(StartOrderProtocolRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        DiscardOrderProtocolResponse DiscardOrderProtocol(DiscardOrderProtocolRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        AcceptOrderProtocolResponse AcceptOrderProtocol(AcceptOrderProtocolRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        RejectOrderProtocolResponse RejectOrderProtocol(RejectOrderProtocolRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        SuspendOrderProtocolResponse SuspendOrderProtocol(SuspendOrderProtocolRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        SaveProtocolResponse SaveProtocol(SaveProtocolRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        ResubmitProtocolResponse ResubmitProtocol(ResubmitProtocolRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        CancelProtocolAndOrderResponse CancelProtocolAndOrder(CancelProtocolAndOrderRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        ApproveResidentProtocolResponse ApproveResidentProtocol(ApproveResidentProtocolRequest request);
    }
}
