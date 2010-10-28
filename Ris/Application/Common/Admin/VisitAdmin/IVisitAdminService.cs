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

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [RisApplicationService]
    [ServiceContract]
    public interface IVisitAdminService
    {
        [OperationContract]
        LoadVisitEditorFormDataResponse LoadVisitEditorFormData(LoadVisitEditorFormDataRequest request);

        /// <summary>
        /// Lists visits for a specified patient.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListVisitsForPatientResponse ListVisitsForPatient(ListVisitsForPatientRequest request);

        [OperationContract]
        LoadVisitForEditResponse LoadVisitForEdit(LoadVisitForEditRequest request);

        /// <summary>
        /// Updates an existing visit.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        UpdateVisitResponse UpdateVisit(UpdateVisitRequest request);

        /// <summary>
        /// Adds a new visit to the system.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddVisitResponse AddVisit(AddVisitRequest request);
    }
}
