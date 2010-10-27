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

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
    [DataContract]
    public class AuthorityTokenSummary : DataContractBase
    {
        public AuthorityTokenSummary(string name, string description)
        {
            Name = name;
            Description = description;
        }

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;
    }
}
