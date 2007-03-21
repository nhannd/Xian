using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientReconciliation
{
    [DataContract]
    public class PatientProfileDiff : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public string RightProfileAssigningAuthority;

        [DataMember(IsRequired = true)]
        public string LeftProfileAssigningAuthority;

        [DataMember(IsRequired = true)]
        public PropertyDiff Healthcard;

        [DataMember(IsRequired = true)]
        public PropertyDiff FamilyName;

        [DataMember(IsRequired = true)]
        public PropertyDiff MiddleName;

        [DataMember(IsRequired = true)]
        public PropertyDiff GivenName;

        [DataMember(IsRequired = true)]
        public PropertyDiff DateOfBirth;

        [DataMember(IsRequired = true)]
        public PropertyDiff Sex;

        [DataMember(IsRequired = true)]
        public PropertyDiff HomePhone;

        [DataMember(IsRequired = true)]
        public PropertyDiff HomeAddress;

        [DataMember(IsRequired = true)]
        public PropertyDiff WorkPhone;

        [DataMember(IsRequired = true)]
        public PropertyDiff WorkAddress;
    }

    [DataContract]
    public class PropertyDiff : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public bool IsDiscrepant;

        [DataMember(IsRequired = true)]
        public string AlignedLeftValue;

        [DataMember(IsRequired = true)]
        public string AlignedRightValue;

        [DataMember(IsRequired = true)]
        public string DiffMask;
    }
}
