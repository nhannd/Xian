#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
    [DataContract]
    public class ListUsersResponse : DataContractBase
    {
        public ListUsersResponse(List<UserSummary> users)
        {
            Users = users;
        }

        [DataMember]
        public List<UserSummary> Users;
    }
}
