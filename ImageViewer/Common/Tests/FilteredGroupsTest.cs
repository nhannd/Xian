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

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using NUnit.Framework;
using ClearCanvas.Dicom.Tests;

namespace ClearCanvas.ImageViewer.Common.Tests
{
	[TestFixture]
	public class FilteredGroupsTest : AbstractTest
	{
		public FilteredGroupsTest()
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