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

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
    [DataContract]
    public class UserSummary : DataContractBase
    {
        public UserSummary(string userId, string displayName, DateTime creationTime, DateTime? validFrom, DateTime? validUntil,
            DateTime? lastLoginTime, bool enabled)
        {
            this.UserName = userId;
            this.DisplayName = displayName;
            this.CreationTime = creationTime;
            this.ValidFrom = validFrom;
            this.ValidUntil = validUntil;
            this.LastLoginTime = lastLoginTime;
            this.Enabled = enabled;
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

        protected bool Equals(UserSummary userSummary)
        {
            if (userSummary == null) return false;
            return Equals(UserName, userSummary.UserName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as UserSummary);
        }

        public override int GetHashCode()
        {
            return UserName.GetHashCode();
        }
    }
}
