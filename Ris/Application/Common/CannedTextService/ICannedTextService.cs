#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
    [RisApplicationService]
    [ServiceContract]
    public interface ICannedTextService
    {
		/// <summary>
		/// List all the canned text subscribe by the current user.
		/// </summary>
		/// <param name="request"><see cref="ListCannedTextForUserRequest"/></param>
		/// <returns><see cref="ListCannedTextForUserResponse"/></returns>
		[OperationContract]
		ListCannedTextForUserResponse ListCannedTextForUser(ListCannedTextForUserRequest request);

		/// <summary>
		/// Loads all form data needed to edit a canned text.
		/// </summary>
		/// <param name="request"><see cref="GetCannedTextEditFormDataRequest"/></param>
		/// <returns><see cref="GetCannedTextEditFormDataResponse"/></returns>
		[OperationContract]
		GetCannedTextEditFormDataResponse GetCannedTextEditFormData(GetCannedTextEditFormDataRequest request);

		/// <summary>
		/// Load details for a specified canned text.
		/// </summary>
		/// <param name="request"><see cref="LoadCannedTextForEditRequest"/></param>
		/// <returns><see cref="LoadCannedTextForEditResponse"/></returns>
		[OperationContract]
		LoadCannedTextForEditResponse LoadCannedTextForEdit(LoadCannedTextForEditRequest request);

		/// <summary>
		/// Adds a new canned text.
		/// </summary>
		/// <param name="request"><see cref="AddCannedTextRequest"/></param>
		/// <returns><see cref="AddCannedTextResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddCannedTextResponse AddCannedText(AddCannedTextRequest request);

		/// <summary>
		/// Updates an existing canned text.
		/// </summary>
		/// <param name="request"><see cref="UpdateCannedTextRequest"/></param>
		/// <returns><see cref="UpdateCannedTextResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		UpdateCannedTextResponse UpdateCannedText(UpdateCannedTextRequest request);

		/// <summary>
		/// Deletes an existing canned text.
		/// </summary>
		/// <param name="request"><see cref="DeleteCannedTextRequest"/></param>
		/// <returns><see cref="DeleteCannedTextResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DeleteCannedTextResponse DeleteCannedText(DeleteCannedTextRequest request);

		/// <summary>
		/// Modifies the category of a set of existing canned texts.
		/// </summary>
		/// <param name="request"><see cref="EditCannedTextCategoriesRequest"/></param>
		/// <returns><see cref="EditCannedTextCategoriesResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		EditCannedTextCategoriesResponse EditCannedTextCategories(EditCannedTextCategoriesRequest request);
	}
}
