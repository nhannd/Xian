#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
    [DataContract]
    public class UserDetail : DataContractBase
    {
        public UserDetail(string userId, string displayName, string emailAddress, DateTime creationTime, DateTime? validFrom, DateTime? validUntil, 
            DateTime? lastLoginTime, bool enabled, DateTime? expiryTime, List<AuthorityGroupSummary> authorityGroups)
        {
            UserName = userId;
            DisplayName = displayName;
            AuthorityGroups = authorityGroups;
            CreationTime = creationTime;
            ValidFrom = validFrom;
            ValidUntil = validUntil;
            LastLoginTime = lastLoginTime;
            Enabled = enabled;
            PasswordExpiryTime = expiryTime;
            EmailAddress = emailAddress;
        }

        public UserDetail()
        {
            AuthorityGroups = new List<AuthorityGroupSummary>();
        }

        [DataMember]
        public string UserName;

        [DataMember]
        public string DisplayName;

        [DataMember]
        public DateTime CreationTime;

        [DataMember]
        public DateTime? ValidFrom;

        [DataMember]
        public DateTime? ValidUntil;

        [DataMember]
        public DateTime? LastLoginTime;

        [DataMember]
        public bool Enabled;

        [DataMember]
        public List<AuthorityGroupSummary> AuthorityGroups;

        /// <summary>
        /// Used by client to request password reset.
        /// </summary>
        [DataMember]
        public bool ResetPassword;

        [DataMember]
        public DateTime? PasswordExpiryTime;

        [DataMember]
        public string EmailAddress;
    }
}
