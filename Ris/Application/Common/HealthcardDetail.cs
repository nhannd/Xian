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
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class HealthcardDetail : DataContractBase
    {
        public HealthcardDetail(string id, EnumValueInfo assigningAuthority, string versionCode, DateTime? expiryDate)
        {
            this.Id = id;
            this.AssigningAuthority = assigningAuthority;
            this.VersionCode = versionCode;
            this.ExpiryDate = expiryDate;
        }

        public HealthcardDetail()
        {
        }

        [DataMember]
        public string Id;

        [DataMember]
        public EnumValueInfo AssigningAuthority;

        [DataMember]
        public string VersionCode;

        [DataMember]
        public DateTime? ExpiryDate;
    }
}