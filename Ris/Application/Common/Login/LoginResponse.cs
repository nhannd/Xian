#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Login
{
    [DataContract]
    public class LoginResponse : DataContractBase
    {
        public LoginResponse(SessionToken sessionToken, string[] userAuthorityTokens, StaffSummary staffSummary)
        {
            this.SessionToken = sessionToken;
            this.UserAuthorityTokens = userAuthorityTokens;
            this.StaffSummary = staffSummary;
        }

        /// <summary>
        /// Staff Summary for the logged in user.
        /// </summary>
        [DataMember]
        public StaffSummary StaffSummary;

        /// <summary>
        /// Set of authority tokens granted to the user
        /// </summary>
        [DataMember]
        public string[] UserAuthorityTokens;

        /// <summary>
        /// Session token which the client must return with all subsequent service calls.
        /// </summary>
        [DataMember]
        public SessionToken SessionToken;
    }
}
