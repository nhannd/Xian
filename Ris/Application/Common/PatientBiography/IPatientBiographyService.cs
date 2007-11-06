#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
        /// Get details for an order
        /// </summary>
        /// <param name="request"><see cref="LoadOrderDetailRequest"/></param>
        /// <returns><see cref="LoadOrderDetailResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        LoadOrderDetailResponse LoadOrderDetail(LoadOrderDetailRequest request);
    }
}
