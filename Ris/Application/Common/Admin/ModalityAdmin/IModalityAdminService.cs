using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
    /// <summary>
    /// Provides data loading/saving for the <see cref="ModalitySummaryComponent"/> and <see cref="ModalityEditorComponent"/>
    /// </summary>
    [ServiceContract]
    public interface IModalityAdminService
    {
        /// <summary>
        /// List all modalities for the <see cref="ModalitySummaryComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListAllModalitiesResponse ListAllModalities(ListAllModalitiesRequest request);

        /// <summary>
        /// Add a new modality created via the <see cref="ModalityEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AddModalityResponse AddModality(AddModalityRequest request);

        /// <summary>
        /// Update changes to a modality made via the <see cref="ModalityEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        UpdateModalityResponse UpdateModality(UpdateModalityRequest request);

        /// <summary>
        /// Loads all modality data for the <see cref="ModalityEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadModalityForEditResponse LoadModalityForEdit(LoadModalityForEditRequest request);
    }
}
