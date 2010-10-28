#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		/// Delete an existing external practitioners.
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

		/// <summary>
		/// Merge two external practitioners.
		/// </summary>
		[OperationContract]
		MergeExternalPractitionerResponse MergeExternalPractitioner(MergeExternalPractitionerRequest request);

		/// <summary>
		/// Load all form data needed to merge two external practitioners.
		/// </summary>
		[OperationContract]
		LoadMergeExternalPractitionerFormDataResponse LoadMergeExternalPractitionerFormData(LoadMergeExternalPractitionerFormDataRequest request);
	}
}
