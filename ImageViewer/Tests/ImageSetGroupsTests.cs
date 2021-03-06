#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using NUnit.Framework;
using ClearCanvas.Dicom.Tests;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class ImageSetGroupsTests : AbstractTest
	{
		public ImageSetGroupsTests()
		{
		}

		[Test]
		public void Test()
		{
			IImageSet imageSet1 = CreateImageSet("Patient1", "Patient1");
			IImageSet imageSet2 = CreateImageSet("Patient1", "Patient1");
			ImageSetCollection collection = new ImageSetCollection();
			collection.Add(imageSet1);
			collection.Add(imageSet2);

			ImageSetGroups groups = new ImageSetGroups(collection);
			Assert.IsTrue(groups.Root.Items.Count == 0);
			Assert.IsTrue(groups.Root.ChildGroups.Count == 1);
			Assert.IsTrue(groups.Root.ChildGroups[0].Items.Count == 2);

			imageSet1.Dispose();
			imageSet2.Dispose();
		}

		private IImageSet CreateImageSet(string patientId, string description)
		{
			string studyInstanceUid = DicomUid.GenerateUid().UID;
			string seriesInstanceUid = DicomUid.GenerateUid().UID;
			string sopInstanceUid = DicomUid.GenerateUid().UID;

			ImageSop sop = CreateImageSop(patientId, studyInstanceUid, seriesInstanceUid, sopInstanceUid);
			DicomGrayscalePresentationImage img = new DicomGrayscalePresentationImage(sop.Frames[1]);
			sop.Dispose();

			DisplaySet displaySet = new DisplaySet(patientId, seriesInstanceUid);
			displaySet.PresentationImages.Add(img);
			ImageSet imageSet = new ImageSet();
			imageSet.PatientInfo = description;
			imageSet.DisplaySets.Add(displaySet);

			return imageSet;
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