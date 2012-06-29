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
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
    [DataContract]
    public class ListAuthorityTokensResponse : DataContractBase
    {
        public ListAuthorityTokensResponse(List<AuthorityTokenSummary> authorityTokens)
        {
            AuthorityTokens = authorityTokens;
        }

        [DataMember]
        public List<AuthorityTokenSummary> AuthorityTokens;
    }
}
