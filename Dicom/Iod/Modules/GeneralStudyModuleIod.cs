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
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// GeneralStudy Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.2.1 (Table C.7-3)</remarks>
	public class GeneralStudyModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralStudyModuleIod"/> class.
		/// </summary>	
		public GeneralStudyModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralStudyModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
		public GeneralStudyModuleIod(DicomAttributeCollection dicomAttributeCollection) : base(dicomAttributeCollection) {}

		/// <summary>
		/// Gets or sets the value of StudyInstanceUid in the underlying collection.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return base.DicomAttributeCollection[DicomTags.StudyInstanceUid].GetString(0, string.Empty); }
			set { base.DicomAttributeCollection[DicomTags.StudyInstanceUid].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of StudyDate and StudyTime in the underlying collection.
		/// </summary>
		public DateTime? StudyDateTime
		{
			get
			{
				string date = base.DicomAttributeCollection[DicomTags.StudyDate].GetString(0, string.Empty);
				string time = base.DicomAttributeCollection[DicomTags.StudyTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				DicomAttribute date = base.DicomAttributeCollection[DicomTags.StudyDate];
				DicomAttribute time = base.DicomAttributeCollection[DicomTags.StudyTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of ReferringPhysiciansName in the underlying collection.
		/// </summary>
		public string ReferringPhysiciansName
		{
			get { return base.DicomAttributeCollection[DicomTags.ReferringPhysiciansName].GetString(0, string.Empty); }
			set { base.DicomAttributeCollection[DicomTags.ReferringPhysiciansName].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ReferringPhysicianIdentificationSequence in the underlying collection.
		/// </summary>
		public PersonIdentificationMacro ReferringPhysicianIdentificationSequence
		{
			get
			{
				if (base.DicomAttributeCollection[DicomTags.ReferringPhysicianIdentificationSequence].Count == 0)
				{
					return null;
				}
				return new PersonIdentificationMacro(((DicomSequenceItem[]) base.DicomAttributeCollection[DicomTags.ReferringPhysicianIdentificationSequence].Values)[0]);
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeCollection[DicomTags.ReferringPhysicianIdentificationSequence] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.ReferringPhysicianIdentificationSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of StudyId in the underlying collection.
		/// </summary>
		public string StudyId
		{
			get { return base.DicomAttributeCollection[DicomTags.StudyId].GetString(0, string.Empty); }
			set { base.DicomAttributeCollection[DicomTags.StudyId].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of AccessionNumber in the underlying collection.
		/// </summary>
		public string AccessionNumber
		{
			get { return base.DicomAttributeCollection[DicomTags.AccessionNumber].GetString(0, string.Empty); }
			set { base.DicomAttributeCollection[DicomTags.AccessionNumber].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of StudyDescription in the underlying collection.
		/// </summary>
		public string StudyDescription
		{
			get { return base.DicomAttributeCollection[DicomTags.StudyDescription].GetString(0, string.Empty); }
			set { base.DicomAttributeCollection[DicomTags.StudyDescription].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of PhysiciansOfRecord in the underlying collection.
		/// </summary>
		public string PhysiciansOfRecord
		{
			get { return base.DicomAttributeCollection[DicomTags.PhysiciansOfRecord].ToString(); }
			set { base.DicomAttributeCollection[DicomTags.PhysiciansOfRecord].SetStringValue(value); }
		}

		/// <summary>
		/// Gets or sets the value of PhysiciansOfRecordIdentificationSequence in the underlying collection.
		/// </summary>
		public PersonIdentificationMacro PhysiciansOfRecordIdentificationSequence
		{
			get
			{
				if (base.DicomAttributeCollection[DicomTags.PhysiciansOfRecordIdentificationSequence].Count == 0)
				{
					return null;
				}
				return new PersonIdentificationMacro(((DicomSequenceItem[]) base.DicomAttributeCollection[DicomTags.PhysiciansOfRecordIdentificationSequence].Values)[0]);
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeCollection[DicomTags.PhysiciansOfRecordIdentificationSequence] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.PhysiciansOfRecordIdentificationSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of NameOfPhysiciansReadingStudy in the underlying collection.
		/// </summary>
		public string NameOfPhysiciansReadingStudy
		{
			get { return base.DicomAttributeCollection[DicomTags.NameOfPhysiciansReadingStudy].ToString(); }
			set { base.DicomAttributeCollection[DicomTags.NameOfPhysiciansReadingStudy].SetStringValue(value); }
		}

		/// <summary>
		/// Gets or sets the value of PhysiciansReadingStudyIdentificationSequence in the underlying collection.
		/// </summary>
		public PersonIdentificationMacro PhysiciansReadingStudyIdentificationSequence
		{
			get
			{
				if (base.DicomAttributeCollection[DicomTags.PhysiciansReadingStudyIdentificationSequence].Count == 0)
				{
					return null;
				}
				return new PersonIdentificationMacro(((DicomSequenceItem[]) base.DicomAttributeCollection[DicomTags.PhysiciansReadingStudyIdentificationSequence].Values)[0]);
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeCollection[DicomTags.PhysiciansReadingStudyIdentificationSequence] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.PhysiciansReadingStudyIdentificationSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of ReferencedStudySequence in the underlying collection.
		/// </summary>
		public ISopInstanceReferenceMacro ReferencedStudySequence
		{
			get
			{
				if (base.DicomAttributeCollection[DicomTags.ReferencedStudySequence].Count == 0)
				{
					return null;
				}
				return new SopInstanceReferenceMacro(((DicomSequenceItem[]) base.DicomAttributeCollection[DicomTags.ReferencedStudySequence].Values)[0]);
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeCollection[DicomTags.ReferencedStudySequence] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.ReferencedStudySequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of ProcedureCodeSequence in the underlying collection.
		/// </summary>
		public CodeSequenceMacro ProcedureCodeSequence
		{
			get
			{
				if (base.DicomAttributeCollection[DicomTags.ProcedureCodeSequence].Count == 0)
				{
					return null;
				}
				return new CodeSequenceMacro(((DicomSequenceItem[]) base.DicomAttributeCollection[DicomTags.ProcedureCodeSequence].Values)[0]);
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeCollection[DicomTags.ProcedureCodeSequence] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.ProcedureCodeSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}
	}
}