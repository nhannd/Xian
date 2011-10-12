#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	//NOTE: internal for now because we don't actually implement IPatientRootQuery anywhere.

	internal interface IPatientRootPatientIdentifier : IPatientRootData, IIdentifier
	{ }

	[DataContract(Namespace = QueryNamespace.Value)]
	internal class PatientRootPatientIdentifier : Identifier, IPatientRootData
	{
		#region Private Fields

		private string _patientId;
		private string _patientsName;
		private string _patientsBirthDate;
		private string _patientsBirthTime;
		private string _patientsSex;

		private string _patientSpeciesDescription;
		private string _patientSpeciesCodeSequenceCodingSchemeDesignator;
		private string _patientSpeciesCodeSequenceCodeValue;
		private string _patientSpeciesCodeSequenceCodeMeaning;
		private string _patientBreedDescription;
		private string _patientBreedCodeSequenceCodingSchemeDesignator;
		private string _patientBreedCodeSequenceCodeValue;
		private string _patientBreedCodeSequenceCodeMeaning;
		private string _responsiblePerson;
		private string _responsiblePersonRole;
		private string _responsibleOrganization;

		private int? _numberOfPatientRelatedStudies;
		private int? _numberOfPatientRelatedSeries;
		private int? _numberOfPatientRelatedInstances;

		#endregion

		#region Public Constructors

		public PatientRootPatientIdentifier()
		{
		}

		public PatientRootPatientIdentifier(IPatientRootPatientIdentifier other)
			: base(other)
		{
			CopyFrom(other);
		}

		public PatientRootPatientIdentifier(IPatientRootData other, IIdentifier identifier)
			: base(identifier)
		{
			CopyFrom(other);
		}

		public PatientRootPatientIdentifier(IPatientRootData other)
		{
			CopyFrom(other);
		}

		private void CopyFrom(IPatientRootData other)
		{
			PatientId = other.PatientId;
			PatientsName = other.PatientsName;
			PatientsBirthDate = other.PatientsBirthDate;
			PatientsBirthTime = other.PatientsBirthTime;
			PatientsSex = other.PatientsSex;

			PatientSpeciesDescription = other.PatientSpeciesDescription;
			PatientSpeciesCodeSequenceCodingSchemeDesignator = other.PatientSpeciesCodeSequenceCodingSchemeDesignator;
			PatientSpeciesCodeSequenceCodeValue = other.PatientSpeciesCodeSequenceCodeValue;
			PatientSpeciesCodeSequenceCodeMeaning = other.PatientSpeciesCodeSequenceCodeMeaning;
			PatientBreedDescription = other.PatientBreedDescription;
			PatientBreedCodeSequenceCodingSchemeDesignator = other.PatientBreedCodeSequenceCodingSchemeDesignator;
			PatientBreedCodeSequenceCodeValue = other.PatientBreedCodeSequenceCodeValue;
			PatientBreedCodeSequenceCodeMeaning = other.PatientBreedCodeSequenceCodeMeaning;
			ResponsiblePerson = other.ResponsiblePerson;
			ResponsiblePersonRole = other.ResponsiblePersonRole;
			ResponsibleOrganization = other.ResponsibleOrganization;

			NumberOfPatientRelatedStudies = other.NumberOfPatientRelatedStudies;
			NumberOfPatientRelatedSeries = other.NumberOfPatientRelatedSeries;
			NumberOfPatientRelatedInstances = other.NumberOfPatientRelatedInstances;
		}

		public PatientRootPatientIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}

		#endregion

		#region Public Properties

		public override string QueryRetrieveLevel
		{
			get { return "PATIENT"; }
		}

		[DicomField(DicomTags.PatientId, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		[DicomField(DicomTags.PatientsName, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsName
		{
			get { return _patientsName; }
			set { _patientsName = value; }
		}

		[DicomField(DicomTags.PatientsBirthDate, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsBirthDate
		{
			get { return _patientsBirthDate; }
			set { _patientsBirthDate = value; }
		}

		[DicomField(DicomTags.PatientsBirthTime, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsBirthTime
		{
			get { return _patientsBirthTime; }
			set { _patientsBirthTime = value; }
		}

		[DicomField(DicomTags.PatientsSex, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsSex
		{
			get { return _patientsSex; }
			set { _patientsSex = value; }
		}

		[DicomField(DicomTags.NumberOfPatientRelatedStudies, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public int? NumberOfPatientRelatedStudies
		{
			get { return _numberOfPatientRelatedStudies; }
			set { _numberOfPatientRelatedStudies = value; }
		}

		[DicomField(DicomTags.NumberOfPatientRelatedSeries, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public int? NumberOfPatientRelatedSeries
		{
			get { return _numberOfPatientRelatedSeries; }
			set { _numberOfPatientRelatedSeries = value; }
		}

		[DicomField(DicomTags.NumberOfPatientRelatedInstances, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public int? NumberOfPatientRelatedInstances
		{
			get { return _numberOfPatientRelatedInstances; }
			set { _numberOfPatientRelatedInstances = value; }
		}

		#region Species

		[DicomField(DicomTags.PatientSpeciesDescription)]
		[DataMember(IsRequired = false)]
		public string PatientSpeciesDescription
		{
			get { return _patientSpeciesDescription; }
			set { _patientSpeciesDescription = value; }
		}

		[DicomField(DicomTags.CodingSchemeDesignator, DicomTags.PatientSpeciesCodeSequence)]
		[DataMember(IsRequired = false)]
		public string PatientSpeciesCodeSequenceCodingSchemeDesignator
		{
			get { return _patientSpeciesCodeSequenceCodingSchemeDesignator; }
			set { _patientSpeciesCodeSequenceCodingSchemeDesignator = value; }
		}

		[DicomField(DicomTags.CodeValue, DicomTags.PatientSpeciesCodeSequence)]
		[DataMember(IsRequired = false)]
		public string PatientSpeciesCodeSequenceCodeValue
		{
			get { return _patientSpeciesCodeSequenceCodeValue; }
			set { _patientSpeciesCodeSequenceCodeValue = value; }
		}

		[DicomField(DicomTags.CodeMeaning, DicomTags.PatientSpeciesCodeSequence)]
		[DataMember(IsRequired = false)]
		public string PatientSpeciesCodeSequenceCodeMeaning
		{
			get { return _patientSpeciesCodeSequenceCodeMeaning; }
			set { _patientSpeciesCodeSequenceCodeMeaning = value; }
		}

		#endregion

		#region Breed

		[DicomField(DicomTags.PatientBreedDescription)]
		[DataMember(IsRequired = false)]
		public string PatientBreedDescription
		{
			get { return _patientBreedDescription; }
			set { _patientBreedDescription = value; }
		}

		[DicomField(DicomTags.CodingSchemeDesignator, DicomTags.PatientBreedCodeSequence)]
		[DataMember(IsRequired = false)]
		public string PatientBreedCodeSequenceCodingSchemeDesignator
		{
			get { return _patientBreedCodeSequenceCodingSchemeDesignator; }
			set { _patientBreedCodeSequenceCodingSchemeDesignator = value; }
		}

		[DicomField(DicomTags.CodeValue, DicomTags.PatientBreedCodeSequence)]
		[DataMember(IsRequired = false)]
		public string PatientBreedCodeSequenceCodeValue
		{
			get { return _patientBreedCodeSequenceCodeValue; }
			set { _patientBreedCodeSequenceCodeValue = value; }
		}

		[DicomField(DicomTags.CodeMeaning, DicomTags.PatientBreedCodeSequence)]
		[DataMember(IsRequired = false)]
		public string PatientBreedCodeSequenceCodeMeaning
		{
			get { return _patientBreedCodeSequenceCodeMeaning; }
			set { _patientBreedCodeSequenceCodeMeaning = value; }
		}

		#endregion

		#region Responsible Person/Organization

		[DicomField(DicomTags.ResponsiblePerson)]
		[DataMember(IsRequired = false)]
		public string ResponsiblePerson
		{
			get { return _responsiblePerson; }
			set { _responsiblePerson = value; }
		}

		[DicomField(DicomTags.ResponsiblePersonRole)]
		[DataMember(IsRequired = false)]
		public string ResponsiblePersonRole
		{
			get { return _responsiblePersonRole; }
			set { _responsiblePersonRole = value; }
		}

		[DicomField(DicomTags.ResponsibleOrganization)]
		[DataMember(IsRequired = false)]
		public string ResponsibleOrganization
		{
			get { return _responsibleOrganization; }
			set { _responsibleOrganization = value; }
		}

		#endregion

		#endregion
	}
}
