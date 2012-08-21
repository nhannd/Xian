#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.PatientReconciliation
{
    /// <summary>
    /// Provides patient reconcilliation services
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IPatientReconciliationService
    {
        /// <summary>
        /// List reconcilliation matches for a specified patient profile
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListPatientReconciliationMatchesResponse ListPatientReconciliationMatches(ListPatientReconciliationMatchesRequest request);
        
        /// <summary>
        /// Obtains a detailed "diff" between two Patient Profiles
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadPatientProfileDiffResponse LoadPatientProfileDiff(LoadPatientProfileDiffRequest request);
        
        /// <summary>
        /// Reconciles one or more patients
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        ReconcilePatientsResponse ReconcilePatients(ReconcilePatientsRequest request);

        /// <summary>
        /// Lists all profiles for the specified set of patients
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListProfilesForPatientsResponse ListProfilesForPatients(ListProfilesForPatientsRequest request);
    }
}
