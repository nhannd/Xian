using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM SOP Instance.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The purpose of the <see cref="Sop"/> class (and derived classes) is to provide convenient access to Dicom
	/// Sop Instance data from arbitrary sources (Local File, WADO, streaming server, etc).  A number of properties are provided to 
	/// retrieve commonly accessed Dicom Data.  These properties are not intended to be purely representative of the
	/// actual data in the Dicom Header or to indicate when no data is available, but exist only to facilitate ease of use.
	/// That being said, some properties will return a default value when it is reasonable to do so.  However, no implementation 
	/// of <see cref="Sop"/> should simply manufacture data when none is present.
	/// </para>
	/// <para>
	/// Please follow these guidelines when implementing <see cref="Sop"/>-derived classes.
	/// 1) For (new) properties that represent Type 1 tags, override the <see cref="ValidateInternal"/> method (which is called by <see cref="Validate"/>),
	///    calling the base class' <see cref="ValidateInternal"/> first, then doing further validation on those properties.  Validation on other tags can be done
	///    at your discretion; for example, the <see cref="Sop"/> class validates that the <see cref="PatientId"/> property is non-empty, even though it is Type 2.
	/// 2) Override and implement all of the GetTag methods, but do not throw an exception.  See #4 for an explanation.
	/// 3) Override any properties that you wish to change the behaviour of how the values are returned.  All properties in <see cref="Sop"/> call
	///    one of the <see cref="GetTag"/> methods to retrieve the tag value.  You may wish to retrieve certain tags differently.
	/// 4) <see cref="GetTag"/> methods should make no assumptions about return values.  If a return value cannot be determined, the default value for the
	///    return type should be returned.  These are: "" for strings, 0 for any numeric type.  See <see cref="GetTag"/> for more details.
	/// 5) As mentioned above, when returning values from properties for which no data is available, return the default value 
	///    for the given type according to the following guidelines:
	///    - "" for string types
	///    - 0 for numeric types
	///    - null for reference types that are not strings
	///    - a valid default value when deemed appropriate
	/// 6) If a particular property's value is vital to functionality and has no reasonable default, it should be included in the <see cref="ValidateInternal"/>
	///    override and considered a fault if the value is invalid.  No attempt should be made to correct data in the properties when there is no 'reasonable default'.
	/// 7) If a reasonable default is returned from a property, the property's documentation should reflect that.
	/// 8) If the existence or 'true value' of a tag is important to your implementation, use one of the <see cref="GetTag"/> methods, rather than the
	///    existing property (or adding a property, for that matter).
	/// </para>
	/// </remarks>
	public abstract class Sop : ICacheableSop
	{
		private int _referenceCount;
		private Series _parentSeries;

		/// <summary>
		/// Gets the parent <see cref="Series"/>.
		/// </summary>
		public Series ParentSeries
		{
			get { return _parentSeries; }
			internal set { _parentSeries = value; }
		}

		/// <summary>
		/// Gets the underlying native DICOM object.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Sometimes, it is necessary to break the SOP abstraction and expose
		/// the underlying implementation object, since providing a wrapper for the object
		/// in <see cref="Sop"/> would be prohibitive because of the large number of
		/// methods that would have to be wrapped.
		/// </para>
		/// <para>
		/// Because <see cref="NativeDicomObject"/> returns an <see cref="Object"/>, it
		/// needs to be cast to a known class.  Note that if the interface to that
		/// known class changes at some point in the future, client code may break.
		/// For this reason, <see cref="NativeDicomObject"/> should be used
		/// carefully and sparingly.
		/// </para>
		/// </remarks>
		public abstract object NativeDicomObject { get; }

		/// <summary>
		/// Gets the Transfer Syntax UID.
		/// </summary>
		public virtual string TransferSyntaxUID
		{
			get
			{
				bool tagExists;
				string transferSyntaxInstanceUID;
				GetTag(Dcm.TransferSyntaxUID, out transferSyntaxInstanceUID, out tagExists);
				return transferSyntaxInstanceUID ?? "";
			}
		}

		/// <summary>
		/// Gets the SOP Instance UID.
		/// </summary>
		public virtual string SopInstanceUID
		{
			get
			{
				bool tagExists;
				string sopInstanceUID;
				GetTag(Dcm.SOPInstanceUID, out sopInstanceUID, out tagExists);
				return sopInstanceUID ?? "";
			}
		}

		#region Patient Module
		
		/// <summary>
		/// Gets the patient's name.
		/// </summary>
		public virtual PersonName PatientsName
		{
			get
			{
				bool tagExists;
				string patientsName;
				GetTag(Dcm.PatientsName, out patientsName, out tagExists);
				return new PersonName(patientsName ?? "");
			}
		}

		/// <summary>
		/// Gets the patient ID.
		/// </summary>
		public virtual string PatientId
		{
			get
			{
				bool tagExists;
				string patientId;
				GetTag(Dcm.PatientId, out patientId, out tagExists);
				return patientId ?? "";
			}
		}

		/// <summary>
		/// Gets the patient's birth date.
		/// </summary>
		public virtual string PatientsBirthDate
		{
			get
			{
				bool tagExists;
				string patientsBirthDate;
				GetTag(Dcm.PatientsBirthDate, out patientsBirthDate, out tagExists);
				return patientsBirthDate ?? "";
			}
		}

		/// <summary>
		/// Gets the patient's sex.
		/// </summary>
		public virtual string PatientsSex
		{
			get
			{
				bool tagExists;
				string patientsSex;
				GetTag(Dcm.PatientsSex, out patientsSex, out tagExists);
				return patientsSex ?? "";
			}
		}

		#endregion	

		#region General Study Module

		/// <summary>
		/// Gets the Study Instance UID.
		/// </summary>
		public virtual string StudyInstanceUID
		{
			get
			{
				bool tagExists;
				string studyInstanceUID;
				GetTag(Dcm.StudyInstanceUID, out studyInstanceUID, out tagExists);
				return studyInstanceUID ?? "";
			}
		}

		/// <summary>
		/// Gets the study date.
		/// </summary>
		public virtual string StudyDate
		{
			get
			{
				bool tagExists;
				string studyDate;
				GetTag(Dcm.StudyDate, out studyDate, out tagExists);
				return studyDate ?? "";
			}
		}

		/// <summary>
		/// Gets the study time.
		/// </summary>
		public virtual string StudyTime
		{
			get
			{
				bool tagExists;
				string studyTime;
				GetTag(Dcm.StudyTime, out studyTime, out tagExists);
				return studyTime ?? "";
			}
		}

		/// <summary>
		/// Gets the referring physician's name.
		/// </summary>
		public virtual PersonName ReferringPhysiciansName
		{
			get
			{
				bool tagExists;
				string referringPhysiciansName;
				GetTag(Dcm.ReferringPhysiciansName, out referringPhysiciansName, out tagExists);
				return new PersonName(referringPhysiciansName ?? "");
			}
		}

		/// <summary>
		/// Gets the accession number.
		/// </summary>
		public virtual string AccessionNumber
		{
			get
			{
				bool tagExists;
				string accessionNumber;
				GetTag(Dcm.AccessionNumber, out accessionNumber, out tagExists);
				return accessionNumber ?? "";
			}
		}

		/// <summary>
		/// Gets the study description.
		/// </summary>
		public virtual string StudyDescription
		{
			get
			{
				bool tagExists;
				string studyDescription;
				GetTag(Dcm.StudyDescription, out studyDescription, out tagExists);
				return studyDescription ?? "";
			}
		}

		/// <summary>
		/// Gets the names of physicians reading the study.
		/// </summary>
		public virtual PersonName[] NameOfPhysiciansReadingStudy
		{
			get
			{
				bool tagExists;
				string nameOfPhysiciansReadingStudy;
				GetTagArray(Dcm.NameOfPhysiciansReadingStudy, out nameOfPhysiciansReadingStudy, out tagExists);
				return VMStringConverter.ToPersonNameArray(nameOfPhysiciansReadingStudy);
			}
		}

		#endregion

		#region Patient Study Module

		/// <summary>
		/// Gets the admitting diagnoses descriptions.
		/// </summary>
		public virtual string[] AdmittingDiagnosesDescription
		{
			get
			{
				bool tagExists;
				string admittingDiagnosesDescription;
				GetTagArray(Dcm.AdmittingDiagnosesDescription, out admittingDiagnosesDescription, out tagExists);
				return VMStringConverter.ToStringArray(admittingDiagnosesDescription);
			}
		}

		/// <summary>
		/// Gets the patient's age.
		/// </summary>
		public virtual string PatientsAge
		{
			get
			{
				bool tagExists;
				string patientsAge;
				GetTag(Dcm.PatientsAge, out patientsAge, out tagExists);
				return patientsAge ?? "";
			}
		}

		/// <summary>
		/// Gets the additional patient's history.
		/// </summary>
		public virtual string AdditionalPatientsHistory
		{
			get
			{
				bool tagExists;
				string additionalPatientsHistory;
				GetTag(Dcm.AdditionalPatientHistory, out additionalPatientsHistory, out tagExists);
				return additionalPatientsHistory ?? "";
			}
		}

		#endregion

		#region General Equipment Module

		/// <summary>
		/// Gets the manufacturer.
		/// </summary>
		public virtual string Manufacturer
		{
			get
			{
				bool tagExists;
				string manufacturer;
				GetTag(Dcm.Manufacturer, out manufacturer, out tagExists);
				return manufacturer ?? "";
			}
		}

		/// <summary>
		/// Gets the institution name.
		/// </summary>
		public virtual string InstitutionName
		{
			get
			{
				bool tagExists;
				string institutionName;
				GetTag(Dcm.InstitutionName, out institutionName, out tagExists);
				return institutionName ?? "";
			}
		}

		/// <summary>
		/// Gets the station name.
		/// </summary>
		public virtual string StationName
		{
			get
			{
				bool tagExists;
				string stationName;
				GetTag(Dcm.StationName, out stationName, out tagExists);
				return stationName ?? "";
			}
		}

		/// <summary>
		/// Gets the institutional department name.
		/// </summary>
		public virtual string InstitutionalDepartmentName
		{
			get
			{
				bool tagExists;
				string institutionalDepartmentName;
				GetTag(Dcm.InstitutionalDepartmentName, out institutionalDepartmentName, out tagExists);
				return institutionalDepartmentName ?? "";
			}
		}

		/// <summary>
		/// Gets the manufacturer's model name.
		/// </summary>
		public virtual string ManufacturersModelName
		{
			get
			{
				bool tagExists;
				string manufacturersModelName;
				GetTag(Dcm.ManufacturersModelName, out manufacturersModelName, out tagExists);
				return manufacturersModelName ?? "";
			}
		}

		#endregion

		#region General Series Module

		/// <summary>
		/// Gets the modality.
		/// </summary>
		public virtual string Modality
		{
			get
			{
				bool tagExists;
				string modality;
				GetTag(Dcm.Modality, out modality, out tagExists);
				return modality ?? "";
			}
		}

		/// <summary>
		/// Gets the Series Instance UID.
		/// </summary>
		public virtual string SeriesInstanceUID
		{
			get
			{
				bool tagExists;
				string seriesInstanceUID;
				GetTag(Dcm.SeriesInstanceUID, out seriesInstanceUID, out tagExists);
				return seriesInstanceUID ?? "";
			}
		}

		/// <summary>
		/// Gets the series number.
		/// </summary>
		public virtual int SeriesNumber
		{
			get
			{
				bool tagExists;
				int seriesNumber;
				GetTag(Dcm.SeriesNumber, out seriesNumber, out tagExists);
				return seriesNumber;
			}
		}

		/// <summary>
		/// Gets the laterality.
		/// </summary>
		public virtual string Laterality
		{
			get
			{
				bool tagExists;
				string laterality;
				GetTag(Dcm.Laterality, out laterality, out tagExists);
				return laterality ?? "";
			}
		}

		/// <summary>
		/// Gets the series date.
		/// </summary>
		public virtual string SeriesDate
		{
			get
			{
				bool tagExists;
				string seriesDate;
				GetTag(Dcm.SeriesDate, out seriesDate, out tagExists);
				return seriesDate ?? "";
			}
		}

		/// <summary>
		/// Gets the series time.
		/// </summary>
		public virtual string SeriesTime
		{
			get
			{
				bool tagExists;
				string seriesTime;
				GetTag(Dcm.SeriesTime, out seriesTime, out tagExists);
				return seriesTime ?? "";
			}
		}

		/// <summary>
		/// Gets the names of performing physicians.
		/// </summary>
		public virtual PersonName[] PerformingPhysiciansName
		{
			get
			{
				bool tagExists;
				string performingPhysiciansNames;
				GetTagArray(Dcm.PerformingPhysiciansName, out performingPhysiciansNames, out tagExists);
				return VMStringConverter.ToPersonNameArray(performingPhysiciansNames);
			}
		}

		/// <summary>
		/// Gets the protocol name.
		/// </summary>
		public virtual string ProtocolName
		{
			get
			{
				bool tagExists;
				string protocolName;
				GetTag(Dcm.ProtocolName, out protocolName, out tagExists);
				return protocolName ?? "";
			}
		}

		/// <summary>
		/// Gets the series description.
		/// </summary>
		public virtual string SeriesDescription
		{
			get
			{
				bool tagExists;
				string seriesDescription;
				GetTag(Dcm.SeriesDescription, out seriesDescription, out tagExists);
				return seriesDescription ?? "";
			}
		}

		/// <summary>
		/// Gets the names of operators.
		/// </summary>
		public virtual PersonName[] OperatorsName
		{
			get
			{
				bool tagExists;
				string operatorsNames;
				GetTagArray(Dcm.OperatorsName, out operatorsNames, out tagExists);
				return VMStringConverter.ToPersonNameArray(operatorsNames);
			}
		}

		/// <summary>
		/// Gets the body part examined.
		/// </summary>
		public virtual string BodyPartExamined
		{
			get
			{
				bool tagExists;
				string bodyPartExamined;
				GetTag(Dcm.BodyPartExamined, out bodyPartExamined, out tagExists);
				return bodyPartExamined ?? "";
			}
		}

		/// <summary>
		/// Gets the patient position.
		/// </summary>
		public virtual string PatientPosition
		{
			get
			{
				bool tagExists;
				string bodyPartExamined;
				GetTag(Dcm.BodyPartExamined, out bodyPartExamined, out tagExists);
				return bodyPartExamined ?? "";
			}
		}

		#endregion

		#region Dicom Tag Retrieval Methods

		/// <summary>
		/// Gets a DICOM tag (16 bit, unsigned).
		/// when a tag
		/// </summary>
		/// <remarks>
		/// GetTag methods should make no assumptions about what to return in the <paramref name="value"/> parameter
		/// when a tag does not exist.  It should simply return the default value for <paramref name="value"/>'s Type,
		/// which is either null, 0 or "" depending on whether it is a reference or value Type.  Similarly, no data validation
		/// should be done in these methods either.  It is expected that the unaltered tag value will be returned.
		/// </remarks>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out ushort value, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag with value multiplicity (16 bit, unsigned).
		/// </summary>
		/// <remarks>
		/// GetTag methods should make no assumptions about what to return in the <paramref name="value"/> parameter
		/// when a tag does not exist.  It should simply return the default value for <paramref name="value"/>'s Type,
		/// which is either null, 0 or "" depending on whether it is a reference or value Type.  Similarly, no data validation
		/// should be done in these methods either.  It is expected that the unaltered tag value will be returned.
		/// </remarks>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out ushort value, uint position, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag (integer).
		/// </summary>
		/// <remarks>
		/// GetTag methods should make no assumptions about what to return in the <paramref name="value"/> parameter
		/// when a tag does not exist.  It should simply return the default value for <paramref name="value"/>'s Type,
		/// which is either null, 0 or "" depending on whether it is a reference or value Type.  Similarly, no data validation
		/// should be done in these methods either.  It is expected that the unaltered tag value will be returned.
		/// </remarks>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out int value, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag with value multiplicity (integer).
		/// </summary>
		/// <remarks>
		/// GetTag methods should make no assumptions about what to return in the <paramref name="value"/> parameter
		/// when a tag does not exist.  It should simply return the default value for <paramref name="value"/>'s Type,
		/// which is either null, 0 or "" depending on whether it is a reference or value Type.  Similarly, no data validation
		/// should be done in these methods either.  It is expected that the unaltered tag value will be returned.
		/// </remarks>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out int value, uint position, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag (double).
		/// </summary>
		/// <remarks>
		/// GetTag methods should make no assumptions about what to return in the <paramref name="value"/> parameter
		/// when a tag does not exist.  It should simply return the default value for <paramref name="value"/>'s Type,
		/// which is either null, 0 or "" depending on whether it is a reference or value Type.  Similarly, no data validation
		/// should be done in these methods either.  It is expected that the unaltered tag value will be returned.
		/// </remarks>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out double value, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag with value multiplicity (double).
		/// </summary>
		/// <remarks>
		/// GetTag methods should make no assumptions about what to return in the <paramref name="value"/> parameter
		/// when a tag does not exist.  It should simply return the default value for <paramref name="value"/>'s Type,
		/// which is either null, 0 or "" depending on whether it is a reference or value Type.  Similarly, no data validation
		/// should be done in these methods either.  It is expected that the unaltered tag value will be returned.
		/// </remarks>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out double value, uint position, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag (string).
		/// </summary>
		/// <remarks>
		/// GetTag methods should make no assumptions about what to return in the <paramref name="value"/> parameter
		/// when a tag does not exist.  It should simply return the default value for <paramref name="value"/>'s Type,
		/// which is either null, 0 or "" depending on whether it is a reference or value Type.  Similarly, no data validation
		/// should be done in these methods either.  It is expected that the unaltered tag value will be returned.
		/// </remarks>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out string value, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag with value multiplicity (string).
		/// </summary>
		/// <remarks>
		/// GetTag methods should make no assumptions about what to return in the <paramref name="value"/> parameter
		/// when a tag does not exist.  It should simply return the default value for <paramref name="value"/>'s Type,
		/// which is either null, 0 or "" depending on whether it is a reference or value Type.  Similarly, no data validation
		/// should be done in these methods either.  It is expected that the unaltered tag value will be returned.
		/// </remarks>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out string value, uint position, out bool tagExists);

		/// <summary>
		/// Gets an entire DICOM tag to a string, encoded as a Dicom array if VM > 1.
		/// </summary>
		/// <remarks>
		/// GetTag methods should make no assumptions about what to return in the <paramref name="value"/> parameter
		/// when a tag does not exist.  It should simply return the default value for <paramref name="value"/>'s Type,
		/// which is either null, 0 or "" depending on whether it is a reference or value Type.  Similarly, no data validation
		/// should be done in these methods either.  It is expected that the unaltered tag value will be returned.
		/// </remarks>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTagArray(DcmTagKey tag, out string value, out bool tagExists);

		#endregion

		#region Disposal

		/// <summary>
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(e);
			}
		}

		public override string ToString()
		{
			return this.SopInstanceUID;
		}

		#endregion

		#endregion

		#region ICacheableSop Members

		string ICacheableSop.SopInstanceUID
		{
			get { return this.SopInstanceUID; }
		}

		bool IReferenceCountable.IsReferenceCountZero
		{
			get { return (_referenceCount == 0); }
		}

		void ICacheableSop.Load()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		void ICacheableSop.Unload()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		void IReferenceCountable.IncrementReferenceCount()
		{
			_referenceCount++;
		}

		void IReferenceCountable.DecrementReferenceCount()
		{
			if (_referenceCount > 0)
				_referenceCount--;
		}

#if UNIT_TESTS
		int IReferenceCountable.ReferenceCount
		{
			get { return _referenceCount; }
		}
#endif

		#endregion

		/// <summary>
		/// The <see cref="Sop"/> class (and derived classes) should not validate tag values from 
		/// within its properties, but instead clients should call this method at an appropriate time
		/// to determine whether or not the <see cref="Sop"/> should be used or discarded as invalid.
		/// </summary>
		/// <exception cref="SopValidationException">Thrown when validation fails.</exception>
		public void Validate()
		{
			try
			{
				ValidateInternal();
			}
			catch (Exception e)
			{
				if (e is SopValidationException)
					throw;

				throw new SopValidationException(SR.ExceptionSopInstanceValidationFailed, e);
			}
		}

		/// <summary>
		/// Validates the <see cref="Sop"/> object.
		/// </summary>
		/// <remarks>
		/// Derived classes should call the base class implementation first, and then do further validation.
		/// The <see cref="Sop"/> class validates properties deemed vital to usage of the object.
		/// </remarks>
		/// <exception cref="SopValidationException">Thrown when validation fails.</exception>
		protected virtual void ValidateInternal()
		{ 
			DicomValidator.ValidateSOPInstanceUID(this.SopInstanceUID);
			DicomValidator.ValidateSeriesInstanceUID(this.SeriesInstanceUID);
			DicomValidator.ValidateStudyInstanceUID(this.StudyInstanceUID);

			ValidatePatientId();
		}

		private void ValidatePatientId()
		{
			//Patient ID is a Type 2 tag, so this is our own restriction, not a Dicom Restriction.
			if (String.IsNullOrEmpty(this.PatientId) || this.PatientId.TrimEnd(' ').Length == 0)
				throw new SopValidationException(SR.ExceptionInvalidPatientID);
		}
	}
}
