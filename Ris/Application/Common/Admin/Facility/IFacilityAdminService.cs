using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.Facility
{
    [ServiceContract]
    public interface IFacilityAdminService
    {
        /// <summary>
        /// Return all facility options
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        GetAllFacilitiesResponse GetAllFacilities(GetAllFacilitiesRequest request);

        /// <summary>
        /// Add a facility
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        AddFacilityResponse AddFacility(AddFacilityRequest request);

        /// <summary>
        /// Update a facility
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        UpdateFacilityResponse UpdateFacility(UpdateFacilityRequest request);

        /// <summary>
        /// Loads the Facility for the specified Facility reference
        /// </summary>
        /// <param name="facilityRef"></param>
        /// <returns></returns>
        [OperationContract]
        LoadFacilityForEditResponse LoadFacilityForEdit(LoadFacilityForEditRequest request);
    }
}
