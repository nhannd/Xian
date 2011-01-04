#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
    [DataContract]
    public class UserDetail : DataContractBase
    {
        public UserDetail(string userId, string displayName, DateTime creationTime, DateTime? validFrom, DateTime? validUntil, 
            DateTime? lastLoginTime, bool enabled, DateTime? expiryTime, List<AuthorityGroupSummary> authorityGroups)
        {
            this.UserName = userId;
            this.DisplayName = displayName;
            this.AuthorityGroups = authorityGroups;
            this.CreationTime = creationTime;
            this.ValidFrom = validFrom;
            this.ValidUntil = validUntil;
            this.LastLoginTime = lastLoginTime;
            this.Enabled = enabled;
            this.PasswordExpiryTime = expiryTime;
        }

        public UserDetail()
        {
            this.AuthorityGroups = new List<AuthorityGroupSummary>();
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

    }
}
