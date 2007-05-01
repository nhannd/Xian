using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin
{
    /// <summary>
    /// Provides operations to administer facilities
    /// </summary>
    [ServiceContract]
    public interface IFacilityAdminService
    {
        /// <summary>
        /// Summary list of all facilities
        /// </summary>
        /// <param name="request"><see cref="ListAllFacilitiesRequest"/></param>
        /// <returns><see cref="ListAllFacilitiesResponse"/></returns>
        [OperationContract]
        ListAllFacilitiesResponse ListAllFacilities(ListAllFacilitiesRequest request);

        /// <summary>
        /// Add a new facility.  A facility with the same code as an existing facility cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddFacilityRequest"/></param>
        /// <returns><see cref="AddFacilityResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddFacilityResponse AddFacility(AddFacilityRequest request);

        /// <summary>
        /// Update a new facility.  A facility with the same code as an existing facility cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateFacilityRequest"/></param>
        /// <returns><see cref="UpdateFacilityResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateFacilityResponse UpdateFacility(UpdateFacilityRequest request);

        /// <summary>
        /// Load details for a specified facility
        /// </summary>
        /// <param name="request"><see cref="GetDataForCancelOrderTableRequest"/></param>
        /// <returns><see cref="GetDataForCancelOrderTableResponse"/></returns>
        [OperationContract]
        LoadFacilityForEditResponse LoadFacilityForEdit(LoadFacilityForEditRequest request);
    }
}
