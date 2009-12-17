#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Validation;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM SOP Instance.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Note that there should no longer be any need to derive from this class; the <see cref="Sop"/>, <see cref="ImageSop"/>
	/// and <see cref="Frame"/> classes are now just simple Bridge classes (see Bridge Design Pattern)
	/// to <see cref="ISopDataSource"/> and <see cref="ISopFrameData"/>.  See the
	/// remarks for <see cref="ISopDataSource"/> for more information.
	/// </para>
	/// <para>Also, for more information on 'transient references' and the lifetime of <see cref="Sop"/>s,
	/// see <see cref="ISopReference"/>.
	/// </para>
	/// </remarks>
	
	public partial class Sop : IDisposable, ISopInstanceData
	{
		private volatile Series _parentSeries;
		private volatile ISopDataCacheItemReference _dataSourceReference;

		/// <summary>
		/// Creates a new instance of <see cref="Sop"/> from a local file.
		/// </summary>
		/// <param name="filename">The path to a local DICOM Part 10 file.</param>
		public Sop(string filename)
		{
			ISopDataSource dataSource = new LocalSopDataSource(filename);
			try
			{
				Initialize(dataSource);
			}
			catch (Exception)
			{
				dataSource.Dispose();
				throw;
			}
		}

		/// <summary>
		/// Creates a new instance of <see cref="Sop"/>.
		/// </summary>
		public Sop(ISopDataSource dataSource)
		{
			Initialize(dataSource);
		}

		private void Initialize(ISopDataSource dataSource)
		{
			//We want to explicitly enforce that image data sources are wrapped in ImageSops.
			IsImage = this is ImageSop;

			if (dataSource.IsImage != IsImage)
				throw new ArgumentException("Data source/Sop type mismatch.", "dataSource");

			//silently use shared/cached data source.
			_dataSourceReference = SopDataCache.Add(dataSource);
		}

		/// <summary>
		/// Gets the <see cref="Sop"/>'s data source.
		/// </summary>
		public ISopDataSource DataSource
		{
			get { return _dataSourceReference.RealDataSource; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the SOP instance is 'stored',
		/// for example in the local data store or a remote PACS server.
		/// </summary>
		public bool IsStored
		{
			get { return DataSource.IsStored; }	
		}

		/// <summary>
		/// Gets a value indicating whether or not the SOP instance is an image class.
		/// </summary>
		public bool IsImage { get; private set; }

		/// <summary>
		/// Gets the parent <see cref="Series"/>.
		/// </summary>
		public virtual Series ParentSeries
		{
			get { return _parentSeries; }
			internal set { _parentSeries = value; }
		}

		/// <summary>
		/// Gets an <see cref="IImageIdentifier"/> for this <see cref="Sop"/>.
		/// </summary>
		/// <remarks>An <see cref="IImageIdentifier"/> can be used in situations where you only
		/// need some data about the <see cref="Sop"/>, but not the <see cref="Sop"/> itself.  It can be problematic
		/// to hold references to <see cref="Sop"/> objects outside the context of an <see cref="IImageViewer"/>
		/// <b>without creating a <see cref="ISopReference">transient reference</see></b>
		/// because they are no longer valid when the viewer is closed; in these situations, it may be appropriate to
		/// use an identifier.
		/// </remarks>
		public IImageIdentifier GetIdentifier()
		{
			StudyItem studyIdentifier = new StudyItem(StudyInstanceUid, DataSource.Server, DataSource.StudyLoaderName);
			studyIdentifier.InstanceAvailability = "ONLINE";
			return new ImageIdentifier(this, studyIdentifier);
		}

		internal IList<VoiDataLut> GetVoiDataLuts()
		{
			return _dataSourceReference.VoiDataLuts;
		}

		#region Meta info

		/// <summary>
		/// Gets the Transfer Syntax UID.
		/// </summary>
		public virtual string TransferSyntaxUid
		{
			get { return DataSource.TransferSyntaxUid; }
		}

		#endregion

		#region SOP Common Module

		/// <summary>
		/// Gets the SOP Instance UID.
		/// </summary>
		public virtual string SopInstanceUid
		{
			get { return DataSource.SopInstanceUid; }
		}

		/// <summary>
		/// Gets the SOP Class UID.
		/// </summary>
		public virtual string SopClassUid
		{
			get { return DataSource.SopClassUid; }
			
		}

		/// <summary>
		/// Gets the specific character set.
		/// </summary>
		public virtual string[] SpecificCharacterSet
		{
			get
			{
				string specificCharacterSet;
				specificCharacterSet = this[DicomTags.SpecificCharacterSet].ToString();

				if (!string.IsNullOrEmpty(specificCharacterSet))
				{
					string[] values;
					values = DicomStringHelper.GetStringArray(specificCharacterSet);
					return values;
				}
				else
				{
					return new string[0];
				}
			}
		}

		/// <summary>
		/// Gets the instance number.
		/// </summary>
		public virtual int InstanceNumber
		{
			get { return DataSource.InstanceNumber; }
		}
		#endregion

		#region Patient Module

		/// <summary>
		/// Gets the patient's name.
		/// </summary>
		public virtual PersonName PatientsName
		{
			get
			{
				string patientsName;
				patientsName = this[DicomTags.PatientsName].GetString(0, null);
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
				string patientId;
				patientId = this[DicomTags.PatientId].GetString(0, null);
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
				string patientsBirthDate;
				patientsBirthDate = this[DicomTags.PatientsBirthDate].GetString(0, null);
				return patientsBirthDate ?? "";
			}
		}

		/// <summary>
		/// Gets the patient's birth time.
		/// </summary>
		public virtual string PatientsBirthTime
		{
			get
			{
				string patientsBirthTime;
				patientsBirthTime = this[DicomTags.PatientsBirthTime].GetString(0, null);
				return patientsBirthTime ?? "";
			}
		}

		/// <summary>
		/// Gets the patient's sex.
		/// </summary>
		public virtual string PatientsSex
		{
			get
			{
				string patientsSex;
				patientsSex = this[DicomTags.PatientsSex].GetString(0, null);
				return patientsSex ?? "";
			}
		}

		#endregion	

		#region General Study Module

		/// <summary>
		/// Gets the Study Instance UID.
		/// </summary>
		public virtual string StudyInstanceUid
		{
			get { return DataSource.StudyInstanceUid; }
		}

		/// <summary>
		/// Gets the study date.
		/// </summary>
		public virtual string StudyDate
		{
			get
			{
				string studyDate;
				studyDate = this[DicomTags.StudyDate].GetString(0, null);
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
				string studyTime;
				studyTime = this[DicomTags.StudyTime].GetString(0, null);
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
				string referringPhysiciansName;
				referringPhysiciansName = this[DicomTags.ReferringPhysiciansName].GetString(0, null);
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
				string accessionNumber;
				accessionNumber = this[DicomTags.AccessionNumber].GetString(0, null);
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
				string studyDescription;
				studyDescription = this[DicomTags.StudyDescription].GetString(0, null);
				return studyDescription ?? "";
			}
		}

		/// <summary>
		/// Gets the study ID.
		/// </summary>
		public virtual string StudyId
		{
			get
			{
				string studyId;
				studyId = this[DicomTags.StudyId].GetString(0, null);
				return studyId ?? "";
			}
		}

		/// <summary>
		/// Gets the names of physicians reading the study.
		/// </summary>
		public virtual PersonName[] NameOfPhysiciansReadingStudy
		{
			get
			{
				string nameOfPhysiciansReadingStudy;
				nameOfPhysiciansReadingStudy = this[DicomTags.NameOfPhysiciansReadingStudy].ToString();
				return DicomStringHelper.GetPersonNameArray(nameOfPhysiciansReadingStudy);
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
				string admittingDiagnosesDescription;
				admittingDiagnosesDescription = this[DicomTags.AdmittingDiagnosesDescription].ToString();
				return DicomStringHelper.GetStringArray(admittingDiagnosesDescription);
			}
		}

		/// <summary>
		/// Gets the patient's age.
		/// </summary>
		public virtual string PatientsAge
		{
			get
			{
				string patientsAge;
				patientsAge = this[DicomTags.PatientsAge].GetString(0, null);
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
				string additionalPatientsHistory;
				additionalPatientsHistory = this[DicomTags.AdditionalPatientHistory].GetString(0, null);
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
				string manufacturer; 
				manufacturer = this[DicomTags.Manufacturer].GetString(0, null);
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
				string institutionName;
				institutionName = this[DicomTags.InstitutionName].GetString(0, null);
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
				string stationName;
				stationName = this[DicomTags.StationName].GetString(0, null);
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
				string institutionalDepartmentName;
				institutionalDepartmentName = this[DicomTags.InstitutionalDepartmentName].GetString(0, null);
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
				string manufacturersModelName;
				manufacturersModelName = this[DicomTags.ManufacturersModelName].GetString(0, null);
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
				string modality;
				modality = this[DicomTags.Modality].GetString(0, null);
				return modality ?? "";
			}
		}

		/// <summary>
		/// Gets the series instance UID.
		/// </summary>
		public virtual string SeriesInstanceUid
		{
			get { return DataSource.SeriesInstanceUid; }
		}

		/// <summary>
		/// Gets the series number.
		/// </summary>
		public virtual int SeriesNumber
		{
			get
			{
				int seriesNumber;
				seriesNumber = this[DicomTags.SeriesNumber].GetInt32(0, 0);
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
				string laterality;
				laterality = this[DicomTags.Laterality].GetString(0, null);
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
				string seriesDate;
				seriesDate = this[DicomTags.SeriesDate].GetString(0, null);
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
				string seriesTime;
				seriesTime = this[DicomTags.SeriesTime].GetString(0, null);
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
				string performingPhysiciansNames;
				performingPhysiciansNames = this[DicomTags.PerformingPhysiciansName].ToString();
				return DicomStringHelper.GetPersonNameArray(performingPhysiciansNames);
			}
		}

		/// <summary>
		/// Gets the protocol name.
		/// </summary>
		public virtual string ProtocolName
		{
			get
			{
				string protocolName;
				protocolName = this[DicomTags.ProtocolName].GetString(0, null);
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
				string seriesDescription;
				seriesDescription = this[DicomTags.SeriesDescription].GetString(0, null);
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
				string operatorsNames;
				operatorsNames = this[DicomTags.OperatorsName].ToString();
				return DicomStringHelper.GetPersonNameArray(operatorsNames);
			}
		}

		/// <summary>
		/// Gets the body part examined.
		/// </summary>
		public virtual string BodyPartExamined
		{
			get
			{
				string bodyPartExamined;
				bodyPartExamined = this[DicomTags.BodyPartExamined].GetString(0, null);
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
				string patientPosition;
				patientPosition = this[DicomTags.PatientPosition].GetString(0, null);
				return patientPosition ?? "";
			}
		}

		#endregion

		#region Dicom Tag Retrieval Methods

		private delegate void GetTagDelegate<T>(DicomAttribute attribute, uint position, out T value);

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
		/// <param name="value"></param>
		/// <param name="tagExists"></param>
		/// <seealso cref="this[DicomTag]"/>
		/// <seealso cref="this[uint]"/>
		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public virtual void GetTag(uint tag, out ushort value, out bool tagExists)
		{
			GetTag(tag, out value, 0, out tagExists);
		}

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
		/// <param name="value"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		/// <seealso cref="this[DicomTag]"/>
		/// <seealso cref="this[uint]"/>
		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public virtual void GetTag(uint tag, out ushort value, uint position, out bool tagExists)
		{
			GetTag<ushort>(tag, out value, position, out tagExists, GetUint16FromAttribute);
		}

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
		/// <param name="value"></param>
		/// <param name="tagExists"></param>
		/// <seealso cref="this[DicomTag]"/>
		/// <seealso cref="this[uint]"/>
		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public virtual void GetTag(uint tag, out int value, out bool tagExists)
		{
			GetTag(tag, out value, 0, out tagExists);
		}

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
		/// <param name="value"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		/// <seealso cref="this[DicomTag]"/>
		/// <seealso cref="this[uint]"/>
		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public virtual void GetTag(uint tag, out int value, uint position, out bool tagExists)
		{
			GetTag<int>(tag, out value, position, out tagExists, GetInt32FromAttribute);
		}

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
		/// <param name="value"></param>
		/// <param name="tagExists"></param>
		/// <seealso cref="this[DicomTag]"/>
		/// <seealso cref="this[uint]"/>
		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public virtual void GetTag(uint tag, out double value, out bool tagExists)
		{
			GetTag(tag, out value, 0, out tagExists);
		}

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
		/// <param name="value"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		/// <seealso cref="this[DicomTag]"/>
		/// <seealso cref="this[uint]"/>
		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public virtual void GetTag(uint tag, out double value, uint position, out bool tagExists)
		{
			GetTag<double>(tag, out value, position, out tagExists, GetFloat64FromAttribute);
		}

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
		/// <param name="value"></param>
		/// <param name="tagExists"></param>
		/// <seealso cref="this[DicomTag]"/>
		/// <seealso cref="this[uint]"/>
		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public virtual void GetTag(uint tag, out string value, out bool tagExists)
		{
			GetTag(tag, out value, 0, out tagExists);
		}

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
		/// <param name="value"></param>
		/// <param name="position"></param>
		/// <param name="tagExists"></param>
		/// <seealso cref="this[DicomTag]"/>
		/// <seealso cref="this[uint]"/>
		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public virtual void GetTag(uint tag, out string value, uint position, out bool tagExists)
		{
			GetTag<string>(tag, out value, position, out tagExists, GetStringFromAttribute);
			value = value ?? "";
		}

		/// <summary>
		/// Gets a DICOM OB or OW tag (byte[]), not including encapsulated pixel data.
		/// </summary>
		/// <remarks>
		/// GetTag methods should make no assumptions about what to return in the <paramref name="value"/> parameter
		/// when a tag does not exist.  It should simply return the default value for <paramref name="value"/>'s Type,
		/// which is either null, 0 or "" depending on whether it is a reference or value Type.  Similarly, no data validation
		/// should be done in these methods either.  It is expected that the unaltered tag value will be returned.
		/// </remarks>
		/// <param name="tag"></param>
		/// <param name="value"></param>
		/// <param name="tagExists"></param>
		/// <seealso cref="this[DicomTag]"/>
		/// <seealso cref="this[uint]"/>
		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public virtual void GetTag(uint tag, out byte[] value, out bool tagExists)
		{
			GetTag<byte[]>(tag, out value, 0, out tagExists, GetAttributeValueOBOW);
		}

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
		/// <param name="value"></param>
		/// <param name="tagExists"></param>
		/// <seealso cref="this[DicomTag]"/>
		/// <seealso cref="this[uint]"/>
		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public virtual void GetMultiValuedTagRaw(uint tag, out string value, out bool tagExists)
		{
			GetTag<string>(tag, out value, 0, out tagExists, GetStringArrayFromAttribute);
			value = value ?? "";
		}

		/// <summary>
		/// Gets a specific DICOM tag in the underlying native object.
		/// </summary>
		/// <remarks>
		/// <see cref="DicomAttribute"/>s returned from this indexer are considered
		/// read-only and should not be modified in any way.
		/// </remarks>
		/// <param name="tag">The DICOM tag to retrieve.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the specified DICOM tag is not within the valid range for either the meta info or the dataset.</exception>
		public DicomAttribute this[DicomTag tag]
		{
			get { return this[tag.TagValue]; }
		}

		/// <summary>
		/// Gets a specific DICOM tag in the underlying native object.
		/// </summary>
		/// <remarks>
		/// <see cref="DicomAttribute"/>s returned from this indexer are considered
		/// read-only and should not be modified in any way.
		/// </remarks>
		/// <param name="tag">The DICOM tag to retrieve.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the specified DICOM tag is not within the valid range for either the meta info or the dataset.</exception>
		public virtual DicomAttribute this[uint tag]
		{
			get { return this.DataSource[tag]; }
		}

		private static void GetUint16FromAttribute(DicomAttribute attribute, uint position, out ushort value)
		{
			attribute.TryGetUInt16((int)position, out value);
		}

		private static void GetInt32FromAttribute(DicomAttribute attribute, uint position, out int value)
		{
			attribute.TryGetInt32((int)position, out value);
		}

		private static void GetFloat64FromAttribute(DicomAttribute attribute, uint position, out double value)
		{
			attribute.TryGetFloat64((int)position, out value);
		}

		private static void GetStringFromAttribute(DicomAttribute attribute, uint position, out string value)
		{
			attribute.TryGetString((int)position, out value);
		}

		private static void GetStringArrayFromAttribute(DicomAttribute attribute, uint position, out string value)
		{
			value = attribute.ToString();
		}

		private static void GetAttributeValueOBOW(DicomAttribute attribute, uint position, out byte[] value)
		{
			if (attribute is DicomAttributeOW || attribute is DicomAttributeOB)
				value = (byte[])attribute.Values;
			else
				value = null;
		}

		private void GetTag<T>(uint tag, out T value, uint position, out bool tagExists, GetTagDelegate<T> getter)
		{
			value = default(T);
			tagExists = false;

			DicomAttribute dicomAttribute = this[tag];
			if (dicomAttribute != null)
			{
				tagExists = !dicomAttribute.IsEmpty && dicomAttribute.Count > position;
				if (tagExists)
					getter(dicomAttribute, position, out value);
			}
		}

		#endregion

		#region Static Helpers

		/// <summary>
		/// Creates either a <see cref="Sop"/> or <see cref="ImageSop"/> based
		/// on the <see cref="SopClass"/> of the given <see cref="ISopDataSource"/>.
		/// </summary>
		public static Sop Create(ISopDataSource dataSource)
		{
			if (dataSource.IsImage)
				return new ImageSop(dataSource);
			else
				return new Sop(dataSource);
		}

		/// <summary>
		/// Creates either a <see cref="Sop"/> or <see cref="ImageSop"/> based
		/// on the <see cref="SopClass"/> of the SOP instance specified by <paramref name="filename"/>.
		/// </summary>
		public static Sop Create(string filename)
		{
			return Create(new LocalSopDataSource(filename));
		}

		internal static bool IsImageSop(string sopClassUid)
		{
			return IsImageSop(SopClass.GetSopClass(sopClassUid));
		}

		internal static bool IsImageSop(SopClass sopClass)
		{
			return _imageSopClasses.Contains(sopClass);
		}

		private static readonly ReadOnlyCollection<SopClass> _imageSopClasses = new List<SopClass>(GetImageSopClasses()).AsReadOnly();

		private static IEnumerable<SopClass> GetImageSopClasses()
		{
			yield return SopClass.ComputedRadiographyImageStorage;
			yield return SopClass.CtImageStorage;

			yield return SopClass.DigitalIntraOralXRayImageStorageForPresentation;
			yield return SopClass.DigitalIntraOralXRayImageStorageForProcessing;

			yield return SopClass.DigitalMammographyXRayImageStorageForPresentation;
			yield return SopClass.DigitalMammographyXRayImageStorageForProcessing;

			yield return SopClass.DigitalXRayImageStorageForPresentation;
			yield return SopClass.DigitalXRayImageStorageForProcessing;

			yield return SopClass.EnhancedCtImageStorage;
			yield return SopClass.EnhancedMrImageStorage;

			yield return SopClass.EnhancedXaImageStorage;

			yield return SopClass.EnhancedXrfImageStorage;

			yield return SopClass.MrImageStorage;

			yield return SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorage;
			yield return SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage;
			yield return SopClass.MultiFrameSingleBitSecondaryCaptureImageStorage;
			yield return SopClass.MultiFrameTrueColorSecondaryCaptureImageStorage;

			yield return SopClass.NuclearMedicineImageStorageRetired;
			yield return SopClass.NuclearMedicineImageStorage;

			yield return SopClass.OphthalmicPhotography16BitImageStorage;
			yield return SopClass.OphthalmicPhotography8BitImageStorage;
			yield return SopClass.OphthalmicTomographyImageStorage;

			yield return SopClass.PositronEmissionTomographyImageStorage;

			yield return SopClass.RtImageStorage;

			yield return SopClass.SecondaryCaptureImageStorage;

			yield return SopClass.UltrasoundImageStorage;
			yield return SopClass.UltrasoundImageStorageRetired;
			yield return SopClass.UltrasoundMultiFrameImageStorage;
			yield return SopClass.UltrasoundMultiFrameImageStorageRetired;

			yield return SopClass.VideoEndoscopicImageStorage;
			yield return SopClass.VideoMicroscopicImageStorage;
			yield return SopClass.VideoPhotographicImageStorage;

			yield return SopClass.VlEndoscopicImageStorage;
			yield return SopClass.VlMicroscopicImageStorage;
			yield return SopClass.VlPhotographicImageStorage;
			yield return SopClass.VlSlideCoordinatesMicroscopicImageStorage;

			yield return SopClass.XRay3dAngiographicImageStorage;
			yield return SopClass.XRay3dCraniofacialImageStorage;

			yield return SopClass.XRayAngiographicBiPlaneImageStorageRetired;
			yield return SopClass.XRayAngiographicImageStorage;

			yield return SopClass.XRayRadiofluoroscopicImageStorage;
		}

		#endregion

		#region Validation

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
			catch(SopValidationException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new SopValidationException("Sop validation failed.", e);
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
			if (ValidationSettings.Default.DisableSopValidation)
				return;

			DicomValidator.ValidateSOPInstanceUID(this.SopInstanceUid);
			DicomValidator.ValidateSeriesInstanceUID(this.SeriesInstanceUid);
			DicomValidator.ValidateStudyInstanceUID(this.StudyInstanceUid);

			ValidatePatientId();
		}

		private void ValidatePatientId()
		{
			//Patient ID is a Type 2 tag, so this is our own restriction, not a Dicom Restriction.
			if (String.IsNullOrEmpty(this.PatientId) || this.PatientId.TrimEnd(' ').Length == 0)
				throw new SopValidationException(SR.ExceptionInvalidPatientID);
		}

		#endregion

		/// <summary>
		/// Disposes all resources being used by this <see cref="Sop"/>.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _dataSourceReference != null)
			{
				_dataSourceReference.Dispose();
				_dataSourceReference = null;
			}
		}

		/// <summary>
		/// Returns the SOP instance UID in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0} | {1}", this.InstanceNumber, this.SopInstanceUid);
		}
	}
}
