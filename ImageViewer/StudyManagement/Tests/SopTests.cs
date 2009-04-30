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

			Assert.IsFalse(dataSource.IsDisposed);
			Assert.IsFalse(SopDataCache.ItemCount == 0, "The Sop data cache is empty.");

			sop.Dispose();
			
			Assert.IsTrue(dataSource.IsDisposed);
			Assert.IsTrue(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
		}

		[Test]
		public void TestReferences1()
		{
			IList<DicomAttributeCollection> collections = base.SetupMRSeries(1, 1, "test");
			DicomFile file = new DicomFile(null, new DicomAttributeCollection(), collections[0]);
			TestDataSource dataSource = new TestDataSource(file);
			Sop sop = Sop.Create(dataSource);

			ISopReference reference1 = sop.CreateTransientReference();
			ISopReference reference2 = sop.CreateTransientReference();
			
			reference1.Dispose();
			Assert.IsFalse(dataSource.IsDisposed);
			Assert.IsFalse(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
			
			reference2.Dispose();
			Assert.IsFalse(dataSource.IsDisposed);
			Assert.IsFalse(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");

			sop.Dispose();
			Assert.IsTrue(dataSource.IsDisposed);
			Assert.IsTrue(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
		}

		[Test]
		public void TestReferences2()
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