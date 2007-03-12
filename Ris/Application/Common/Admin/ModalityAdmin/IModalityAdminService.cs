using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
    [ServiceContract]
    public interface IModalityAdminService
    {
        /// <summary>
        /// Return all modality options
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ListAllModalitiesResponse ListAllModalities(ListAllModalitiesRequest request);

        /// <summary>
        /// Add the specified modality
        /// </summary>
        /// <param name="modality"></param>
        [OperationContract]
        AddModalityResponse AddModality(AddModalityRequest request);

        /// <summary>
        /// Update the specified modality
        /// </summary>
        /// <param name="modality"></param>
        [OperationContract]
        UpdateModalityResponse UpdateModality(UpdateModalityRequest request);

        /// <summary>
        /// Loads the modality for the specified modality reference
        /// </summary>
        /// <param name="modalityRef"></param>
        /// <returns></returns>
        [OperationContract]
        LoadModalityForEditResponse LoadModalityForEdit(LoadModalityForEditRequest request);
    }
}
