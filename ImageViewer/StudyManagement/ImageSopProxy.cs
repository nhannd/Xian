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

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal class ImageSopProxy : ImageSop
	{
		private ImageSop _realImageSop;

		internal ImageSopProxy(ImageSop realImageSop) : base(null)
		{
			Platform.CheckForNullReference(realImageSop, "realImageSop");
			_realImageSop = realImageSop;
		}

		//for unit testing
		internal ImageSop RealImageSop
		{
			get { return _realImageSop; }
		}

		public override Series ParentSeries
		{
			get
			{
				return _realImageSop.ParentSeries;
			}
			internal set
			{
				_realImageSop.ParentSeries = value;
			}
		}

		public override FrameCollection Frames
		{
			get
			{
				return _realImageSop.Frames;
			}
		}

		public override DicomMessageBase NativeDicomObject
		{
			get 
			{ 
				return _realImageSop.NativeDicomObject; 
			}
		}

		public override string TransferSyntaxUID
		{
			get
			{
				return _realImageSop.TransferSyntaxUID;
			}
		}

		public override string SopInstanceUID
		{
			get
			{
				return _realImageSop.SopInstanceUID;
			}
		}

		public override string SopClassUID
		{
			get
			{
				return _realImageSop.SopClassUID;
			}
		}

		public override string[] SpecificCharacterSet
		{
			get
			{
				return _realImageSop.SpecificCharacterSet;
			}
		}

		public override PersonName PatientsName
		{
			get
			{
				return _realImageSop.PatientsName;
			}
		}

		public override string PatientId
		{
			get
			{
				return _realImageSop.PatientId;
			}
		}

		public override string PatientsBirthDate
		{
			get
			{
				return _realImageSop.PatientsBirthDate;
			}
		}

		public override string PatientsSex
		{
			get
			{
				return _realImageSop.PatientsSex;
			}
		}

		public override string StudyInstanceUID
		{
			get
			{
				return _realImageSop.StudyInstanceUID;
			}
		}

		public override string StudyDate
		{
			get
			{
				return _realImageSop.StudyDate;
			}
		}

		public override string StudyTime
		{
			get
			{
				return _realImageSop.StudyTime;
			}
		}

		public override PersonName ReferringPhysiciansName
		{
			get
			{
				return _realImageSop.ReferringPhysiciansName;
			}
		}

		public override string AccessionNumber
		{
			get
			{
				return _realImageSop.AccessionNumber;
			}
		}

		public override string StudyDescription
		{
			get
			{
				return _realImageSop.StudyDescription;
			}
		}

		public override PersonName[] NameOfPhysiciansReadingStudy
		{
			get
			{
				return _realImageSop.NameOfPhysiciansReadingStudy;
			}
		}

		public override string[] AdmittingDiagnosesDescription
		{
			get
			{
				return _realImageSop.AdmittingDiagnosesDescription;
			}
		}

		public override string PatientsAge
		{
			get
			{
				return _realImageSop.PatientsAge;
			}
		}

		public override string AdditionalPatientsHistory
		{
			get
			{
				return _realImageSop.AdditionalPatientsHistory;
			}
		}

		public override string Modality
		{
			get
			{
				return _realImageSop.Modality;
			}
		}

		public override string SeriesInstanceUID
		{
			get
			{
				return _realImageSop.SeriesInstanceUID;
			}
		}

		public override int SeriesNumber
		{
			get
			{
				return _realImageSop.SeriesNumber;
			}
		}

		public override string Laterality
		{
			get
			{
				return _realImageSop.Laterality;
			}
		}

		public override string SeriesDate
		{
			get
			{
				return _realImageSop.SeriesDate;
			}
		}

		public override string SeriesTime
		{
			get
			{
				return _realImageSop.SeriesTime;
			}
		}

		public override PersonName[] PerformingPhysiciansName
		{
			get
			{
				return _realImageSop.PerformingPhysiciansName;
			}
		}

		public override string ProtocolName
		{
			get
			{
				return _realImageSop.ProtocolName;
			}
		}

		public override string SeriesDescription
		{
			get
			{
				return _realImageSop.SeriesDescription;
			}
		}

		public override PersonName[] OperatorsName
		{
			get
			{
				return _realImageSop.OperatorsName;
			}
		}

		public override string BodyPartExamined
		{
			get
			{
				return _realImageSop.BodyPartExamined;
			}
		}

		public override string PatientPosition
		{
			get
			{
				return _realImageSop.PatientPosition;
			}
		}

		public override string Manufacturer
		{
			get
			{
				return _realImageSop.Manufacturer;
			}
		}

		public override string InstitutionName
		{
			get
			{
				return _realImageSop.InstitutionName;
			}
		}

		public override string StationName
		{
			get
			{
				return _realImageSop.StationName;
			}
		}

		public override string InstitutionalDepartmentName
		{
			get
			{
				return _realImageSop.InstitutionalDepartmentName;
			}
		}

		public override string ManufacturersModelName
		{
			get
			{
				return _realImageSop.ManufacturersModelName;
			}
		}

		public override int InstanceNumber
		{
			get
			{
				return _realImageSop.InstanceNumber;
			}
		}

		public override int NumberOfFrames
		{
			get
			{
				return _realImageSop.NumberOfFrames;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override PatientOrientation PatientOrientation
		{
			get
			{
				return _realImageSop.PatientOrientation;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override string ImageType
		{
			get
			{
				return _realImageSop.ImageType;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int AcquisitionNumber
		{
			get
			{
				return _realImageSop.AcquisitionNumber;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override string AcquisitionDate
		{
			get
			{
				return _realImageSop.AcquisitionDate;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override string AcquisitionTime
		{
			get
			{
				return _realImageSop.AcquisitionTime;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override string AcquisitionDateTime
		{
			get
			{
				return _realImageSop.AcquisitionDateTime;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int ImagesInAcquisition
		{
			get
			{
				return _realImageSop.ImagesInAcquisition;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override string ImageComments
		{
			get
			{
				return _realImageSop.ImageComments;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override string LossyImageCompression
		{
			get
			{
				return _realImageSop.LossyImageCompression;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override double[] LossyImageCompressionRatio
		{
			get
			{
				return _realImageSop.LossyImageCompressionRatio;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override PixelSpacing PixelSpacing
		{
			get
			{
				return _realImageSop.PixelSpacing;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override ImageOrientationPatient ImageOrientationPatient
		{
			get
			{
				return _realImageSop.ImageOrientationPatient;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override ImagePositionPatient ImagePositionPatient
		{
			get
			{
				return _realImageSop.ImagePositionPatient;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override double SliceThickness
		{
			get
			{
				return _realImageSop.SliceThickness;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override double SliceLocation
		{
			get
			{
				return _realImageSop.SliceLocation;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override PixelAspectRatio PixelAspectRatio
		{
			get
			{
				return _realImageSop.PixelAspectRatio;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int SamplesPerPixel
		{
			get
			{
				return _realImageSop.SamplesPerPixel;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				return _realImageSop.PhotometricInterpretation;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int Rows
		{
			get
			{
				return _realImageSop.Rows;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int Columns
		{
			get
			{
				return _realImageSop.Columns;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int BitsAllocated
		{
			get
			{
				return _realImageSop.BitsAllocated;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int BitsStored
		{
			get
			{
				return _realImageSop.BitsStored;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int HighBit
		{
			get
			{
				return _realImageSop.HighBit;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int PixelRepresentation
		{
			get
			{
				return _realImageSop.PixelRepresentation;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int PlanarConfiguration
		{
			get
			{
				return _realImageSop.PlanarConfiguration;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override double RescaleIntercept
		{
			get
			{
				return _realImageSop.RescaleIntercept;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override double RescaleSlope
		{
			get
			{
				return _realImageSop.RescaleSlope;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override string RescaleType
		{
			get
			{
				return _realImageSop.RescaleType;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override Window[] WindowCenterAndWidth
		{
			get
			{
				return _realImageSop.WindowCenterAndWidth;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override string[] WindowCenterAndWidthExplanation
		{
			get
			{
				return _realImageSop.WindowCenterAndWidthExplanation;
			}
		}

		protected override Frame CreateFrame(int index)
		{
			throw new InvalidOperationException("Cannot create frame with a proxy object.");
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override byte[] GetNormalizedPixelData()
		{
			return _realImageSop.GetNormalizedPixelData();
		}

		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public override void GetTag(uint tag, out ushort val, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, out tagExists);
		}

		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public override void GetTag(uint tag, out ushort val, uint position, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, position, out tagExists);
		}

		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public override void GetTag(uint tag, out int val, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, out tagExists);
		}

		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public override void GetTag(uint tag, out int val, uint position, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, position, out tagExists);
		}

		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public override void GetTag(uint tag, out double val, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, out tagExists);
		}

		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public override void GetTag(uint tag, out double val, uint position, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, position, out tagExists);
		}

		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public override void GetTag(uint tag, out string val, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, out tagExists);
		}

		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public override void GetTag(uint tag, out string val, uint position, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out val, position, out tagExists);
		}

		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public override void GetMultiValuedTagRaw(uint tag, out string val, out bool tagExists)
		{
			_realImageSop.GetMultiValuedTagRaw(tag, out val, out tagExists);
		}

		[Obsolete("This method is now obsolete - use the indexers instead.")]
		public override void GetTag(uint tag, out byte[] value, out bool tagExists)
		{
			_realImageSop.GetTag(tag, out value, out tagExists);
		}

		internal override void IncrementReferenceCount()
		{
			_realImageSop.IncrementReferenceCount();
		}

		internal override void DecrementReferenceCount()
		{
			_realImageSop.DecrementReferenceCount();
		}

		protected override void Dispose(bool disposing)
		{
			throw new InvalidOperationException("Sop objects should be disposed by using the Open/Close methods.");
		}
	}
}
