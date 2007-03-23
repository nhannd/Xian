using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin
{
    /// <summary>
    /// Provides data loading/saving for the <see cref="FacilitySummaryComponent"/> and <see cref="FacilityEditorComponent"/>
    /// </summary>
    [ServiceContract]
    public interface IFacilityAdminService
    {
        /// <summary>
        /// List all facilities for the <see cref="FacilitySummaryComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListAllFacilitiesResponse ListAllFacilities(ListAllFacilitiesRequest request);

        /// <summary>
        /// Add a new facility created via the <see cref="FacilityEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AddFacilityResponse AddFacility(AddFacilityRequest request);

        /// <summary>
        /// Update changes to a facility made via the <see cref="FacilityEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        UpdateFacilityResponse UpdateFacility(UpdateFacilityRequest request);

        /// <summary>
        /// Loads all facility data for the <see cref="FacilityEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadFacilityForEditResponse LoadFacilityForEdit(LoadFacilityForEditRequest request);
    }
}
