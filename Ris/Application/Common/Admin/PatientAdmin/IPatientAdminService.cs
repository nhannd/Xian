#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    [RisApplicationService]
    [ServiceContract]
    public interface IPatientAdminService
    {
        [OperationContract]
        LoadPatientProfileEditorFormDataResponse LoadPatientProfileEditorFormData(LoadPatientProfileEditorFormDataRequest request);

        [OperationContract]
        LoadPatientProfileForEditResponse LoadPatientProfileForEdit(LoadPatientProfileForEditRequest request);

        /// <summary>
        /// Updates a patient profile.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdatePatientProfileResponse UpdatePatientProfile(UpdatePatientProfileRequest request);

        /// <summary>
        /// Adds a new patient to the system.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddPatientResponse AddPatient(AddPatientRequest request);
    }
}
