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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
    /// <summary>
    /// Provides operations to administer modaltiies
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IModalityAdminService
    {
        /// <summary>
        /// Summary list all modalities
        /// </summary>
        /// <param name="request"><see cref="ListAllModalitiesRequest"/></param>
        /// <returns><see cref="ListAllModalitiesResponse"/></returns>
        [OperationContract]
        ListAllModalitiesResponse ListAllModalities(ListAllModalitiesRequest request);

        /// <summary>
        /// Add a new modality.  A modality with the same ID as an existing modality cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddModalityRequest"/></param>
        /// <returns><see cref="AddModalityResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddModalityResponse AddModality(AddModalityRequest request);

        /// <summary>
        /// Update a new modality.  A modality with the same ID as an existing modality cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateModalityRequest"/></param>
        /// <returns><see cref="UpdateModalityResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateModalityResponse UpdateModality(UpdateModalityRequest request);

		/// <summary>
		/// Delete a modality.
		/// </summary>
		/// <param name="request"><see cref="DeleteModalityRequest"/></param>
		/// <returns><see cref="DeleteModalityResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteModalityResponse DeleteModality(DeleteModalityRequest request);
		
		/// <summary>
        /// Load details for a specified modality
        /// </summary>
        /// <param name="request"><see cref="LoadModalityForEditRequest"/></param>
        /// <returns><see cref="LoadModalityForEditResponse"/></returns>
        [OperationContract]
        LoadModalityForEditResponse LoadModalityForEdit(LoadModalityForEditRequest request);
    }
}
