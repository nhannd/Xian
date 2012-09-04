#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
	[DataContract]
	public class LoadPatientProfileEditorFormDataResponse : DataContractBase
	{
		[DataMember]
		public List<EnumValueInfo> HealthcardAssigningAuthorityChoices;

		[DataMember]
		public List<EnumValueInfo> MrnAssigningAuthorityChoices;

		[DataMember]
		public List<EnumValueInfo> SexChoices;

		[DataMember]
		public List<EnumValueInfo> AddressTypeChoices;

		[DataMember]
		public List<EnumValueInfo> PhoneTypeChoices;

		[DataMember]
		public List<EnumValueInfo> ContactPersonTypeChoices;

		[DataMember]
		public List<EnumValueInfo> ContactPersonRelationshipChoices;

		[DataMember]
		public List<PatientNoteCategorySummary> NoteCategoryChoices;

		[DataMember]
		public List<EnumValueInfo> PrimaryLanguageChoices;

		[DataMember]
		public List<EnumValueInfo> ReligionChoices;

		[DataMember]
		public List<EnumValueInfo> AllergenTypeChoices;

		[DataMember]
		public List<EnumValueInfo> AllergySeverityChoices;

		[DataMember]
		public List<EnumValueInfo> AllergySensitivityTypeChoices;

		[DataMember]
		public List<EnumValueInfo> PersonRelationshipTypeChoices;
	}
}
