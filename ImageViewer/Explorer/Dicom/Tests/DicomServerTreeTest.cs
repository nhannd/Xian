#if	UNIT_TESTS

using System;
using System.Collections;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.Tests
{
	[TestFixture]
	public class DicomServerTreeTest
	{
		public DicomServerTreeTest()
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
        public void ChildServersNullTest()
		{
            DicomServerTree dst = new DicomServerTree();
            Assert.IsNotNull(dst.MyServerGroup.ChildServers);
		}
	}
}

#endif