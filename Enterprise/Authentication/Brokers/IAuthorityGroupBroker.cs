#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Enterprise.Authentication.Brokers
{
    public partial interface IAuthorityGroupBroker
    {
        int GetUserCountForGroup(AuthorityGroup group);

        Guid[] FindDataGroupsByUserName(string userName);
    }
}
