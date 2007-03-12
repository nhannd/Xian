using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
    [ServiceContract]
    public interface ILocationAdminService
    {
        /// <summary>
        /// Return all location options
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ListAllLocationsResponse ListAllLocations(ListAllLocationsRequest request);

        /// <summary>
        /// Add the specified location
        /// </summary>
        /// <param name="location"></param>
        [OperationContract]
        AddLocationResponse AddLocation(AddLocationRequest request);

        /// <summary>
        /// Update the specified location
        /// </summary>
        /// <param name="location"></param>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        UpdateLocationResponse UpdateLocation(UpdateLocationRequest request);

        [OperationContract]
        GetLocationEditFormDataResponse GetLocationEditFormData(GetLocationEditFormDataRequest request);

        [OperationContract]
        LoadLocationForEditResponse LoadLocationForEdit(LoadLocationForEditRequest request);
    }
}
