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

using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.LocalDataStore
{
	public class LocalDataStoreFrame : LocalFrame
	{
		private readonly ImageSopInstance _dataStoreImageSopInstance;
		private Dicom.DataStore.Study _dataStoreStudy;
		private Dicom.DataStore.Series _dataStoreSeries;

		public LocalDataStoreFrame(ImageSopInstance dbSop, LocalDataStoreImageSop parentImageSop, int frameNumber)
			: base(parentImageSop, frameNumber)
		{
			_dataStoreImageSopInstance = dbSop;
			_dataStoreStudy = _dataStoreImageSopInstance.GetParentSeries().GetParentStudy() as Dicom.DataStore.Study;
			_dataStoreSeries = _dataStoreImageSopInstance.GetParentSeries() as Dicom.DataStore.Series;
		}

		public override string FrameOfReferenceUid
		{
			get
			{
				return _dataStoreSeries.FrameOfReferenceUid ?? "";
			}
		}

		public override PixelSpacing PixelSpacing
		{
			get
			{
				return this._dataStoreImageSopInstance.PixelSpacing ?? new PixelSpacing(0, 0);
			}
		}

		public override PixelSpacing ImagerPixelSpacing
		{
			get
			{
				return this._dataStoreImageSopInstance.ImagerPixelSpacing ?? new PixelSpacing(0, 0);
			}
		}

		public override PixelAspectRatio PixelAspectRatio
		{
			get
			{
				return this._dataStoreImageSopInstance.PixelAspectRatio ?? new PixelAspectRatio(0, 0);
			}
		}

		public override ImageOrientationPatient ImageOrientationPatient
		{
			get
			{
				return this._dataStoreImageSopInstance.ImageOrientationPatient ?? new ImageOrientationPatient(0, 0, 0, 0, 0, 0);
			}
		}

		public override ImagePositionPatient ImagePositionPatient
		{
			get
			{
				return this._dataStoreImageSopInstance.ImagePositionPatient ?? new ImagePositionPatient(0, 0, 0);
			}
		}

		public override int SamplesPerPixel
		{
			get
			{
				return this._dataStoreImageSopInstance.SamplesPerPixel;
			}
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				return this._dataStoreImageSopInstance.PhotometricInterpretation;
			}
		}

		public override int Rows
		{
			get
			{
				return this._dataStoreImageSopInstance.Rows;
			}
		}

		public override int Columns
		{
			get
			{
				return this._dataStoreImageSopInstance.Columns;
			}
		}

		public override int BitsAllocated
		{
			get
			{
				return this._dataStoreImageSopInstance.BitsAllocated;
			}
		}

		public override int BitsStored
		{
			get
			{
				return this._dataStoreImageSopInstance.BitsStored;
			}
		}

		public override int HighBit
		{
			get
			{
				return this._dataStoreImageSopInstance.HighBit;
			}
		}

		public override int PixelRepresentation
		{
			get { return this._dataStoreImageSopInstance.PixelRepresentation; }
		}

		public override int PlanarConfiguration
		{
			get
			{
				return this._dataStoreImageSopInstance.PlanarConfiguration;
			}
		}

		public override double RescaleIntercept
		{
			get
			{
				return this._dataStoreImageSopInstance.RescaleIntercept;
			}
		}

		public override double RescaleSlope
		{
			get
			{
				if (this._dataStoreImageSopInstance.RescaleSlope != 0.0)
					return this._dataStoreImageSopInstance.RescaleSlope;

				return 1.0;
			}
		}

		public override double SliceThickness
		{
			get
			{
				return this._dataStoreImageSopInstance.SliceThickness;
			}
		}

		public override double SpacingBetweenSlices
		{
			get
			{
				return this._dataStoreImageSopInstance.SpacingBetweenSlices;
			}
		}

		public override Window[] WindowCenterAndWidth
		{
			get
			{
				if (this._dataStoreImageSopInstance.WindowValues == null || this._dataStoreImageSopInstance.WindowValues.Count == 0)
					return new Window[] { };

				List<Window> windowCentersAndWidths = new List<Window>();

				foreach (object existingWindow in this._dataStoreImageSopInstance.WindowValues)
				{
					Window window = (Window)existingWindow;
					windowCentersAndWidths.Add(new Window(window.Width, window.Center));
				}

				return windowCentersAndWidths.ToArray();
			}
		}
	}
}
