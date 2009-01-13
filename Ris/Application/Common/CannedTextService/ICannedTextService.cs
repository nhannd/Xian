using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
    [RisServiceProvider]
    [ServiceContract]
    public interface ICannedTextService
    {
		/// <summary>
		/// List all the canned text subscribe by the current staff
		/// </summary>
		/// <param name="request"><see cref="ListCannedTextRequest"/></param>
		/// <returns><see cref="ListCannedTextResponse"/></returns>
		[OperationContract]
		ListCannedTextResponse ListCannedText(ListCannedTextRequest request);

		/// <summary>
		/// Loads all form data needed to edit a CannedText
		/// </summary>
		/// <param name="request"><see cref="GetCannedTextEditFormDataRequest"/></param>
		/// <returns><see cref="GetCannedTextEditFormDataResponse"/></returns>
		[OperationContract]
		GetCannedTextEditFormDataResponse GetCannedTextEditFormData(GetCannedTextEditFormDataRequest request);

		/// <summary>
		/// Load details for a specified canned text
		/// </summary>
		/// <param name="request"><see cref="LoadCannedTextForEditRequest"/></param>
		/// <returns><see cref="LoadCannedTextForEditResponse"/></returns>
		[OperationContract]
		LoadCannedTextForEditResponse LoadCannedTextForEdit(LoadCannedTextForEditRequest request);

		/// <summary>
		/// Adds a new canned text
		/// </summary>
		/// <param name="request"><see cref="AddCannedTextRequest"/></param>
		/// <returns><see cref="AddCannedTextResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddCannedTextResponse AddCannedText(AddCannedTextRequest request);

		/// <summary>
		/// Updates an existing canned text
		/// </summary>
		/// <param name="request"><see cref="UpdateCannedTextRequest"/></param>
		/// <returns><see cref="UpdateCannedTextResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		UpdateCannedTextResponse UpdateCannedText(UpdateCannedTextRequest request);

		/// <summary>
		/// Deletes an existing CannedText.
		/// </summary>
		/// <param name="request"><see cref="DeleteCannedTextRequest"/></param>
		/// <returns><see cref="DeleteCannedTextResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DeleteCannedTextResponse DeleteCannedText(DeleteCannedTextRequest request);
	}
}
