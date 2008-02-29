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
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.LocalDataStore
{
	public class LocalDataStoreImageSop : LocalImageSop
	{
        private readonly ImageSopInstance _dataStoreImageSopInstance;
        private readonly Dicom.DataStore.Study _dataStoreStudy;
        private readonly Dicom.DataStore.Series _dataStoreSeries;

		public LocalDataStoreImageSop(ImageSopInstance sop)
			: base(sop.LocationUri.LocalDiskPath)
		{
            _dataStoreImageSopInstance = sop;
            _dataStoreStudy = _dataStoreImageSopInstance.GetParentSeries().GetParentStudy() as Dicom.DataStore.Study;
            _dataStoreSeries = _dataStoreImageSopInstance.GetParentSeries() as Dicom.DataStore.Series;
		}

        private ImageSopInstance DataStoreImageSopInstance
        {
            get { return _dataStoreImageSopInstance; }
        }

        private Dicom.DataStore.Study DataStoreStudy
        {
            get { return _dataStoreStudy; }
        }

        private Dicom.DataStore.Series DataStoreSeries
        {
            get { return _dataStoreSeries; }
        }

		public override string TransferSyntaxUID
		{
			get
			{
				return this.DataStoreImageSopInstance.TransferSyntaxUid ?? "";
			}
		}

		public override string SopInstanceUID
		{
			get
			{
				return this.DataStoreImageSopInstance.SopInstanceUid ?? "";
			}
		}

		public override PersonName PatientsName
		{
			get 
            {
				return new PersonName(this.DataStoreStudy.PatientsName ?? "");
            }
		}

        public override string PatientId
        {
            get 
            {
				return this.DataStoreStudy.PatientId ?? "";
            }
        }

		public override string PatientsBirthDate
		{
            get 
            {
                return this.DataStoreStudy.PatientsBirthDateRaw ?? "";
            }
		}

		public override string PatientsSex
		{
            get 
            {
                return this.DataStoreStudy.PatientsSex ?? "";
            }
		}

		public override string StudyInstanceUID
		{
            get 
            {
				return this.DataStoreStudy.StudyInstanceUid ?? "";
            }
		}

		public override string StudyDate
		{
            get 
            {
				return this.DataStoreStudy.StudyDateRaw ?? "";
            }
		}

		public override string StudyTime
		{
            get 
            {
				return this.DataStoreStudy.StudyTimeRaw ?? "";
            }
        }

		public override string AccessionNumber
		{
            get 
            {
				return this.DataStoreStudy.AccessionNumber ?? "";
            }
		}

		public override string StudyDescription
		{
            get 
            {
				return this.DataStoreStudy.StudyDescription ?? "";
            }
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override string FrameOfReferenceUid
		{
			get
			{
				return this.Frames[1].FrameOfReferenceUid;
			}
		}

		public override string Modality
		{
            get 
            {
				return this.DataStoreSeries.Modality ?? "";
            }
		}

		public override string SeriesInstanceUID
		{
            get 
            {
				return this.DataStoreSeries.SeriesInstanceUid ?? "";
            }
		}

		public override int SeriesNumber
		{
            get 
            {
                return DataStoreSeries.SeriesNumber;
            }
		}

		public override string SeriesDescription
		{
			get
			{
				return this.DataStoreSeries.SeriesDescription ?? "";
			}
		}

		public override string Laterality
		{
            get 
            {
				return this.DataStoreSeries.Laterality ?? "";
            }
		}

		public override int InstanceNumber
		{
			get
			{
    			return DataStoreImageSopInstance.InstanceNumber;
			}
		}

		public override int NumberOfFrames
		{
			get
			{
				return Math.Max(DataStoreImageSopInstance.NumberOfFrames, 1);
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override PixelSpacing PixelSpacing
		{
            get 
            {
				return this.Frames[1].PixelSpacing;
            }
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override PixelAspectRatio PixelAspectRatio
        {
            get
            {
				return this.Frames[1].PixelAspectRatio;
            }
        }

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override ImageOrientationPatient ImageOrientationPatient
		{
			get
			{
				return this.Frames[1].ImageOrientationPatient;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override ImagePositionPatient ImagePositionPatient
		{
			get
			{
				return this.Frames[1].ImagePositionPatient;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int SamplesPerPixel
		{
            get 
            {
				return this.Frames[1].SamplesPerPixel;
            }
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				return this.Frames[1].PhotometricInterpretation;
			}
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int Rows
		{
            get 
            {
				return this.Frames[1].Rows;
            }
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int Columns
		{
            get 
            {
				return this.Frames[1].Columns;
            }
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int BitsAllocated
		{
            get 
            {
				return this.Frames[1].BitsAllocated;
            }
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int BitsStored
		{
            get 
            {
				return this.Frames[1].BitsStored;
            }
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int HighBit
		{
            get 
            {
				return this.Frames[1].HighBit;
            }
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int PixelRepresentation
		{
            get { return this.Frames[1].PixelRepresentation; }
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override int PlanarConfiguration
		{
            get 
            {
				return this.Frames[1].PlanarConfiguration;
            }
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override double RescaleIntercept
		{
            get 
            {
                return this.Frames[1].RescaleIntercept;
            }
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override double RescaleSlope
		{
            get 
            {
				return this.Frames[1].RescaleSlope;
            }
		}

		[Obsolete("This method has been deprecated and will be removed in v.1.2. Use equivalent method on Frame class instead.")]
		public override Window[] WindowCenterAndWidth
		{
            get
            {
            	return this.Frames[1].WindowCenterAndWidth;
            }
		}

		protected override void AddFrames()
		{
			for (int i = 1; i <= this.NumberOfFrames; i++)
				this.Frames.Add(new LocalDataStoreFrame(_dataStoreImageSopInstance, this, i));
		}
	}
}
