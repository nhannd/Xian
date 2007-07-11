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
		public abstract string TransferSyntaxUID { get; }

		/// <summary>
		/// Gets the SOP Instance UID.
		/// </summary>
		public abstract string SopInstanceUID { get; }
		
		#region Patient Module
		
		/// <summary>
		/// Gets the patient's name.
		/// </summary>
		public abstract PersonName PatientsName { get; }

		/// <summary>
		/// Gets the patient ID.
		/// </summary>
		public abstract string PatientId { get; }

		/// <summary>
		/// Gets the patient's birth date.
		/// </summary>
		public abstract string PatientsBirthDate { get; }

		/// <summary>
		/// Gets the patient's sex.
		/// </summary>
		public abstract string PatientsSex { get; }

		#endregion	

		#region General Study Module

		/// <summary>
		/// Gets the Study Instance UID.
		/// </summary>
		public abstract string StudyInstanceUID { get; }

		/// <summary>
		/// Gets the study date.
		/// </summary>
		public abstract string StudyDate { get; }

		/// <summary>
		/// Gets the study time.
		/// </summary>
		public abstract string StudyTime { get; }

		/// <summary>
		/// Gets the referring physician's name.
		/// </summary>
		public abstract PersonName ReferringPhysiciansName { get; }

		/// <summary>
		/// Gets the accession number.
		/// </summary>
		public abstract string AccessionNumber { get; }

		/// <summary>
		/// Gets the study description.
		/// </summary>
		public abstract string StudyDescription { get; }

		/// <summary>
		/// Gets the names of physicians reading the study.
		/// </summary>
		public abstract PersonName[] NameOfPhysiciansReadingStudy { get; }

		#endregion

		#region Patient Study Module

		/// <summary>
		/// Gets the admitting diagnoses descriptions.
		/// </summary>
		public abstract string[] AdmittingDiagnosesDescription { get; }

		/// <summary>
		/// Gets the patient's age.
		/// </summary>
		public abstract string PatientsAge { get; }

		/// <summary>
		/// Gets the additional patient's history.
		/// </summary>
		public abstract string AdditionalPatientsHistory { get; }

		#endregion

		#region General Equipment Module

		/// <summary>
		/// Gets the manufacturer.
		/// </summary>
		public abstract string Manufacturer { get; }

		/// <summary>
		/// Gets the institution name.
		/// </summary>
		public abstract string InstitutionName { get; }

		/// <summary>
		/// Gets the station name.
		/// </summary>
		public abstract string StationName { get; }

		/// <summary>
		/// Gets the institutional department name.
		/// </summary>
		public abstract string InstitutionalDepartmentName { get; }

		/// <summary>
		/// Gets the manufacturer's model name.
		/// </summary>
		public abstract string ManufacturersModelName { get; }

		#endregion

		#region General Series Module

		/// <summary>
		/// Gets the modality.
		/// </summary>
		public abstract string Modality { get; }

		/// <summary>
		/// Gets the Series Instance UID.
		/// </summary>
		public abstract string SeriesInstanceUID { get; }

		/// <summary>
		/// Gets the series number.
		/// </summary>
		public abstract int SeriesNumber { get; }

		/// <summary>
		/// Gets the laterality.
		/// </summary>
		public abstract string Laterality { get; }

		/// <summary>
		/// Gets the series date.
		/// </summary>
		public abstract string SeriesDate { get; }

		/// <summary>
		/// Gets the series time.
		/// </summary>
		public abstract string SeriesTime { get; }

		/// <summary>
		/// Gets the names of performing physicians.
		/// </summary>
		public abstract PersonName[] PerformingPhysiciansName { get; }

		/// <summary>
		/// Gets the protocol name.
		/// </summary>
		public abstract string ProtocolName { get; }

		/// <summary>
		/// Gets the series description.
		/// </summary>
		public abstract string SeriesDescription { get; }

		/// <summary>
		/// Gets the names of operators.
		/// </summary>
		public abstract PersonName[] OperatorsName { get; }

		/// <summary>
		/// Gets the body part examined.
		/// </summary>
		public abstract string BodyPartExamined { get; }

		/// <summary>
		/// Gets the patient position.
		/// </summary>
		public abstract string PatientPosition { get; }

		#endregion

		#region Dicom Tag Retrieval Methods

		/// <summary>
		/// Gets a DICOM tag (16 bit, unsigned).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out ushort val, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag with value multiplicity (16 bit, unsigned).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out ushort val, uint position, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag (integer).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out int val, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag with value multiplicity (integer).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out int val, uint position, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag (double).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out double val, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag with value multiplicity (double).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out double val, uint position, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag (string).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out string val, out bool tagExists);

		/// <summary>
		/// Gets a DICOM tag with value multiplicity (string).
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTag(DcmTagKey tag, out string val, uint position, out bool tagExists);

		/// <summary>
		/// Gets an entire DICOM tag to a string, encoded as a Dicom array if VM > 1.
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="val"></param>
		/// <param name="tagExists"></param>
		public abstract void GetTagArray(DcmTagKey tag, out string val, out bool tagExists);

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

		public override string ToString()
		{
			return this.SopInstanceUID;
		}
	}
}
