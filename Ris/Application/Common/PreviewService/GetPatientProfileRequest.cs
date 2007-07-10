using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [DataContract]
    public class GetPatientProfileRequest : DataContractBase
    {
        public GetPatientProfileRequest()
        {
            includeAddresses = false;
            includeContactPersons = false;
            includeEmailAddresses = false;
            includeTelephoneNumbers = false;
            includeNotes = false;
        }

        [DataMember]
        public bool includeAddresses;

        [DataMember]
        public bool includeContactPersons;

        [DataMember]
        public bool includeEmailAddresses;

        [DataMember]
        public bool includeTelephoneNumbers;

        [DataMember]
        public bool includeNotes;

    }
}
