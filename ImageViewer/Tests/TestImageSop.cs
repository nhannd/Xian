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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tests
{
	internal class TestImageSop : ImageSop
	{
		private string _patientID;
		private string _studyInstanceUID;
		private string _seriesInstanceUID;
		private string _sopInstanceUID;

		public TestImageSop(
			string patientID, 
			string studyInstanceUID, 
			string seriesInstanceUID, 
			string sopInstanceUID)
		{
			_patientID = patientID;
			_studyInstanceUID = studyInstanceUID;
			_seriesInstanceUID = seriesInstanceUID;
			_sopInstanceUID = sopInstanceUID;
		}

        public override DicomMessageBase NativeDicomObject
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override PersonName PatientsName
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string PatientId
		{
			get
			{
				return _patientID;
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

		public override string StudyInstanceUID
		{
			get
			{
				return _studyInstanceUID;
			}
		}

		public override string StudyDate
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
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string SeriesInstanceUID
		{
			get
			{
				return _seriesInstanceUID;
			}
		}

		public override int SeriesNumber
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
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

		public override int InstanceNumber
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PatientOrientation PatientOrientation
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ImageType
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int AcquisitionNumber
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AcquisitionDate
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AcquisitionTime
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string AcquisitionDateTime
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int ImagesInAcquisition
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string ImageComments
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string LossyImageCompression
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double[] LossyImageCompressionRatio
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PixelSpacing PixelSpacing
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override ImageOrientationPatient ImageOrientationPatient
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override ImagePositionPatient ImagePositionPatient
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double SliceThickness
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double SliceLocation
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override PixelAspectRatio PixelAspectRatio
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int SamplesPerPixel
		{
			get
			{
				return 1;
			}
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				return PhotometricInterpretation.Monochrome1;
			}
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
				return 16;
			}
		}

		public override int HighBit
		{
			get
			{
				return 15;
			}
		}

		public override int PixelRepresentation
		{
			get
			{
				return 0;
			}
		}

		public override int PlanarConfiguration
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double RescaleIntercept
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override double RescaleSlope
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string RescaleType
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override Window[] WindowCenterAndWidth
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string[] WindowCenterAndWidthExplanation
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override byte[] GetNormalizedPixelData()
		{
			throw new Exception("The method or operation is not implemented.");
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

		public override void GetTag(uint tag, out string val, uint position, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetTagAsDicomStringArray(uint tag, out string val, out bool tagExists)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override string SopInstanceUID
		{
			get
			{
				return _sopInstanceUID;
			}
		}

		public override string TransferSyntaxUID
		{
			get
			{
				return "1.2.840.10008.1.2";
			}
		}
	}
}
