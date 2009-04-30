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

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Tests;
using System;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class AddImageTest : AbstractTest
	{
		public AddImageTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void BuildStudyTree()
		{
			IImageViewer viewer = new ImageViewerComponent();
			StudyTree studyTree = viewer.StudyTree;

			string studyUid1 = DicomUid.GenerateUid().UID;
			string studyUid2 = DicomUid.GenerateUid().UID;
			string studyUid3 = DicomUid.GenerateUid().UID;

			string seriesUid1 = DicomUid.GenerateUid().UID;
			string seriesUid2 = DicomUid.GenerateUid().UID;
			string seriesUid3 = DicomUid.GenerateUid().UID;
			string seriesUid4 = DicomUid.GenerateUid().UID;
			string seriesUid5 = DicomUid.GenerateUid().UID;

			string imageUid1 = DicomUid.GenerateUid().UID;
			string imageUid2 = DicomUid.GenerateUid().UID;
			string imageUid3 = DicomUid.GenerateUid().UID;
			string imageUid4 = DicomUid.GenerateUid().UID;
			string imageUid5 = DicomUid.GenerateUid().UID;
			string imageUid6 = DicomUid.GenerateUid().UID;
			string imageUid7 = DicomUid.GenerateUid().UID;
			string imageUid8 = DicomUid.GenerateUid().UID;
			string imageUid9 = DicomUid.GenerateUid().UID;

			ImageSop image1 = CreateImageSop("patient1", studyUid1, seriesUid1, imageUid1);
			ImageSop image2 = CreateImageSop("patient1", studyUid1, seriesUid1, imageUid2);
			ImageSop image3 = CreateImageSop("patient1", studyUid1, seriesUid2, imageUid3);
			ImageSop image4 = CreateImageSop("patient1", studyUid1, seriesUid2, imageUid4);
			ImageSop image5 = CreateImageSop("patient1", studyUid2, seriesUid3, imageUid5);
			ImageSop image6 = CreateImageSop("patient1", studyUid2, seriesUid3, imageUid6);
			ImageSop image7 = CreateImageSop("patient2", studyUid3, seriesUid4, imageUid7);
			ImageSop image8 = CreateImageSop("patient2", studyUid3, seriesUid4, imageUid8);
			ImageSop image9 = CreateImageSop("patient2", studyUid3, seriesUid5, imageUid9);

			// This is an internal method.  We would never do this from real
			// client code, but we do it here because we just want to test that
			// images are being properly added to the tree. 
			studyTree.AddSop(image1);
			studyTree.AddSop(image2);
			studyTree.AddSop(image3);
			studyTree.AddSop(image4);
			studyTree.AddSop(image5);
			studyTree.AddSop(image6);
			studyTree.AddSop(image7);
			studyTree.AddSop(image8);
			studyTree.AddSop(image9);

			Assert.IsTrue(studyTree.Patients.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies.Count == 1);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid2].Series.Count == 1);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid2].Series.Count == 1);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies[studyUid3].Series.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid1].Sops.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid2].Sops.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid2].Series[seriesUid3].Sops.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies[studyUid3].Series[seriesUid4].Sops.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies[studyUid3].Series[seriesUid5].Sops.Count == 1);

			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid1].Sops[imageUid1].SopInstanceUID == image1.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid1].Sops[imageUid2].SopInstanceUID == image2.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid2].Sops[imageUid3].SopInstanceUID == image3.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid2].Sops[imageUid4].SopInstanceUID == image4.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid2].Series[seriesUid3].Sops[imageUid5].SopInstanceUID == image5.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid2].Series[seriesUid3].Sops[imageUid6].SopInstanceUID == image6.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies[studyUid3].Series[seriesUid4].Sops[imageUid7].SopInstanceUID == image7.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies[studyUid3].Series[seriesUid4].Sops[imageUid8].SopInstanceUID == image8.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies[studyUid3].Series[seriesUid5].Sops[imageUid9].SopInstanceUID == image9.SopInstanceUID);

			Assert.IsTrue(studyTree.GetSop(imageUid1).SopInstanceUID == image1.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop(imageUid2).SopInstanceUID == image2.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop(imageUid3).SopInstanceUID == image3.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop(imageUid4).SopInstanceUID == image4.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop(imageUid5).SopInstanceUID == image5.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop(imageUid6).SopInstanceUID == image6.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop(imageUid7).SopInstanceUID == image7.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop(imageUid8).SopInstanceUID == image8.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop(imageUid9).SopInstanceUID == image9.SopInstanceUID);

			viewer.Dispose();

			Assert.IsTrue(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
		}

		[Test]
		public void AddDuplicateImage()
		{
			IImageViewer viewer = new ImageViewerComponent();
			StudyTree studyTree = viewer.StudyTree;

			string studyUid1 = DicomUid.GenerateUid().UID;
			string seriesUid1 = DicomUid.GenerateUid().UID;
			string imageUid1 = DicomUid.GenerateUid().UID;

			ImageSop image1 = CreateImageSop("patient1", studyUid1, seriesUid1, imageUid1);
			ImageSop image2 = CreateImageSop("patient1", studyUid1, seriesUid1, imageUid1);

			//The sop has already silently disposed the 2nd data source.
			Assert.IsTrue(Object.ReferenceEquals(image1.DataSource, image2.DataSource));
			studyTree.AddSop(image1);
			studyTree.AddSop(image2);

			Assert.IsTrue(studyTree.Patients["patient1"].Studies[studyUid1].Series[seriesUid1].Sops.Count == 1);

			TestDataSource dataSource = (TestDataSource)image1.DataSource;
			viewer.Dispose();

			Assert.IsTrue(dataSource.IsDisposed);
			Assert.IsTrue(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
		}

		private ImageSop CreateImageSop(string patientId, string studyUid, string seriesUid, string sopUid)
		{
			DicomAttributeCollection dataSet = new DicomAttributeCollection();
			base.SetupMR(dataSet);
			DicomFile file = new DicomFile(null, new DicomAttributeCollection(), dataSet);
			TestDataSource dataSource = new TestDataSource(file);
			file.DataSet[DicomTags.PatientId].SetStringValue(patientId);
			file.DataSet[DicomTags.StudyInstanceUid].SetStringValue(studyUid);
			file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(seriesUid);
			file.DataSet[DicomTags.SopInstanceUid].SetStringValue(sopUid);

			return new ImageSop(dataSource);
		}
	}
}

#endif