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

	public interface IPatientRootPatientIdentifier : IPatientRootData, IIdentifier
	{ }

	[DataContract(Namespace = QueryNamespace.Value)]
	public class PatientRootPatientIdentifier : Identifier, IPatientRootData
	{
		#region Private Fields

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

	    [DicomField(DicomTags.PatientId, CreateEmptyElement = true, SetNullValueIfEmpty = true), DataMember(IsRequired = false)]
	    public string PatientId { get; set; }

	    [DicomField(DicomTags.PatientsName, CreateEmptyElement = true, SetNullValueIfEmpty = true), DataMember(IsRequired = false)]
	    public string PatientsName { get; set; }

	    [DicomField(DicomTags.PatientsBirthDate, CreateEmptyElement = true, SetNullValueIfEmpty = true), DataMember(IsRequired = false)]
	    public string PatientsBirthDate { get; set; }

	    [DicomField(DicomTags.PatientsBirthTime, CreateEmptyElement = true, SetNullValueIfEmpty = true), DataMember(IsRequired = false)]
	    public string PatientsBirthTime { get; set; }

	    [DicomField(DicomTags.PatientsSex, CreateEmptyElement = true, SetNullValueIfEmpty = true), DataMember(IsRequired = false)]
	    public string PatientsSex { get; set; }

	    [DicomField(DicomTags.NumberOfPatientRelatedStudies, CreateEmptyElement = true, SetNullValueIfEmpty = true), DataMember(IsRequired = false)]
	    public int? NumberOfPatientRelatedStudies { get; set; }

	    [DicomField(DicomTags.NumberOfPatientRelatedSeries, CreateEmptyElement = true, SetNullValueIfEmpty = true), DataMember(IsRequired = false)]
	    public int? NumberOfPatientRelatedSeries { get; set; }

	    [DicomField(DicomTags.NumberOfPatientRelatedInstances, CreateEmptyElement = true, SetNullValueIfEmpty = true), DataMember(IsRequired = false)]
	    public int? NumberOfPatientRelatedInstances { get; set; }

	    #region Species

	    [DicomField(DicomTags.PatientSpeciesDescription), DataMember(IsRequired = false)]
	    public string PatientSpeciesDescription { get; set; }

	    [DicomField(DicomTags.CodingSchemeDesignator, DicomTags.PatientSpeciesCodeSequence), DataMember(IsRequired = false)]
	    public string PatientSpeciesCodeSequenceCodingSchemeDesignator { get; set; }

	    [DicomField(DicomTags.CodeValue, DicomTags.PatientSpeciesCodeSequence), DataMember(IsRequired = false)]
	    public string PatientSpeciesCodeSequenceCodeValue { get; set; }

	    [DicomField(DicomTags.CodeMeaning, DicomTags.PatientSpeciesCodeSequence), DataMember(IsRequired = false)]
	    public string PatientSpeciesCodeSequenceCodeMeaning { get; set; }

	    #endregion

		#region Breed

	    [DicomField(DicomTags.PatientBreedDescription), DataMember(IsRequired = false)]
	    public string PatientBreedDescription { get; set; }

	    [DicomField(DicomTags.CodingSchemeDesignator, DicomTags.PatientBreedCodeSequence), DataMember(IsRequired = false)]
	    public string PatientBreedCodeSequenceCodingSchemeDesignator { get; set; }

	    [DicomField(DicomTags.CodeValue, DicomTags.PatientBreedCodeSequence), DataMember(IsRequired = false)]
	    public string PatientBreedCodeSequenceCodeValue { get; set; }

	    [DicomField(DicomTags.CodeMeaning, DicomTags.PatientBreedCodeSequence), DataMember(IsRequired = false)]
	    public string PatientBreedCodeSequenceCodeMeaning { get; set; }

	    #endregion

		#region Responsible Person/Organization

	    [DicomField(DicomTags.ResponsiblePerson), DataMember(IsRequired = false)]
	    public string ResponsiblePerson { get; set; }

	    [DicomField(DicomTags.ResponsiblePersonRole), DataMember(IsRequired = false)]
	    public string ResponsiblePersonRole { get; set; }

	    [DicomField(DicomTags.ResponsibleOrganization), DataMember(IsRequired = false)]
	    public string ResponsibleOrganization { get; set; }

	    #endregion

		#endregion
	}
}
