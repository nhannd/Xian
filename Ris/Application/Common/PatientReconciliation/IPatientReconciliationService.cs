using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.PatientReconciliation
{
    /// <summary>
    /// Provides patient reconcilliation services
    /// </summary>
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
