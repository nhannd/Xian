using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

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
