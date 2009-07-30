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

using System.Diagnostics;
using System.IO;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom.Tests
{
	[TestFixture]
	public class PresentationStateTests
	{
		[Test]
		public void TestIodRoundtripSpatialTransform()
		{
			TestPresentationState ps = new TestPresentationState();

			for (int rotation = 0; rotation < 360; rotation += 90)
			{
				for (int flipH = 0; flipH < 2; flipH++)
				{
					Trace.WriteLine(string.Format("Testing Roundtrip IOD->IMG->IOD with params Rot={0}, fH={1}", rotation, flipH));

					SpatialTransformModuleIod original = new SpatialTransformModuleIod();
					original.ImageHorizontalFlip = (flipH == 1) ? ImageHorizontalFlip.Y : ImageHorizontalFlip.N;
					original.ImageRotation = rotation;

					SpatialTransformModuleIod actual = ps.SerializeSpatialTransform(ps.DeserializeSpatialTransform(original));

					Assert.AreEqual(original.ImageHorizontalFlip, actual.ImageHorizontalFlip, string.Format("Roundtrip IOD->IMG->IOD FAILED: Rot={0}, fH={1}", rotation, flipH));
					Assert.AreEqual(original.ImageRotation, actual.ImageRotation, string.Format("Roundtrip IOD->IMG->IOD FAILED: Rot={0}, fH={1}", rotation, flipH));
				}
			}
		}

		[Test]
		public void TestImageRoundtripSpatialTransform()
		{
			TestPresentationState ps = new TestPresentationState();

			foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.test.bmp"))
				File.Delete(file);

			for (int rotation = 0; rotation < 360; rotation += 90)
			{
				for (int flipX = 0; flipX < 2; flipX ++)
				{
					for (int flipY = 0; flipY < 2; flipY++)
					{
						Trace.WriteLine(string.Format("Testing Roundtrip IMG->IOD->IMG with params Rot={0}, fX={1}, fY={2}", rotation, flipX, flipY));

						TestPresentationImage original = new TestPresentationImage();
						original.SpatialTransform.FlipX = (flipX%2 == 1);
						original.SpatialTransform.FlipY = (flipY%2 == 1);
						original.SpatialTransform.RotationXY = rotation;
						original.SaveBitmap(string.Format("{0:d2}-{1}-{2}-original.test.bmp", rotation / 10, flipX, flipY));

						TestPresentationImage actual = ps.DeserializeSpatialTransform(ps.SerializeSpatialTransform(original));
						actual.SaveBitmap(string.Format("{0:d2}-{1}-{2}-actual.test.bmp", rotation / 10, flipX, flipY));

						Statistics stats = original.Diff(actual);
						Trace.WriteLine(string.Format("DIFF STATS {0}", stats));
						Assert.IsTrue(stats.IsEqualTo(0, 5), string.Format("Roundtrip IMG->IOD->IMG FAILED: Rot={0}, fX={1}, fY={2}", rotation, flipX, flipY));

						actual.Dispose();
						original.Dispose();
					}
				}
			}
		}
	}
}

#endif