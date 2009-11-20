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
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	/// <summary>
	/// These unit tests exercise the VolumeBuilder and the constraints on acceptable frame data sources for MPR
	/// </summary>
	[TestFixture]
	public class VolumeBuilderTest : AbstractMprTest
	{
		[Test]
		public void TestWellBehavedSource()
		{
			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, null, null);
		}

		[Test]
		[ExpectedException(typeof (InsufficientFramesException))]
		public void TestInsufficientFramesSource()
		{
			// it doesn't really matter what function we use
			VolumeFunction function = VolumeFunction.Void.Normalize(100);

			List<ImageSop> images = new List<ImageSop>();
			try
			{
				// create only 2 slices!!
				foreach (ISopDataSource sopDataSource in function.CreateSops(100, 100, 2, false))
				{
					images.Add(new ImageSop(sopDataSource));
				}

				// this line *should* throw an exception
				using (Volume volume = Volume.Create(EnumerateFrames(images))) {}
			}
			finally
			{
				DisposeAll(images);
			}
		}

		[Test]
		[ExpectedException(typeof (MultipleSourceSeriesException))]
		public void TestDifferentSeriesSource()
		{
			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID), null);
		}

		[Test]
		[ExpectedException(typeof (MultipleFramesOfReferenceException))]
		public void TestDifferentFramesOfReferenceSource()
		{
			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.FrameOfReferenceUid].SetStringValue(DicomUid.GenerateUid().UID), null);
		}

		[Test]
		[ExpectedException(typeof (NullImageOrientationException))]
		public void TestMissingImageOrientationSource()
		{
			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.ImageOrientationPatient].SetEmptyValue(), null);
		}

		[Test]
		[ExpectedException(typeof (MultipleImageOrientationsException))]
		public void TestDifferentImageOrientationSource()
		{
			int n = 0;

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.ImageOrientationPatient].SetStringValue(DataSetOrientation.CreateGantryTiltedAboutX(n++).ImageOrientationPatient), null);
		}

		[Test]
		[ExpectedException(typeof (UnevenlySpacedFramesException))]
		public void TestUnevenlySpacedFramesSource()
		{
			int sliceSpacing = 1;
			int sliceLocation = 100;

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void,
			           sopDataSource =>
			           	{
			           		sopDataSource[DicomTags.ImagePositionPatient].SetStringValue(string.Format(@"100\100\{0}", sliceLocation));
			           		sliceLocation += sliceSpacing++;
			           	}, null);
		}

		[Test]
		[ExpectedException(typeof (UnevenlySpacedFramesException))]
		public void TestCoincidentFramesSource()
		{
			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.ImagePositionPatient].SetStringValue(@"100\100\100"), null);
		}

		[Test]
		[ExpectedException(typeof (UnsupportedGantryTiltAxisException))]
		public void TestMultiAxialGantryTiltedSource()
		{
			string imageOrientationPatient = string.Format(@"{1:f9}\{1:f9}\{0:f9}\-{0:f9}\{0:f9}\0", Math.Cos(Math.PI/4), Math.Pow(Math.Cos(Math.PI/4), 2));

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.ImageOrientationPatient].SetStringValue(imageOrientationPatient), null);
		}

		[Test]
		public void Test030DegreeXAxialRotationGantryTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateGantryTiltedAboutX(30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void Test330DegreeXAxialRotationGantryTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateGantryTiltedAboutX(-30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		[ExpectedException(typeof (UnsupportedGantryTiltAxisException))]
		public void Test030DegreeYAxialRotationGantryTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateGantryTiltedAboutY(30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		[ExpectedException(typeof (UnsupportedGantryTiltAxisException))]
		public void Test330DegreeYAxialRotationGantryTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateGantryTiltedAboutY(-30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void Test030DegreeXAxialRotationCouchTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateCouchTiltedAboutX(30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void Test330DegreeXAxialRotationCouchTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateCouchTiltedAboutX(-30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void Test030DegreeYAxialRotationCouchTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateCouchTiltedAboutY(30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void Test330DegreeYAxialRotationCouchTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateCouchTiltedAboutY(-30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void TestAxialSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateAxial(false);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void TestCoronalSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateCoronal(false);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void TestSagittalSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateSagittal(false);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void TestFilledVoxelData()
		{
			TestVolume(VolumeFunction.Stars, null,
			           volume =>
			           	{
			           		foreach (KnownSample sample in StarsKnownSamples)
			           		{
			           			int actual = volume[(int) sample.Point.X, (int) sample.Point.Y, (int) sample.Point.Z];
			           			Trace.WriteLine(string.Format("Sample {0} @{1}", actual, sample.Point));
			           			Assert.AreEqual(sample.Value, actual, "Wrong colour sample @{0}", sample.Point);
			           		}
			           	});
		}

		[Test]
		public void TestXAxialRotationGantryTiltedVoxelData()
		{
			const double angle = 30;
			DataSetOrientation orientation = DataSetOrientation.CreateGantryTiltedAboutX(angle);

			TestVolume(VolumeFunction.Stars,
			           orientation.Initialize,
			           volume =>
			           	{
			           		foreach (KnownSample sample in StarsKnownSamples)
			           		{
			           			Vector3D realPoint = sample.Point;
			           			Vector3D paddedPoint = realPoint + new Vector3D(0, (float) (Math.Tan(angle*Math.PI/180)*(100 - realPoint.Z)), 0);

			           			int actual = volume[(int) paddedPoint.X, (int) paddedPoint.Y, (int) paddedPoint.Z];
			           			Trace.WriteLine(string.Format("Sample {0} @{1} ({2} before padding)", actual, paddedPoint, realPoint));
			           			Assert.AreEqual(sample.Value, actual, "Wrong colour sample @{0} ({1} before padding)", paddedPoint, realPoint);
			           		}
			           	});
		}
	}
}

#endif