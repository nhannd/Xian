#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class PatientAllergyDetail : DataContractBase
	{
		// Default parameterless constructor for Jsml deserialization
		public PatientAllergyDetail()
		{
			this.ReporterName = new PersonNameDetail();
		}

		public PatientAllergyDetail(EnumValueInfo allergenType,
			string allergenDescription,
			EnumValueInfo severity,
			string reaction,
			EnumValueInfo sensitivityType,
			DateTime? onsetTime,
			DateTime? reportedTime,
			PersonNameDetail reporterName,
			EnumValueInfo reporterRelationshipType)
		{
			this.AllergenType = allergenType;
			this.AllergenDescription = allergenDescription;
			this.Severity = severity;
			this.Reaction = reaction;
			this.SensitivityType = sensitivityType;
			this.OnsetTime = onsetTime;
			this.ReportedTime = reportedTime;
			this.ReporterName = reporterName;
			this.ReporterRelationshipType = reporterRelationshipType;
		}

		[DataMember]
		public EnumValueInfo AllergenType;

		[DataMember]
		public string AllergenDescription;

		[DataMember]
		public EnumValueInfo Severity;

		[DataMember]
		public string Reaction;

		[DataMember]
		public EnumValueInfo SensitivityType;

		[DataMember]
		public DateTime? OnsetTime;

		[DataMember]
		public DateTime? ReportedTime;

		[DataMember]
		public PersonNameDetail ReporterName;

		[DataMember]
		public EnumValueInfo ReporterRelationshipType;
	}
}
