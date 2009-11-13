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
		[Test]
		public void TestIdentitySlicePlane()
		{
			TestVolume(VolumeFunction.Planets, null, volume => ValidateVolumeSlicePlane(volume, VolumeSlicerParams.Identity, PlanetsKnownSamples));
		}

		[Test]
		public void TestOrthoXSlicePlane()
		{
			TestVolume(VolumeFunction.Planets, null, volume => ValidateVolumeSlicePlane(volume, VolumeSlicerParams.OrthogonalX, PlanetsKnownSamples));
		}

		[Test]
		public void TestOrthoYSlicePlane()
		{
			TestVolume(VolumeFunction.Planets, null, volume => ValidateVolumeSlicePlane(volume, VolumeSlicerParams.OrthogonalY, PlanetsKnownSamples));
		}

		[Test]
		public void TestObliqueAlphaSlicePlane()
		{
			TestVolume(VolumeFunction.Planets, null, volume => ValidateVolumeSlicePlane(volume, new VolumeSlicerParams(0, 90, 45), PlanetsKnownSamples));
		}

		[Test]
		public void TestObliqueBetaSlicePlane()
		{
			TestVolume(VolumeFunction.Planets, null, volume => ValidateVolumeSlicePlane(volume, new VolumeSlicerParams(32, -62, 69), PlanetsKnownSamples));
		}

		[Test]
		public void TestObliqueGammaSlicePlane()
		{
			TestVolume(VolumeFunction.Planets, null, volume => ValidateVolumeSlicePlane(volume, new VolumeSlicerParams(60, 0, -30), PlanetsKnownSamples));
		}

		protected static void ValidateVolumeSlicePlane(Volume volume, IVolumeSlicerParams slicerParams, IList<KnownSample> samplePoints)
		{
			Trace.WriteLine(string.Format("Using slice plane: {0}", slicerParams));
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

							foreach (KnownSample sample in samplePoints)
							{
								Vector3D point = dip.ConvertToImagePlane(sample.Point);
								if (point.Z > -0.5 && point.Z < 0.5)
								{
									int actual = imageGraphicProvider.ImageGraphic.PixelData.GetPixel((int) point.X, (int) point.Y);
									Trace.WriteLine(string.Format("Sample {0} @{1} ({2} in this slice plane)", actual, sample.Point, point));
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