#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
			dcf.DataSet[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
			dcf.DataSet[DicomTags.InstanceNumber].SetStringValue("1");
			dcf.DataSet[DicomTags.NumberOfFrames].SetStringValue("1");
			dcf.MetaInfo[DicomTags.TransferSyntaxUid].SetStringValue(TransferSyntax.ImplicitVrLittleEndianUid);
			dcf.MetaInfo[DicomTags.MediaStorageSopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
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

		//[Obsolete("CreateFreshCopy is not implemented by this test type.", true)]
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