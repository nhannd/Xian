#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
    [DataContract]
    public class GetPatientProfileDetailRequest : DataContractBase
    {
        public GetPatientProfileDetailRequest(EntityRef patientProfileRef,
            bool includeAddresses,
            bool includeContactPersons,
            bool includeEmailAddresses,
            bool includeTelephoneNumbers,
            bool includeNotes,
            bool includeAttachments,
            bool includeAlerts,
			bool includeAllergies)
        {
            this.PatientProfileRef = patientProfileRef;
            this.IncludeAddresses = includeAddresses;
            this.IncludeContactPersons = includeContactPersons;
            this.IncludeEmailAddresses = includeEmailAddresses;
            this.IncludeTelephoneNumbers = includeTelephoneNumbers;
            this.IncludeNotes = includeNotes;
            this.IncludeAttachments = includeAttachments;
            this.IncludeAlerts = includeAlerts;
        	this.IncludeAllergies = includeAllergies;
        }

        public GetPatientProfileDetailRequest()
        {
        }

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public bool IncludeAddresses;

        [DataMember]
        public bool IncludeContactPersons;

        [DataMember]
        public bool IncludeEmailAddresses;

        [DataMember]
        public bool IncludeTelephoneNumbers;

        [DataMember]
        public bool IncludeNotes;

        [DataMember]
        public bool IncludeAttachments;

        [DataMember]
        public bool IncludeAlerts;

		[DataMember]
		public bool IncludeAllergies;
    }
}
