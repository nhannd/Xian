#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class AddImageTest
	{
		public AddImageTest()
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
		public void BuildStudyTree()
		{
			IImageViewer viewer = new MockImageViewerComponent();
			StudyTree studyTree = viewer.StudyTree;

			TestImageSop image1 = new TestImageSop("patient1", "study1", "series1", "image1");
			TestImageSop image2 = new TestImageSop("patient1", "study1", "series1", "image2");
			TestImageSop image3 = new TestImageSop("patient1", "study1", "series2", "image3");
			TestImageSop image4 = new TestImageSop("patient1", "study1", "series2", "image4");
			TestImageSop image5 = new TestImageSop("patient1", "study2", "series3", "image5");
			TestImageSop image6 = new TestImageSop("patient1", "study2", "series3", "image6");
			TestImageSop image7 = new TestImageSop("patient2", "study3", "series4", "image7");
			TestImageSop image8 = new TestImageSop("patient2", "study3", "series4", "image8");
			TestImageSop image9 = new TestImageSop("patient2", "study3", "series5", "image9");

			// This is an internal method.  We would never do this from real
			// client code, but we do it here because we just want to test that
			// images are being properly added to the tree. 
			studyTree.AddImage(image1);
			studyTree.AddImage(image2);
			studyTree.AddImage(image3);
			studyTree.AddImage(image4);
			studyTree.AddImage(image5);
			studyTree.AddImage(image6);
			studyTree.AddImage(image7);
			studyTree.AddImage(image8);
			studyTree.AddImage(image9);

			Assert.IsTrue(studyTree.Patients.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies.Count == 1);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study1"].Series.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study2"].Series.Count == 1);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study2"].Series.Count == 1);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies["study3"].Series.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study1"].Series["series1"].Sops.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study1"].Series["series2"].Sops.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study2"].Series["series3"].Sops.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies["study3"].Series["series4"].Sops.Count == 2);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies["study3"].Series["series5"].Sops.Count == 1);

			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study1"].Series["series1"].Sops["image1"].SopInstanceUID == image1.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study1"].Series["series1"].Sops["image2"].SopInstanceUID == image2.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study1"].Series["series2"].Sops["image3"].SopInstanceUID == image3.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study1"].Series["series2"].Sops["image4"].SopInstanceUID == image4.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study2"].Series["series3"].Sops["image5"].SopInstanceUID == image5.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study2"].Series["series3"].Sops["image6"].SopInstanceUID == image6.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies["study3"].Series["series4"].Sops["image7"].SopInstanceUID == image7.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies["study3"].Series["series4"].Sops["image8"].SopInstanceUID == image8.SopInstanceUID);
			Assert.IsTrue(studyTree.Patients["patient2"].Studies["study3"].Series["series5"].Sops["image9"].SopInstanceUID == image9.SopInstanceUID);

			Assert.IsTrue(studyTree.GetSop("image1").SopInstanceUID == image1.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop("image2").SopInstanceUID == image2.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop("image3").SopInstanceUID == image3.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop("image4").SopInstanceUID == image4.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop("image5").SopInstanceUID == image5.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop("image6").SopInstanceUID == image6.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop("image7").SopInstanceUID == image7.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop("image8").SopInstanceUID == image8.SopInstanceUID);
			Assert.IsTrue(studyTree.GetSop("image9").SopInstanceUID == image9.SopInstanceUID);
		}

		[Test]
		public void AddDuplicateImage()
		{
			IImageViewer viewer = new MockImageViewerComponent();
			StudyTree studyTree = viewer.StudyTree;

			TestImageSop image1 = new TestImageSop("patient1", "study1", "series1", "image1");
			TestImageSop image2 = new TestImageSop("patient1", "study1", "series1", "image1");

			studyTree.AddImage(image1);
			studyTree.AddImage(image2);

			Assert.IsTrue(studyTree.Patients["patient1"].Studies["study1"].Series["series1"].Sops.Count == 1);
		}

		[Test]
		public void AddSameImageFromDifferentViewers()
		{
			IImageViewer viewer1 = new MockImageViewerComponent();
			IImageViewer viewer2 = new MockImageViewerComponent();

			StudyTree studyTree1 = viewer1.StudyTree;
			StudyTree studyTree2 = viewer2.StudyTree;

			// Create 2 TestImageSops that have the same SOP UID
			TestImageSop image1 = new TestImageSop("patient1", "study1", "series1", "image1");
			TestImageSop image2 = new TestImageSop("patient1", "study1", "series1", "image1");

			studyTree1.AddImage(image1);
			IReferenceCountable sop = studyTree1.SopCache[image1.SopInstanceUID];

			Assert.IsTrue(sop.ReferenceCount == 1);
			
			studyTree2.AddImage(image2);
			Assert.IsTrue(sop.ReferenceCount == 2);

			viewer2.Dispose();
			Assert.IsTrue(sop.ReferenceCount == 1);

			viewer1.Dispose();
			Assert.IsTrue(sop.ReferenceCount == 0);
		}
	}
}

#endif