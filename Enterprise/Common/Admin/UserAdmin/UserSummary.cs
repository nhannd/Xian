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
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
    [DataContract]
    public class UserSummary : DataContractBase
    {
        public UserSummary(string userId, string displayName, string emailAddress, DateTime creationTime, DateTime? validFrom, DateTime? validUntil,
            DateTime? lastLoginTime, DateTime? passwordExpiry, bool enabled, int sessionCount)
        {
            UserName = userId;
            DisplayName = displayName;
            EmailAddress = emailAddress;
            CreationTime = creationTime;
            ValidFrom = validFrom;
            ValidUntil = validUntil;
            LastLoginTime = lastLoginTime;
            Enabled = enabled;
            PasswordExpiry = passwordExpiry;
            SessionCount = sessionCount;
        }

        [DataMember]
        public string UserName;

        [DataMember]
        public string DisplayName;

        [DataMember]
        public string EmailAddress;

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
        public DateTime? PasswordExpiry;

        [DataMember]
        public int SessionCount;

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
