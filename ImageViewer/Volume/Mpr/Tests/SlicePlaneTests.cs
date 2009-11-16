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
using System.Diagnostics;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	[TestFixture]
	public class SlicePlaneTests : AbstractMprTest
	{
		private readonly IList<IVolumeSlicerParams> _slicings;

		public SlicePlaneTests()
		{
			List<IVolumeSlicerParams> slicings = new List<IVolumeSlicerParams>();
			slicings.Add(VolumeSlicerParams.Identity);
			slicings.Add(VolumeSlicerParams.OrthogonalX);
			slicings.Add(VolumeSlicerParams.OrthogonalY);
			slicings.Add(new VolumeSlicerParams(0, 90, 45));
			slicings.Add(new VolumeSlicerParams(32, -62, 69));
			slicings.Add(new VolumeSlicerParams(60, 0, -30));
			slicings.Add(new VolumeSlicerParams(-15, 126, -30));
			_slicings = slicings.AsReadOnly();
		}

		[Test]
		public void TestNoTiltSlicings()
		{
			TestVolume(VolumeFunction.Stars,
			           null,
			           volume =>
			           	{
			           		foreach (IVolumeSlicerParams slicing in _slicings)
			           		{
			           			ValidateVolumeSlicePoints(volume, slicing, StarsKnownSamples);
			           		}
			           	});
		}

		[Test]
		public void TestPositiveXAxialGantryTiltedSlicings()
		{
			TestVolume(VolumeFunction.StarsTilted030X,
			           SetGantryTiltAboutX(30, true),
			           volume =>
			           	{
			           		foreach (IVolumeSlicerParams slicing in _slicings)
			           		{
			           			ValidateVolumeSlicePoints(volume, slicing, StarsKnownSamples, 30, 0, true);
			           		}
			           	});
		}

		[Test]
		public void TestNegativeXAxialGantryTiltedSlicings()
		{
			TestVolume(VolumeFunction.StarsTilted345X,
			           SetGantryTiltAboutX(-15, true),
			           volume =>
			           	{
			           		foreach (IVolumeSlicerParams slicing in _slicings)
			           		{
			           			ValidateVolumeSlicePoints(volume, slicing, StarsKnownSamples, -15, 0, true);
			           		}
			           	});
		}

		protected static InitializeSopDataSourceDelegate SetGantryTiltAboutX(double angle, bool degrees)
		{
			return sopDataSource => sopDataSource[DicomTags.ImageOrientationPatient].SetStringValue(ConvertXAxialGantryTiltToImageOrientationPatient(angle, degrees));
		}

		protected static void ValidateVolumeSlicePoints(Volume volume, IVolumeSlicerParams slicerParams, IList<KnownSample> expectedPoints)
		{
			ValidateVolumeSlicePoints(volume, slicerParams, expectedPoints, 0, 0, false);
		}

		protected static void ValidateVolumeSlicePoints(Volume volume, IVolumeSlicerParams slicerParams, IList<KnownSample> expectedPoints,
		                                                double xAxialGantryTilt, double yAxialGantryTilt, bool gantryTiltInDegrees)
		{
			if (gantryTiltInDegrees)
			{
				xAxialGantryTilt *= Math.PI/180;
				yAxialGantryTilt *= Math.PI/180;
			}

			Trace.WriteLine(string.Format("Using slice plane: {0}", slicerParams.Description));
			using (VolumeSlicer slicer = new VolumeSlicer(volume, slicerParams, DicomUid.GenerateUid().UID))
			{
				foreach (ISopDataSource slice in slicer.CreateSlices())
				{
					using (ImageSop imageSop = new ImageSop(slice))
					{
						foreach (IPresentationImage image in PresentationImageFactory.Create(imageSop))
						{
							IImageGraphicProvider imageGraphicProvider = (IImageGraphicProvider) image;
							DicomImagePlane dip = DicomImagePlane.FromImage(image);

							foreach (KnownSample sample in expectedPoints)
							{
								Vector3D patientPoint = sample.Point;
								if (xAxialGantryTilt != 0 && yAxialGantryTilt == 0)
								{
									float cos = (float) Math.Cos(xAxialGantryTilt);
									float sin = (float) Math.Sin(xAxialGantryTilt);
									patientPoint = new Vector3D(patientPoint.X,
									                            patientPoint.Y*cos + (xAxialGantryTilt > 0 ? 100*sin : 0),
									                            patientPoint.Z/cos - patientPoint.Y*sin - (xAxialGantryTilt > 0 ? 100*sin*sin/cos : 0));
								}
								else if (yAxialGantryTilt != 0)
								{
									Assert.Fail("Unit test not designed to work with gantry tilts about Y (i.e. slew)");
								}

								Vector3D slicedPoint = dip.ConvertToImagePlane(patientPoint);
								if (slicedPoint.Z > -0.5 && slicedPoint.Z < 0.5)
								{
									int actual = imageGraphicProvider.ImageGraphic.PixelData.GetPixel((int) slicedPoint.X, (int) slicedPoint.Y);
									Trace.WriteLine(string.Format("Sample {0} @{1} (SLICE: {2}; PATIENT: {3})", actual, FormatVector(sample.Point), FormatVector(slicedPoint), FormatVector(patientPoint)));
									Assert.AreEqual(sample.Value, actual, "Wrong colour sample @{0}", sample.Point);
								}
							}

							image.Dispose();
						}
					}
					slice.Dispose();
				}
			}
		}
	}
}

#endif