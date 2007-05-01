using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
    /// <summary>
    /// Provides operations to administer locations
    /// </summary>
    [ServiceContract]
    public interface ILocationAdminService
    {
        /// <summary>
        /// Summary list of all locations
        /// </summary>
        /// <param name="request"><see cref="ListAllLocationsRequest"/></param>
        /// <returns><see cref="ListAllLocationsResponse"/></returns>
        [OperationContract]
        ListAllLocationsResponse ListAllLocations(ListAllLocationsRequest request);

        /// <summary>
        /// Add a new location.  A location with the same content as an existing location cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddLocationRequest"/></param>
        /// <returns><see cref="AddLocationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddLocationResponse AddLocation(AddLocationRequest request);

        /// <summary>
        /// Update a new location.  A location with the same content as an existing location cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateLocationRequest"/></param>
        /// <returns><see cref="UpdateLocationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateLocationResponse UpdateLocation(UpdateLocationRequest request);

        /// <summary>
        /// Loads all form data needed to edit a location
        /// </summary>
        /// <param name="request"><see cref="GetLocationEditFormDataRequest"/></param>
        /// <returns><see cref="GetLocationEditFormDataResponse"/></returns>
        [OperationContract]
        GetLocationEditFormDataResponse GetLocationEditFormData(GetLocationEditFormDataRequest request);

        /// <summary>
        /// Load details for a specified location
        /// </summary>
        /// <param name="request"><see cref="LoadLocationForEditRequest"/></param>
        /// <returns><see cref="LoadLocationForEditResponse"/></returns>
        [OperationContract]
        LoadLocationForEditResponse LoadLocationForEdit(LoadLocationForEditRequest request);
    }
}
