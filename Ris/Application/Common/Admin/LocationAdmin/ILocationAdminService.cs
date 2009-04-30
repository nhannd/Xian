#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
    /// <summary>
    /// Provides operations to administer locations
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface ILocationAdminService
    {
        /// <summary>
        /// Summary list of all locations
        /// </summary>
        /// <param name="request"><see cref="ListAllLocationsRequest"/></param>
        /// <returns><see cref="ListAllLocationsResponse"/></returns>
        [OperationContract]
        ListAllLocationsResponse ListAllLocations(ListAllLocationsRequest request);

        /// <summary>
        /// Add a new location.  A location with the same content as an existing location cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddLocationRequest"/></param>
        /// <returns><see cref="AddLocationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddLocationResponse AddLocation(AddLocationRequest request);

        /// <summary>
        /// Update a new location.  A location with the same content as an existing location cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateLocationRequest"/></param>
        /// <returns><see cref="UpdateLocationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateLocationResponse UpdateLocation(UpdateLocationRequest request);

		/// <summary>
		/// Delete a location.
		/// </summary>
		/// <param name="request"><see cref="DeleteLocationRequest"/></param>
		/// <returns><see cref="DeleteLocationResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteLocationResponse DeleteLocation(DeleteLocationRequest request);

		/// <summary>
        /// Loads all form data needed to edit a location
        /// </summary>
        /// <param name="request"><see cref="GetLocationEditFormDataRequest"/></param>
        /// <returns><see cref="GetLocationEditFormDataResponse"/></returns>
        [OperationContract]
        GetLocationEditFormDataResponse GetLocationEditFormData(GetLocationEditFormDataRequest request);

        /// <summary>
        /// Load details for a specified location
        /// </summary>
        /// <param name="request"><see cref="LoadLocationForEditRequest"/></param>
        /// <returns><see cref="LoadLocationForEditResponse"/></returns>
        [OperationContract]
        LoadLocationForEditResponse LoadLocationForEdit(LoadLocationForEditRequest request);
    }
}
