using System;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin
{
	/// <summary>
	/// Provides operations to administer ProcedureType entities.
	/// </summary>
	[RisServiceProvider]
	[ServiceContract]
	public interface IProcedureTypeAdminService
	{
		/// <summary>
		/// Summary list of all items.
		/// </summary>
		[OperationContract]
		ListProcedureTypesResponse ListProcedureTypes(ListProcedureTypesRequest request);

		/// <summary>
		/// Loads details of specified item for editing.
		/// </summary>
		[OperationContract]
		LoadProcedureTypeForEditResponse LoadProcedureTypeForEdit(LoadProcedureTypeForEditRequest request);

		/// <summary>
		/// Loads all form data needed to edit an item.
		/// </summary>
		[OperationContract]
		LoadProcedureTypeEditorFormDataResponse LoadProcedureTypeEditorFormData(LoadProcedureTypeEditorFormDataRequest request);

		/// <summary>
		/// Adds a new item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddProcedureTypeResponse AddProcedureType(AddProcedureTypeRequest request);

		/// <summary>
		/// Updates an item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		UpdateProcedureTypeResponse UpdateProcedureType(UpdateProcedureTypeRequest request);

		/// <summary>
		/// Deletes an item.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteProcedureTypeResponse DeleteProcedureType(DeleteProcedureTypeRequest request);
	}
}
