#if	UNIT_TESTS

using System;
using System.Collections;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.Tests
{
	[TestFixture]
	public class DicomServerEditTest
	{
		public DicomServerEditTest()
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
        public void ChildServersCountTest()
		{
            DicomServerTree dst = new DicomServerTree();
            Assert.Greater(dst.MyServerGroup.ChildServers.Count, 1);
		}
	}
}

#endif