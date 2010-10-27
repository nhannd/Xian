#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Authentication
{
    public class UserInfo
    {
        public UserInfo(string userName, string displayName, DateTime? validFrom, DateTime? validUntil)
        {
            this.UserName = userName;
            this.DisplayName = displayName;
            this.ValidFrom = validFrom;
            this.ValidUntil = validUntil;
        }

        public string UserName;
        public string DisplayName;
        public DateTime? ValidFrom;
        public DateTime? ValidUntil;
    }
}
