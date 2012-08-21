#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Extended.Common.PatientReconciliation
{
    [DataContract]
    public class PatientProfileDiff : DataContractBase
    {
        [DataMember]
        public string RightProfileAssigningAuthority;

        [DataMember]
        public string LeftProfileAssigningAuthority;

        [DataMember]
        public PropertyDiff Healthcard;

        [DataMember]
        public PropertyDiff FamilyName;

        [DataMember]
        public PropertyDiff MiddleName;

        [DataMember]
        public PropertyDiff GivenName;

        [DataMember]
        public PropertyDiff DateOfBirth;

        [DataMember]
        public PropertyDiff Sex;

        [DataMember]
        public PropertyDiff HomePhone;

        [DataMember]
        public PropertyDiff HomeAddress;

        [DataMember]
        public PropertyDiff WorkPhone;

        [DataMember]
        public PropertyDiff WorkAddress;
    }

    [DataContract]
    public class PropertyDiff : DataContractBase
    {
        [DataMember]
        public bool IsDiscrepant;

        [DataMember]
        public string AlignedLeftValue;

        [DataMember]
        public string AlignedRightValue;

        [DataMember]
        public string DiffMask;
    }
}
