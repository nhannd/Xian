#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    /// <summary>
    /// Provides operations to administer protocol codes and protocol groups
    /// </summary>
    [RisServiceProvider]
    [ServiceContract]
    public interface IProtocolAdminService
    {
        /// <summary>
        /// Adds a new protocol code with specified name and description (optional)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AddProtocolCodeResponse AddProtocolCode(AddProtocolCodeRequest request);

        /// <summary>
        /// Updates name and/or description of specified protocol code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        UpdateProtocolCodeResponse UpdateProtocolCode(UpdateProtocolCodeRequest request);

        /// <summary>
        /// Marks a protocol code as deleted
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        DeleteProtocolCodeResponse DeleteProtocolCode(DeleteProtocolCodeRequest request);

        /// <summary>
        /// Summary list of all protocol groups
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListProtocolGroupsResponse ListProtocolGroups(ListProtocolGroupsRequest request);

        /// <summary>
        /// Loads details for specified protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadProtocolGroupForEditResponse LoadProtocolGroupForEdit(LoadProtocolGroupForEditRequest request);

        /// <summary>
        /// Provides a list of available protocol codes and reading groups that can be assigned while adding/updating a protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetProtocolGroupEditFormDataResponse GetProtocolGroupEditFormData(GetProtocolGroupEditFormDataRequest request);

        /// <summary>
        /// Adds a new protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AddProtocolGroupResponse AddProtocolGroup(AddProtocolGroupRequest request);

        /// <summary>
        /// Updates an existing protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        UpdateProtocolGroupResponse UpdateProtocolGroup(UpdateProtocolGroupRequest request);

        /// <summary>
        /// Marks a protocol group as deleted
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        DeleteProtocolGroupResonse DeleteProtocolGroup(DeleteProtocolGroupRequest request);
    }
}
