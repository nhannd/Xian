#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod.Iods
{
	/// <summary>
	/// EncapsulatedPdf IOD
	/// </summary>
	/// <remarks>
	/// <para>As defined in the DICOM Standard 2009, Part 3, Section A.45.1 (Table A.45.1-1)</para>
	/// </remarks>
	public class EncapsulatedPdfIod
	{
		private readonly IDicomAttributeProvider _dicomAttributeProvider;
		private readonly PatientModuleIod _patientModule;
		private readonly ClinicalTrialSubjectModuleIod _clinicalTrialSubjectModule;
		private readonly GeneralStudyModuleIod _generalStudyModule;
		private readonly PatientStudyModuleIod _patientStudyModule;
		private readonly ClinicalTrialStudyModuleIod _clinicalTrialStudyModule;
		private readonly EncapsulatedDocumentSeriesModuleIod _encapsulatedDocumentSeriesModule;
		private readonly ClinicalTrialSeriesModuleIod _clinicalTrialSeriesModule;
		private readonly GeneralEquipmentModuleIod _generalEquipmentModule;
		private readonly ScEquipmentModuleIod _scEquipmentModule;
		private readonly EncapsulatedDocumentModuleIod _encapsulatedDocumentModule;
		private readonly SopCommonModuleIod _sopCommonModule;

		/// <summary>
		/// Initializes a new instance of the <see cref="EncapsulatedPdfIod"/> class.
		/// </summary>	
		public EncapsulatedPdfIod()
			: this(new DicomAttributeCollection()) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="EncapsulatedPdfIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute provider.</param>
		public EncapsulatedPdfIod(IDicomAttributeProvider dicomAttributeProvider)
		{
			_dicomAttributeProvider = dicomAttributeProvider;

			_patientModule = new PatientModuleIod(_dicomAttributeProvider);
			_clinicalTrialSubjectModule = new ClinicalTrialSubjectModuleIod(_dicomAttributeProvider);
			_generalStudyModule = new GeneralStudyModuleIod(_dicomAttributeProvider);
			_patientStudyModule = new PatientStudyModuleIod(_dicomAttributeProvider);
			_clinicalTrialStudyModule = new ClinicalTrialStudyModuleIod(_dicomAttributeProvider);
			_encapsulatedDocumentSeriesModule = new EncapsulatedDocumentSeriesModuleIod(_dicomAttributeProvider);
			_clinicalTrialSeriesModule = new ClinicalTrialSeriesModuleIod(_dicomAttributeProvider);
			_generalEquipmentModule = new GeneralEquipmentModuleIod(_dicomAttributeProvider);
			_scEquipmentModule = new ScEquipmentModuleIod(_dicomAttributeProvider);
			_encapsulatedDocumentModule = new EncapsulatedDocumentModuleIod(_dicomAttributeProvider);
			_sopCommonModule = new SopCommonModuleIod(_dicomAttributeProvider);
		}

		public PatientModuleIod Patient
		{
			get { return _patientModule; }
		}

		public ClinicalTrialSubjectModuleIod ClinicalTrialSubject
		{
			get { return _clinicalTrialSubjectModule; }
		}

		public GeneralStudyModuleIod GeneralStudy
		{
			get { return _generalStudyModule; }
		}

		public PatientStudyModuleIod PatientStudy
		{
			get { return _patientStudyModule; }
		}

		public ClinicalTrialStudyModuleIod ClinicalTrialStudy
		{
			get { return _clinicalTrialStudyModule; }
		}

		public EncapsulatedDocumentSeriesModuleIod EncapsulatedDocumentSeries
		{
			get { return _encapsulatedDocumentSeriesModule; }
		}

		public ClinicalTrialSeriesModuleIod ClinicalTrialSeries
		{
			get { return _clinicalTrialSeriesModule; }
		}

		public GeneralEquipmentModuleIod GeneralEquipment
		{
			get { return _generalEquipmentModule; }
		}

		public ScEquipmentModuleIod ScEquipment
		{
			get { return _scEquipmentModule; }
		}

		public EncapsulatedDocumentModuleIod EncapsulatedDocument
		{
			get { return _encapsulatedDocumentModule; }
		}

		public SopCommonModuleIod SopCommon
		{
			get { return _sopCommonModule; }
		}

		public bool HasClinicalTrialSubjectModule { get; set; }

		public bool HasPatientStudyModule { get; set; }

		public bool HasClinicalTrialStudyModule { get; set; }

		public bool HasClinicalTrialSeriesModule { get; set; }
	}
}