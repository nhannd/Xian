using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.PatientReconcilliation
{
    /// <summary>
    /// Provides patient reconcilliation services
    /// </summary>
    [ServiceContract]
    public interface IPatientReconcilliationService
    {
        /// <summary>
        /// List reconcilliation matches for a specified patient profile
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListPatientReconciliationMatchesResponse ListPatientReconciliationMatches(ListPatientReconciliationMatchesRequest request);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadPatientProfileDiffResponse LoadPatientProfileDiff(LoadPatientProfileDiffRequest request);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ReconcilePatientsResponse ReconcilePatients(ReconcilePatientsRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListIndirectlyReconciledPatientsResponse ListIndirectlyReconciledPatients(ListIndirectlyReconciledPatientsRequest request);
    }
}
