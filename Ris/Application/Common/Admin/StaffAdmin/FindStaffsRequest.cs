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
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
    public class FindStaffsRequest : DataContractBase
    {
        public FindStaffsRequest(string familyName, string givenName)
        {
            this.FamilyName = familyName;
            this.GivenName = givenName;
        }

        [DataMember]
        public string FamilyName;

        [DataMember]
        public string GivenName;
    }
}
