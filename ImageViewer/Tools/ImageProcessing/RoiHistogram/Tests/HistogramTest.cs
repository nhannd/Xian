#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections;
using System.Drawing;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiHistogram
{
	[TestFixture]
	public class HistogramTest
	{
		public HistogramTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
		}
	
		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void Histogram()
		{
			int[] data = { 10, 20, 30, 40, 10, 20, 30, 40, 50, 50, 10, 20, 30 };
			Histogram histogram = new Histogram(0, 60, 5, data);

			int[] bins = histogram.Bins;
		}
	}
}

#endif