#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

					TestPresentationImage image = ps.DeserializeSpatialTransform(original);
					using (image)
					{
						SpatialTransformModuleIod actual = ps.SerializeSpatialTransform(image);
					Assert.AreEqual(original.ImageHorizontalFlip, actual.ImageHorizontalFlip, string.Format("Roundtrip IOD->IMG->IOD FAILED: Rot={0}, fH={1}", rotation, flipH));
					Assert.AreEqual(original.ImageRotation, actual.ImageRotation, string.Format("Roundtrip IOD->IMG->IOD FAILED: Rot={0}, fH={1}", rotation, flipH));
					}
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