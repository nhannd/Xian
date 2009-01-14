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
