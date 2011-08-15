#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
    [DataContract]
    public class AuthorityGroupDetail : DataContractBase
    {
        public AuthorityGroupDetail(EntityRef authorityGroupRef, string name, string description, bool dataGroup, List<AuthorityTokenSummary> authorityTokens)
        {
            AuthorityGroupRef = authorityGroupRef;
            Name = name;
            AuthorityTokens = authorityTokens;
            Description = description;
            DataGroup = dataGroup;
        }

        public AuthorityGroupDetail()
        {
            AuthorityTokens = new List<AuthorityTokenSummary>();
        }

        [DataMember]
        public EntityRef AuthorityGroupRef;

        [DataMember]
        public string Name;

        [DataMember]
        public List<AuthorityTokenSummary> AuthorityTokens;

        [DataMember]
        public string Description;

        [DataMember]
        public bool DataGroup;
    }
}
