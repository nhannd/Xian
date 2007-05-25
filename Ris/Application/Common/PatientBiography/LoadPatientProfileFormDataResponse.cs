using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientBiography
{
    [DataContract]
    public class LoadPatientProfileFormDataResponse : DataContractBase
    {
        [DataMember]
        public List<EnumValueInfo> AddressTypeChoices;

        [DataMember]
        public List<EnumValueInfo> PhoneTypeChoices;

        [DataMember]
        public List<EnumValueInfo> ContactPersonTypeChoices;

        [DataMember]
        public List<EnumValueInfo> ContactPersonRelationshipChoices;
    }
}
