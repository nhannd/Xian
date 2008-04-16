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

#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{
	internal interface IMockImageSopSetters
	{
		string PatientId { set; }
		string StudyInstanceUid { set; }
		string SeriesInstanceUid { set; }
		string SopInstanceUid { set; }
		
		string StudyDate { set; }

		int InstanceNumber { set; }
		int SeriesNumber { set; }
	}

	internal class MockFrame : Frame
	{
		public MockFrame(MockImageSop parent, int frameNumber)
			: base(parent, frameNumber)
		{
		}

		public override byte[] GetNormalizedPixelData()
		{
			return new byte[]{0};

		}
		public override int Rows
		{
			get
			{
				return 512;
			}
		}

		public override int Columns
		{
			get
			{
				return 512;
			}
		}

		public override int BitsAllocated
		{
			get
			{
				return 16;
			}
		}
		
		public override int BitsStored
		{
			get
			{
				return 12;
			}
		}
		
		public override int HighBit
		{
			get
			{
				return 11;
			}
		}
		
		public override int PixelRepresentation
		{
			get
			{
				return 1;
			}
		}
		
		public override PhotometricInterpretation PhotometricInterpretation
		{
			get { return PhotometricInterpretation.Monochrome2; }	
		}
		public override double RescaleSlope
		{
			get
			{
				return 1.0;
			}
		}

		public override double RescaleIntercept
		{
			get
			{
				return 0;
			}
		}

		public override PixelAspectRatio PixelAspectRatio
		{
			get
			{
				return new PixelAspectRatio(1, 1);
			}
		}

		public override PixelSpacing PixelSpacing
		{
			get
			{
				return new PixelSpacing(1, 1);
			}
		}
	}

	// This mock IImageSop only implements the bare minimum of the overridden properties
	// in order to perform unit tests.
	internal class MockImageSop : ImageSop, IMockImageSopSetters
	{
		private string _patientID;
		private string _studyInstanceUID;
		private string _seriesInstanceUID;
		private string _sopInstanceUID;

		private int _seriesNumber;
		private int _instanceNumber;
		private string _studyDate;

		public MockImageSop()
		{
		}

		public MockImageSop(string patientId, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid)
		{
			_patientID = patientId;
			_studyInstanceUID = studyInstanceUid;
			_seriesInstanceUID = seriesInstanceUid;
			_sopInstanceUID = sopInstanceUid;
		}

		protected override Frame CreateFrame(int index)
		{
			return new MockFrame(this, 1);
		}

		protected override void ValidateInternal()
		{
		}

		public override DicomMessageBase NativeDicomObject
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

		public override string PatientId
		{
			get { return _patientID; }
		}

		public override string StudyInstanceUID
		{
			get { return _studyInstanceUID; }
		}

		public override string SeriesInstanceUID
		{
			get { return _seriesInstanceUID; }
		}

		public override string SopInstanceUID
		{
			get { return _sopInstanceUID; }
		}

		public override int InstanceNumber
		{
			get { return _instanceNumber; }
		}

		public override int SeriesNumber
		{
			get { return _seriesNumber; }
		}

		public override string StudyDate
		{
			get { return _studyDate; }
		}

		#region IMockImageSopSetters Members

		string IMockImageSopSetters.PatientId
		{
			set { _patientID = value; }
		}

		string IMockImageSopSetters.StudyInstanceUid
		{
			set { _studyInstanceUID = value; }
		}

		string IMockImageSopSetters.SeriesInstanceUid
		{
			set { _seriesInstanceUID = value; }
		}

		string IMockImageSopSetters.SopInstanceUid
		{
			set { _sopInstanceUID = value; }
		}

		int IMockImageSopSetters.InstanceNumber
		{
			set { _instanceNumber = value; }
		}

		int IMockImageSopSetters.SeriesNumber
		{
			set { _seriesNumber = value; }	
		}

		string IMockImageSopSetters.StudyDate
		{
			set { _studyDate = value; }
		}

		#endregion

		#region Not Implemented

		public override PersonName PatientsName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientsBirthDate
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientsSex
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StudyTime
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PersonName ReferringPhysiciansName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AccessionNumber
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StudyDescription
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PersonName[] NameOfPhysiciansReadingStudy
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string[] AdmittingDiagnosesDescription
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientsAge
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AdditionalPatientsHistory
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string Modality
		{
			get
			{
				return "MR";
			}
		}

		public override string SeriesDescription
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string Laterality
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SeriesDate
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SeriesTime
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PersonName[] PerformingPhysiciansName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ProtocolName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PersonName[] OperatorsName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string BodyPartExamined
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientPosition
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string Manufacturer
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string InstitutionName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string StationName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string InstitutionalDepartmentName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ManufacturersModelName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override void GetTag(uint tag, out ushort val, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(uint tag, out ushort val, uint position, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(uint tag, out int val, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(uint tag, out int val, uint position, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(uint tag, out double val, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(uint tag, out double val, uint position, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(uint tag, out string val, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(uint tag, out string val, uint pos, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetMultiValuedTagRaw(uint tag, out string valueArray, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTag(uint tag, out byte[] value, out bool tagExists)
		{
			throw new NotImplementedException();
		}

		public override string TransferSyntaxUID
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int NumberOfFrames
		{
			get
			{
				return 1;
			}
		}

		#endregion
	}
}
#endif