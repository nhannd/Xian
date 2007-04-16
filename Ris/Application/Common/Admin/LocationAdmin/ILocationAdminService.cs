using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
    /// <summary>
    /// Provides data loading/saving for the <see cref="LocationSummaryComponent"/> and <see cref="LocationEditorComponent"/>
    /// </summary>
    [ServiceContract]
    public interface ILocationAdminService
    {
        /// <summary>
        /// List all locations for the <see cref="LocationSummaryComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListAllLocationsResponse ListAllLocations(ListAllLocationsRequest request);

        /// <summary>
        /// Add a new location created via the <see cref="LocationEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddLocationResponse AddLocation(AddLocationRequest request);

        /// <summary>
        /// Update changes to a location made via the <see cref="LocationEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateLocationResponse UpdateLocation(UpdateLocationRequest request);

        /// <summary>
        /// Loads all form data for the <see cref="LocationEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetLocationEditFormDataResponse GetLocationEditFormData(GetLocationEditFormDataRequest request);

        /// <summary>
        /// Loads all location data for the <see cref="LocationEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadLocationForEditResponse LoadLocationForEdit(LoadLocationForEditRequest request);
    }
}
