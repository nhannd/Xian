#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Validation;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	//TODO: rewrite the documentation.

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
	/// Follow these guidelines when implementing <see cref="Sop"/>-derived classes:
	/// <list>
	/// <item>
	/// <description>
	/// 1) For (new) properties that represent Type 1 tags, override the <see cref="ValidateInternal"/> method (which is called by <see cref="Validate"/>),
	///    calling the base class' <see cref="ValidateInternal"/> first, then doing further validation on those properties.  Validation on other tags can be done
	///    at your discretion; for example, the <see cref="Sop"/> class validates that the <see cref="PatientId"/> property is non-empty, even though it is Type 2.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// 2) Override and implement all of the GetTag methods, but do not throw an exception.  See #4 for an explanation.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// 3) Override any properties that you wish to change the behaviour of how the values are returned.  All properties in <see cref="Sop"/> call
	///    one of the <b>GetTag</b> methods to retrieve the tag value.  You may wish to retrieve certain tags differently.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// 4) <b>GetTag</b> methods should make no assumptions about return values.  If a return value cannot be determined, the default value for the
	///    return type should be returned.  These are: "" for strings, 0 for any numeric type.  See the <b>GetTag</b> methods for more details.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// 5) As mentioned above, when returning values from properties for which no data is available, return the default value 
	///    for the given type according to the following guidelines:
	///    - "" for string types
	///    - 0 for numeric types
	///    - null for reference types that are not strings
	///    - a valid default value when deemed appropriate
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// 6) If a particular property's value is vital to functionality and has no reasonable default, it should be included in the <see cref="ValidateInternal"/>
	///    override and considered a fault if the value is invalid.  No attempt should be made to correct data in the properties when there is no 'reasonable default'.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// 7) If a reasonable default is returned from a property, the property's documentation should reflect that.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// 8) If the existence or 'true value' of a tag is important to your implementation, use one of the <b>GetTag</b> methods, rather than the
	///    existing property (or adding a property, for that matter).
	/// </description>
	/// </item>
	/// </list>
	/// </para>
	/// </remarks>
	public partial class Sop : IDisposable
	{
		private volatile Series _parentSeries;
		private volatile ISopDataCacheItemReference _dataSourceReference;

		/// <summary>
		/// Creates a new instance of <see cref="Sop"/>.
		/// </summary>
		public Sop(ISopDataSource dataSource)
		{
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
		/// Gets the parent <see cref="Series"/>.
		/// </summary>
		public virtual Series ParentSeries
		{
			get { return _parentSeries; }
			internal set { _parentSeries = value; }
		}

		#region Meta info

		/// <summary>
		/// Gets the Transfer Syntax UID.
		/// </summary>
		public virtual string TransferSyntaxUID
		{
			get { return DataSource.TransferSyntaxUid; }
		}

		#endregion

		#region SOP Common Module

		/// <summary>
		/// Gets the SOP Instance UID.
		/// </summary>
		public virtual string SopInstanceUID
		{
			get { return DataSource.SopInstanceUid; }
		}

		/// <summary>
		/// Gets the SOP Class UID.
		/// </summary>
		public virtual string SopClassUID
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
		public virtual string StudyInstanceUID
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
		/// Gets the Series Instance UID.
		/// </summary>
		public virtual string SeriesInstanceUID
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

		public static Sop Create(ISopDataSource dataSource)
		{
			if (SopDataHelper.IsImageSop(dataSource.SopClassUid))
				return new ImageSop(dataSource);
			else
				return new Sop(dataSource);
		}

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
			return this.SopInstanceUID;
		}
	}
}
