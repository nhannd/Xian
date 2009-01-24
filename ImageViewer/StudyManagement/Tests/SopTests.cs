#if UNIT_TESTS

using NUnit.Framework;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Tests;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{
	public class TestDataSource : DicomMessageSopDataSource
	{
		public DicomFile _file;

		public TestDataSource(DicomFile file)
			: base(file)
		{
			_file = file;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_file = null;
		}

		public bool IsDisposed { get { return _file == null; } }
	}

	[TestFixture]
	public class SopTests : AbstractTest
	{
		public SopTests()
		{
		}

		[Test]
		public void TestDisposal()
		{
			IList<DicomAttributeCollection> collections = base.SetupMRSeries(1, 1, "test");
			DicomFile file = new DicomFile(null, new DicomAttributeCollection(), collections[0]);
			TestDataSource dataSource = new TestDataSource(file);
			Sop sop = Sop.Create(dataSource);
			sop.Dispose();
			Assert.IsTrue(dataSource.IsDisposed);
			Assert.IsTrue(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
		}

		[Test]
		public void TestReferences()
		{
			IList<DicomAttributeCollection> collections = base.SetupMRSeries(1, 1, "test");
			DicomFile file = new DicomFile(null, new DicomAttributeCollection(), collections[0]);
			TestDataSource dataSource = new TestDataSource(file);
			Sop sop = Sop.Create(dataSource);

			ISopReference reference1 = sop.CreateTransientReference();
			ISopReference reference2 = sop.CreateTransientReference();
			
			sop.Dispose();
			Assert.IsFalse(dataSource.IsDisposed);
			Assert.IsFalse(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
			
			reference1.Dispose();
			Assert.IsFalse(dataSource.IsDisposed);
			Assert.IsFalse(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
			
			reference2.Dispose();
			Assert.IsTrue(dataSource.IsDisposed);
			Assert.IsTrue(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
		}

		[Test]
		public void TestCaching()
		{
			IList<DicomAttributeCollection> collections = base.SetupMRSeries(1, 1, "test");
			DicomFile file1 = new DicomFile(null, new DicomAttributeCollection(), collections[0].Copy(true));
			DicomFile file2 = new DicomFile(null, new DicomAttributeCollection(), collections[0].Copy(true));

			TestDataSource dataSource1 = new TestDataSource(file1);
			TestDataSource dataSource2 = new TestDataSource(file2);

			Sop sop1 = Sop.Create(dataSource1);
			Assert.IsTrue(SopDataCache.ItemCount == 1, "The Sop data cache should have 1 entry.");
			
			Sop sop2 = Sop.Create(dataSource2);
			Assert.IsTrue(SopDataCache.ItemCount == 1, "The Sop data cache should have 1 entry.");
			Assert.IsTrue(dataSource2.IsDisposed, "The data source has not been disposed.");
			Assert.IsFalse(dataSource1.IsDisposed, "The data source should not be disposed.");

			sop1.Dispose();
			Assert.IsTrue(SopDataCache.ItemCount == 1, "The Sop data cache should have 1 entry.");
			
			ISopReference reference21 = sop2.CreateTransientReference();
			ISopReference reference22 = sop2.CreateTransientReference();

			reference21.Dispose();
			Assert.IsFalse(dataSource1.IsDisposed, "The data source has been disposed.");
			Assert.IsTrue(SopDataCache.ItemCount == 1, "The Sop data cache should have 1 entry.");
			
			sop2.Dispose();
			Assert.IsFalse(dataSource1.IsDisposed, "The data source has been disposed.");
			Assert.IsTrue(SopDataCache.ItemCount == 1, "The Sop data cache should have 1 entry.");
			
			reference22.Dispose();
			Assert.IsTrue(dataSource1.IsDisposed, "The data source has not been disposed.");
			Assert.IsTrue(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
		}
	}
}

#endif