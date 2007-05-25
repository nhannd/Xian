using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.PatientBiography
{
    /// <summary>
    /// Provides patient biography services
    /// </summary>
    [ServiceContract]
    public interface IPatientBiographyService
    {
        /// <summary>
        /// Load all the patient data to populate a Patient detail/biography page
        /// </summary>
        /// <param name="request"><see cref="LoadPatientProfileRequest"/></param>
        /// <returns><see cref="LoadPatientProfileResponse"/></returns>
        [OperationContract]
        LoadPatientProfileResponse LoadPatientProfile(LoadPatientProfileRequest request);

        /// <summary>
        /// Loads all form data required to display Patient Profile details.
        /// </summary>
        /// <param name="request"><see cref="LoadPatientProfileFormDataRequest"/></param>
        /// <returns><see cref="LoadPatientProfileFormDataResponse"/></returns>
        [OperationContract]
        LoadPatientProfileFormDataResponse LoadPatientProfileFormData(LoadPatientProfileFormDataRequest request);

        /// <summary>
        /// Lists all profiles for a specified profile
        /// </summary>
        /// <param name="request"><see cref="ListAllProfilesForPatientRequest"/></param>
        /// <returns><see cref="ListAllProfilesForPatientResponse"/></returns>
        [OperationContract]
        ListAllProfilesForPatientResponse ListAllProfilesForPatient(ListAllProfilesForPatientRequest request);

        /// <summary>
        /// Get all the orders for a patient
        /// </summary>
        /// <param name="request"><see cref="ListOrdersForPatientRequest"/></param>
        /// <returns><see cref="ListOrdersForPatientResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        ListOrdersForPatientResponse ListOrdersForPatient(ListOrdersForPatientRequest request);

        /// <summary>
        /// Get details for an order
        /// </summary>
        /// <param name="request"><see cref="LoadOrderDetailRequest"/></param>
        /// <returns><see cref="LoadOrderDetailResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        LoadOrderDetailResponse LoadOrderDetail(LoadOrderDetailRequest request);
    }
}
