using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.Modality
{
    [ServiceContract]
    public interface IModalityAdminService
    {
        /// <summary>
        /// Return all modality options
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ModalitySummary> GetAllModalities();

        /// <summary>
        /// Add the specified modality
        /// </summary>
        /// <param name="modality"></param>
        [OperationContract]
        ModalitySummary AddModality(ModalityDetail modality);

        /// <summary>
        /// Update the specified modality
        /// </summary>
        /// <param name="modality"></param>
        [OperationContract]
        ModalitySummary UpdateModality(EntityRef modalityRef, ModalityDetail modalityDetail);

        /// <summary>
        /// Loads the modality for the specified modality reference
        /// </summary>
        /// <param name="modalityRef"></param>
        /// <returns></returns>
        [OperationContract]
        ModalityDetail LoadModalityDetail(EntityRef modalityRef);

        [OperationContract]
        ModalityEditFormData LoadModalityEditForm();
    }
}
