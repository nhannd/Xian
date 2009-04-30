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

namespace ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin
{
	/// <summary>
	/// Provides operations to administer DiagnosticService entities.
	/// </summary>
	[RisApplicationService]
	[ServiceContract]
	public interface IDiagnosticServiceAdminService
	{
		/// <summary>
		/// Summary list of all items.
		/// </summary>
		[OperationContract]
		ListDiagnosticServicesResponse ListDiagnosticServices(ListDiagnosticServicesRequest request);

		/// <summary>
		/// Performs text-based query for diagnostic services.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		TextQueryResponse<DiagnosticServiceSummary> TextQuery(TextQueryRequest request);

		/// <summary>
		/// Loads details of specified itemfor editing.
		/// </summary>
		[OperationContract]
		LoadDiagnosticServiceForEditResponse LoadDiagnosticServiceForEdit(LoadDiagnosticServiceForEditRequest request);

		/// <summary>
		/// Loads all form data needed to edit an item.
		/// </summary>
		[OperationContract]
		LoadDiagnosticServiceEditorFormDataResponse LoadDiagnosticServiceEditorFormData(LoadDiagnosticServiceEditorFormDataRequest request);

		/// <summary>
		/// Adds a new item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddDiagnosticServiceResponse AddDiagnosticService(AddDiagnosticServiceRequest request);

		/// <summary>
		/// Updates an item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		UpdateDiagnosticServiceResponse UpdateDiagnosticService(UpdateDiagnosticServiceRequest request);

		/// <summary>
		/// Deletes an item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteDiagnosticServiceResponse DeleteDiagnosticService(DeleteDiagnosticServiceRequest request);
	}
}
