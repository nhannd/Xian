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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming.Tests
{
	[TestFixture]
	public class StreamingSopDataSourceTests
	{
		[TestFixtureSetUp]
		public void Init()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[Test]
		public void TestIsEmbedded()
		{
			// no values => not embedded
			AssertIsEmbedded(false, false, null, null, null, null);

			// normal embedded => embedded
			AssertIsEmbedded(true, false, 16, 15, 14, 15);
			AssertIsEmbedded(true, false, 16, 15, 15, 0);
			AssertIsEmbedded(true, false, 8, 6, 7, 0);
			AssertIsEmbedded(true, false, 8, 6, 5, 6);

			// has embedded attributes, but already has overlay data => not embedded
			AssertIsEmbedded(false, true, 16, 15, 14, 15);

			// we don't check for missing pixel data because it is streamed separately even if it were there

			// not 8 or 16 bits allocated => not embedded
			AssertIsEmbedded(false, false, null, 12, 11, 13);
			AssertIsEmbedded(false, false, 6, 12, 11, 13);

			// bit position overlaps pixel data as defined by bits stored and high bit => not embedded
			AssertIsEmbedded(false, false, 16, 12, 11, 11);
		}

		private static void AssertIsEmbedded(bool expectedResult, bool hasOverlayData, int? bitsAllocated, int? bitsStored, int? highBit, int? overlayBitPosition)
		{
			var dataset = new DicomAttributeCollection();

			if (hasOverlayData)
				dataset[DicomTags.OverlayData].Values = new byte[1];
			if (bitsAllocated.HasValue)
				dataset[DicomTags.BitsAllocated].SetInt32(0, bitsAllocated.Value);
			if (bitsStored.HasValue)
				dataset[DicomTags.BitsStored].SetInt32(0, bitsStored.Value);
			if (highBit.HasValue)
				dataset[DicomTags.HighBit].SetInt32(0, highBit.Value);
			if (overlayBitPosition.HasValue)
				dataset[DicomTags.OverlayBitPosition].SetInt32(0, overlayBitPosition.Value);

			var plane = new OverlayPlaneModuleIod(dataset)[0];
			Assert.AreEqual(expectedResult, StreamingSopDataSource.IsOverlayEmbedded(plane),
			                "Embedded overlay status incorrectly detected (OD:{0},PD:{1},bA{2},bS{3},hb{4},bp{5})",
			                hasOverlayData, bitsAllocated, bitsStored, highBit, overlayBitPosition);
		}
	}
}

#endif