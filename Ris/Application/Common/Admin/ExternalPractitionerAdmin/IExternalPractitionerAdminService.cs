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

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
    /// <summary>
    /// Provides operations to administer staffs
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IExternalPractitionerAdminService
    {
        /// <summary>
        /// Returns a list of practitioners based on a textual query.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		TextQueryResponse<ExternalPractitionerSummary> TextQuery(TextQueryRequest request);

        /// <summary>
        /// Summary list of all practitioners
        /// </summary>
        [OperationContract]
        ListExternalPractitionersResponse ListExternalPractitioners(ListExternalPractitionersRequest request);

        /// <summary>
        /// Add a new practitioner.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddExternalPractitionerResponse AddExternalPractitioner(AddExternalPractitionerRequest request);

        /// <summary>
        /// Update a new practitioner.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateExternalPractitionerResponse UpdateExternalPractitioner(UpdateExternalPractitionerRequest request);

		/// <summary>
		/// Delete duplicate external practitioners.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteExternalPractitionerResponse DeleteExternalPractitioner(DeleteExternalPractitionerRequest request);

        /// <summary>
        /// Load details for a specified practitioner for editing.
        /// </summary>
        [OperationContract]
        LoadExternalPractitionerForEditResponse LoadExternalPractitionerForEdit(LoadExternalPractitionerForEditRequest request);

        /// <summary>
        /// Loads all form data needed to edit a practitioner.
        /// </summary>
        [OperationContract]
        LoadExternalPractitionerEditorFormDataResponse LoadExternalPractitionerEditorFormData(LoadExternalPractitionerEditorFormDataRequest request);

		/// <summary>
		/// Merge duplicate external practitioners.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		MergeDuplicatePractitionerResponse MergeDuplicatePractitioner(MergeDuplicatePractitionerRequest request);

		/// <summary>
		/// Merge duplicate external practitioners contact points.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		MergeDuplicateContactPointResponse MergeDuplicateContactPoint(MergeDuplicateContactPointRequest request);

		/// <summary>
		/// Loads all form data needed to merge two practitioners.
		/// </summary>
		[OperationContract]
		LoadMergeDuplicatePractitionerFormDataResponse LoadMergeDuplicatePractitionerFormData(LoadMergeDuplicatePractitionerFormDataRequest request);

		/// <summary>
		/// Loads all form data needed to merge two contact points.
		/// </summary>
		[OperationContract]
		LoadMergeDuplicateContactPointFormDataResponse LoadMergeDuplicateContactPointFormData(LoadMergeDuplicateContactPointFormDataRequest request);
	}
}
