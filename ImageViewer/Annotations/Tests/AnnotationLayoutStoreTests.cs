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

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations.Tests
{
#pragma warning disable 1591,0419,1574,1587

	[TestFixture]
	public class AnnotationLayoutStoreTests
	{
		public AnnotationLayoutStoreTests()
		{
		}

		[TestFixtureSetUp]
		public void Setup()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[Test]
		public void Test()
		{
			List<IAnnotationItem> annotationItems = new List<IAnnotationItem>();

			AnnotationLayoutStore.Instance.Clear();

			IList<StoredAnnotationLayout> layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
			Assert.AreEqual(layouts.Count, 0);

			AnnotationLayoutStore.Instance.Update(this.CreateLayout("testLayout1"));
			layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
			Assert.AreEqual(layouts.Count, 1);

			layouts = new List<StoredAnnotationLayout>();
			layouts.Clear();
			layouts.Add(CreateLayout("testLayout1"));
			layouts.Add(CreateLayout("testLayout2"));
			layouts.Add(CreateLayout("testLayout3"));
			layouts.Add(CreateLayout("testLayout4"));

			AnnotationLayoutStore.Instance.Update(layouts);

			layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
			Assert.AreEqual(layouts.Count, 4);

			ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
			using (Stream stream = resolver.OpenResource("AnnotationLayoutStoreDefaults.xml"))
			{
				StreamReader reader = new StreamReader(stream);
				AnnotationLayoutStoreSettings.Default.LayoutSettingsXml = reader.ReadToEnd();
			}

			layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
			Assert.AreEqual(layouts.Count, 8);

			StoredAnnotationLayout layout = AnnotationLayoutStore.Instance.GetLayout("Dicom.OT", annotationItems);
			layout = CopyLayout(layout, "Dicom.OT.Copied");

			AnnotationLayoutStore.Instance.Update(layout);
			layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
			Assert.AreEqual(layouts.Count, 9);

			layout = AnnotationLayoutStore.Instance.GetLayout("Dicom.OT.Copied", annotationItems);
			Assert.AreNotEqual(layout, null);
			
			AnnotationLayoutStore.Instance.RemoveLayout("Dicom.OT.Copied");
			layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
			Assert.AreEqual(layouts.Count, 8);

			layout = AnnotationLayoutStore.Instance.GetLayout("Dicom.OT.Copied", annotationItems);
			Assert.AreEqual(layout, null);

			layout = AnnotationLayoutStore.Instance.GetLayout("Dicom.OT", annotationItems);
			Assert.AreNotEqual(layout, null);

			layouts = new List<StoredAnnotationLayout>(); 
			layouts.Clear();
			layouts.Add(CreateLayout("testLayout1"));
			layouts.Add(CreateLayout("testLayout2"));
			layouts.Add(CreateLayout("testLayout3"));
			layouts.Add(CreateLayout("testLayout4"));

			AnnotationLayoutStore.Instance.Update(layouts);

			layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
			Assert.AreEqual(layouts.Count, 12);

			AnnotationLayoutStore.Instance.RemoveLayout("testLayout1");
			layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
			Assert.AreEqual(layouts.Count, 11);

			AnnotationLayoutStore.Instance.RemoveLayout("testLayout2");
			layouts = AnnotationLayoutStore.Instance.GetLayouts(annotationItems);
			Assert.AreEqual(layouts.Count, 10);

			layout = AnnotationLayoutStore.Instance.GetLayout("Dicom.OT", annotationItems);
			Assert.AreNotEqual(layout, null);

			layout = AnnotationLayoutStore.Instance.GetLayout("testLayout3", annotationItems); 
			Assert.AreEqual(layout.AnnotationBoxGroups.Count, 1);

			layout = AnnotationLayoutStore.Instance.GetLayout("Dicom.OT", annotationItems);
			layout = CopyLayout(layout, "testLayout3");
			AnnotationLayoutStore.Instance.Update(layout);
			layout = AnnotationLayoutStore.Instance.GetLayout("testLayout3", annotationItems);
			Assert.AreEqual(layout.AnnotationBoxGroups.Count, 4);
		}

		StoredAnnotationLayout CopyLayout(StoredAnnotationLayout layout, string newIdentifier)
		{
			StoredAnnotationLayout newLayout = new StoredAnnotationLayout(newIdentifier);
			foreach (StoredAnnotationBoxGroup group in layout.AnnotationBoxGroups)
				newLayout.AnnotationBoxGroups.Add(group);
			
			return newLayout;
		}

		StoredAnnotationLayout CreateLayout(string identifier)
		{
			StoredAnnotationLayout layout = new StoredAnnotationLayout(identifier);
			layout.AnnotationBoxGroups.Add(new StoredAnnotationBoxGroup("group1"));
			layout.AnnotationBoxGroups[0].AnnotationBoxes.Add(
				new AnnotationBox(new RectangleF(0.0F, 0.0F, 0.5F, 0.1F), AnnotationLayoutFactory.GetAnnotationItem("Dicom.GeneralStudy.StudyDescription")));
			layout.AnnotationBoxGroups[0].AnnotationBoxes.Add(
				new AnnotationBox(new RectangleF(0.0F, 0.1F, 0.5F, 0.2F), AnnotationLayoutFactory.GetAnnotationItem("Dicom.GeneralStudy.StudyDescription")));

			return layout;
		}
	}
}

#endif