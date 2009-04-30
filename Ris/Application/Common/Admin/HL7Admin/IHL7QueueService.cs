#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System;
using System.ServiceModel;
using System.Text;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    /// <summary>
    /// 
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IHL7QueueService
    {
        /// <summary>
        /// Provides all form data for the HL7 queue preview screen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetHL7QueueFormDataResponse GetHL7QueueFormData(GetHL7QueueFormDataRequest request);

        /// <summary>
        /// Provides a list of HL7 queue items matching a specified set of criteria
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        ListHL7QueueItemsResponse ListHL7QueueItems(ListHL7QueueItemsRequest request);

        /// <summary>
        /// Load details for a specified HL7 queue item
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadHL7QueueItemResponse LoadHL7QueueItem(LoadHL7QueueItemRequest request);

        /// <summary>
        /// Manually [re]process a specified HL7 queue item
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        ProcessHL7QueueItemResponse ProcessHL7QueueItem(ProcessHL7QueueItemRequest request);

        /// <summary>
        /// Determines the patient referenced by a particular HL7 message
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        GetReferencedPatientResponse GetReferencedPatient(GetReferencedPatientRequest request);
    }
}
