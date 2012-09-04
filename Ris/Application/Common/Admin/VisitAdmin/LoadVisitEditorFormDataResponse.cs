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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class LoadVisitEditorFormDataResponse : DataContractBase
    {
        [DataMember]
        public List<EnumValueInfo> VisitNumberAssigningAuthorityChoices;

        [DataMember]
        public List<EnumValueInfo> PatientClassChoices;

        [DataMember]
        public List<EnumValueInfo> PatientTypeChoices;

        [DataMember]
        public List<EnumValueInfo> AdmissionTypeChoices;

        [DataMember]
        public List<EnumValueInfo> AmbulatoryStatusChoices;

        [DataMember]
        public List<EnumValueInfo> VisitLocationRoleChoices;

        [DataMember]
        public List<EnumValueInfo> VisitPractitionerRoleChoices;

        [DataMember]
        public List<EnumValueInfo> VisitStatusChoices;

        [DataMember]
        public List<FacilitySummary> FacilityChoices;

		[DataMember]
		public List<LocationSummary> CurrentLocationChoices;
	}
}
