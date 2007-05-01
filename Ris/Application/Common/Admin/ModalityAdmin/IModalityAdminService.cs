using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
    /// <summary>
    /// Provides operations to administer modaltiies
    /// </summary>
    [ServiceContract]
    public interface IModalityAdminService
    {
        /// <summary>
        /// Summary list all modalities
        /// </summary>
        /// <param name="request"><see cref="ListAllModalitiesRequest"/></param>
        /// <returns><see cref="ListAllModalitiesResponse"/></returns>
        [OperationContract]
        ListAllModalitiesResponse ListAllModalities(ListAllModalitiesRequest request);

        /// <summary>
        /// Add a new modality.  A modality with the same ID as an existing modality cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddModalityRequest"/></param>
        /// <returns><see cref="AddModalityResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddModalityResponse AddModality(AddModalityRequest request);

        /// <summary>
        /// Update a new modality.  A modality with the same ID as an existing modality cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateModalityRequest"/></param>
        /// <returns><see cref="UpdateModalityResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateModalityResponse UpdateModality(UpdateModalityRequest request);

        /// <summary>
        /// Load details for a specified modality
        /// </summary>
        /// <param name="request"><see cref="LoadModalityForEditRequest"/></param>
        /// <returns><see cref="LoadModalityForEditResponse"/></returns>
        [OperationContract]
        LoadModalityForEditResponse LoadModalityForEdit(LoadModalityForEditRequest request);
    }
}
