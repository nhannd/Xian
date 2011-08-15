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
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	public interface IStudyRootStudyIdentifier : IStudyRootData, IStudyIdentifier
	{ }

	/// <summary>
	/// Study Root Query identifier for a study.
	/// </summary>
	[DataContract(Namespace = QueryNamespace.Value)]
	public class StudyRootStudyIdentifier : StudyIdentifier, IStudyRootStudyIdentifier
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

		#endregion

		#region Public Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public StudyRootStudyIdentifier()
		{
		}

		public StudyRootStudyIdentifier(IStudyRootStudyIdentifier other)
			: base(other)
		{
			CopyFrom(other);
		}

		public StudyRootStudyIdentifier(IStudyRootData other, IIdentifier identifier)
			: base(other, identifier)
		{
			CopyFrom(other);
		}

		public StudyRootStudyIdentifier(IStudyRootData other)
			: base(other)
		{
			CopyFrom(other);
		}

		public StudyRootStudyIdentifier(IPatientData patientData, IStudyIdentifier identifier)
			: base(identifier)
		{
			CopyFrom(patientData);
		}

		public StudyRootStudyIdentifier(IPatientData patientData, IStudyData studyData, IIdentifier identifier)
			: base(studyData, identifier)
		{
			CopyFrom(patientData);
		}

		/// <summary>
		/// Creates an instance of <see cref="StudyRootStudyIdentifier"/> from a <see cref="DicomAttributeCollection"/>.
		/// </summary>
		public StudyRootStudyIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}

		#endregion

		private void CopyFrom(IPatientData other)
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
		}

		#region Public Properties

		/// <summary>
		/// Gets or sets the patient id of the identified study.
		/// </summary>
		[DicomField(DicomTags.PatientId, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		/// <summary>
		/// Gets or sets the patient's name for the identified study.
		/// </summary>
		[DicomField(DicomTags.PatientsName, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsName
		{
			get { return _patientsName; }
			set { _patientsName = value; }
		}

		/// <summary>
		/// Gets or sets the patient's birth date for the identified study.
		/// </summary>
		[DicomField(DicomTags.PatientsBirthDate, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsBirthDate
		{
			get { return _patientsBirthDate; }
			set { _patientsBirthDate = value; }
		}

		/// <summary>
		/// Gets or sets the patient's birth time for the identified study.
		/// </summary>
		[DicomField(DicomTags.PatientsBirthTime, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsBirthTime
		{
			get { return _patientsBirthTime; }
			set { _patientsBirthTime = value; }
		}

		/// <summary>
		/// Gets or sets the patient's sex for the identified study.
		/// </summary>
		[DicomField(DicomTags.PatientsSex, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string PatientsSex
		{
			get { return _patientsSex; }
			set { _patientsSex = value; }
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

		public override string ToString()
		{
			return String.Format("{0} | {1} | {2}", this.PatientsName, this.PatientId, base.ToString());
		}
	}
}
