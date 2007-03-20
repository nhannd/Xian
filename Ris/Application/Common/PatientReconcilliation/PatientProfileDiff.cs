using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientReconcilliation
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
        public bool Discrepant;

        [DataMember]
        public string AlignedLeftValue;

        [DataMember]
        public string AlignedRightValue;

        [DataMember]
        public string DiffMask;
    }
}
