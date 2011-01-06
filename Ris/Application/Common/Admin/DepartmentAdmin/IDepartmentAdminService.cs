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

namespace ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin
{
	/// <summary>
	/// Provides operations to administer Department entities.
	/// </summary>
	[RisApplicationService]
	[ServiceContract]
	public interface IDepartmentAdminService
	{
		/// <summary>
		/// Summary list of all items.
		/// </summary>
		[OperationContract]
		ListDepartmentsResponse ListDepartments(ListDepartmentsRequest request);

		/// <summary>
		/// Loads details of specified itemfor editing.
		/// </summary>
		[OperationContract]
		LoadDepartmentForEditResponse LoadDepartmentForEdit(LoadDepartmentForEditRequest request);

		/// <summary>
		/// Loads all form data needed to edit an item.
		/// </summary>
		[OperationContract]
		LoadDepartmentEditorFormDataResponse LoadDepartmentEditorFormData(LoadDepartmentEditorFormDataRequest request);

		/// <summary>
		/// Adds a new item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddDepartmentResponse AddDepartment(AddDepartmentRequest request);

		/// <summary>
		/// Updates an item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		UpdateDepartmentResponse UpdateDepartment(UpdateDepartmentRequest request);


		/// <summary>
		/// Deletes an item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteDepartmentResponse DeleteDepartment(DeleteDepartmentRequest request);

	}
}
