#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
    /// <summary>
    /// Provides operations to administer user sessions. Authentication is required.
    /// </summary>
    [EnterpriseCoreService]
    [ServiceContract]
    [Authentication(true)]
    public interface IUserSessionAdminService
    {
        /// <summary>
        /// Load session information for a specified user account
        /// </summary>
        /// <param name="request"><see cref="ListUserSessionsRequest"/></param>
        /// <returns><see cref="ListUserSessionsResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        ListUserSessionsResponse ListUserSessions(ListUserSessionsRequest request);

        /// <summary>
        /// Terminate the specified session(s)
        /// </summary>
        /// <param name="request"><see cref="ListUserSessionsRequest"/></param>
        /// <returns><see cref="ListUserSessionsResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        TerminateUserSessionResponse TerminateUserSession(TerminateUserSessionRequest request);
    }



    [DataContract]
    public class ListUserSessionsRequest : DataContractBase
    {
        /// <summary>
        /// Filter by UserName (required).
        /// </summary>
        [DataMember(IsRequired = true)]
        public string UserName;

        /// <summary>
        /// Filter by Application (optional).
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Application;

    }

    [DataContract]
    public class ListUserSessionsResponse : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public string UserName;

        /// <summary>
        /// List of sessions for this user (null if there's none)
        /// </summary>
        [DataMember(IsRequired = false)]
        public List<UserSessionSummary> Sessions;
    }

    [DataContract]
    public class TerminateUserSessionRequest : DataContractBase
    {
        /// <summary>
        /// ID of sessions to be terminated (required)
        /// </summary>
        [DataMember(IsRequired = true)]
        public List<string> SessionIDs;

    }

    [DataContract]
    public class TerminateUserSessionResponse : DataContractBase
    {
        /// <summary>
        /// ID of sessions which have been terminated
        /// </summary>
        [DataMember(IsRequired = true)]
        public string[] TerminatedSessionIDs;
    }


    [DataContract]
    public class UserSessionSummary : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public string SessionID;

        [DataMember(IsRequired = false)]
        public string Application;

        [DataMember(IsRequired = false)]
        public string HostName;

        [DataMember(IsRequired = false)]
        public DateTime CreationTime;

        [DataMember(IsRequired = false)]
        public DateTime ExpiryTime;
    }
}
