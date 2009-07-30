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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using ClearCanvas.ImageViewer.Tests;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom.Tests
{
	internal class TestPresentationImage : BasicPresentationImage, IDicomPresentationImage
	{
		private const int _height = 32;
		private const int _width = 32;

		private readonly ImageSop _imageSop;

		public TestPresentationImage() : base(TestPattern.CreateRGBKCorners(new Size(_width, _height)))
		{
			DicomFile dcf = new DicomFile();
			dcf.DataSet[DicomTags.StudyInstanceUid].SetStringValue("1");
			dcf.DataSet[DicomTags.SeriesInstanceUid].SetStringValue("2");
			dcf.DataSet[DicomTags.SopInstanceUid].SetStringValue("3");
			dcf.DataSet[DicomTags.SopClassUid].SetStringValue(SopClass.RawDataStorageUid);
			dcf.DataSet[DicomTags.InstanceNumber].SetStringValue("1");
			dcf.DataSet[DicomTags.NumberOfFrames].SetStringValue("1");
			dcf.MetaInfo[DicomTags.TransferSyntaxUid].SetStringValue(TransferSyntax.ImplicitVrLittleEndianUid);
			dcf.MetaInfo[DicomTags.MediaStorageSopClassUid].SetStringValue(SopClass.RawDataStorageUid);
			dcf.MetaInfo[DicomTags.MediaStorageSopInstanceUid].SetStringValue("3");
			_imageSop = new ImageSop(new TestDataSource(dcf));
		}

		public Statistics Diff(TestPresentationImage other)
		{
			if (other == null)
				throw new ArgumentNullException();

			List<int> diffs = new List<int>();

			using (Bitmap thatBitmap = other.DrawToBitmap(_width, _height))
			{
				using (Bitmap thisBitmap = this.DrawToBitmap(_width, _height))
				{
					for (int x = 0; x < _width; x++)
					{
						for (int y = 0; y < _height; y++)
						{
							Color thatColor = thatBitmap.GetPixel(x, y);
							Color thisColor = thisBitmap.GetPixel(x, y);

							diffs.Add(Math.Abs(thatColor.R - thisColor.R));
							diffs.Add(Math.Abs(thatColor.G - thisColor.G));
							diffs.Add(Math.Abs(thatColor.B - thisColor.B));
							diffs.Add(Math.Abs(thatColor.A - thisColor.A));
						}
					}
				}
			}

			return new Statistics(diffs);
		}

		public void SaveBitmap(string filename)
		{
			using (Bitmap bmp = this.DrawToBitmap(_width, _height))
			{
				bmp.Save(filename);
			}
		}

		[Obsolete("CreateFreshCopy is not implemented by this test type.", true)]
		public override IPresentationImage CreateFreshCopy()
		{
			throw new NotImplementedException();
		}

		#region IDicomPresentationImage Members (Not Implemented)

		GraphicCollection IDicomPresentationImage.DicomGraphics
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IImageSopProvider Members

		ImageSop IImageSopProvider.ImageSop
		{
			get { return _imageSop; }
		}

		Frame IImageSopProvider.Frame
		{
			get { return _imageSop.Frames[0]; }
		}

		#endregion

		#region ISopProvider Members (Not Implemented)

		Sop ISopProvider.Sop
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IDicomSoftcopyPresentationStateProvider Members (Not Implemented)

		DicomSoftcopyPresentationState IDicomSoftcopyPresentationStateProvider.PresentationState
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_imageSop.Dispose();
			}
		}
	}
}

#endif