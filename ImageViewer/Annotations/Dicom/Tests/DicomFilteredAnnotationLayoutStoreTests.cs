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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Annotations.Dicom.Tests
{
#pragma warning disable 1591,0419,1574,1587

	[TestFixture]
	public class DicomFilteredAnnotationLayoutStoreTests
	{
		public DicomFilteredAnnotationLayoutStoreTests()
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
			DicomFilteredAnnotationLayoutStore.Instance.Clear();

			IList<DicomFilteredAnnotationLayout> layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(layouts.Count, 0);

			DicomFilteredAnnotationLayoutStore.Instance.Update(CreateLayout("testLayout1", "Dicom.MR", "MR"));
			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(layouts.Count, 1);

			layouts = new List<DicomFilteredAnnotationLayout>();
			layouts.Clear();
			layouts.Add(CreateLayout("testLayout1", "Dicom.MR", "MR"));
			layouts.Add(CreateLayout("testLayout2", "Dicom.MG", "MG"));
			layouts.Add(CreateLayout("testLayout3", "Dicom.CT", "CT"));
			layouts.Add(CreateLayout("testLayout4", "Dicom.PT", "PT"));

			DicomFilteredAnnotationLayoutStore.Instance.Update(layouts);

			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(layouts.Count, 4);

			ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
			using (Stream stream = resolver.OpenResource("DicomFilteredAnnotationLayoutStoreDefaults.xml"))
			{
				StreamReader reader = new StreamReader(stream);
				DicomFilteredAnnotationLayoutStoreSettings.Default.FilteredLayoutSettingsXml = reader.ReadToEnd();
			}

			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(layouts.Count, 8);

			DicomFilteredAnnotationLayout layout = DicomFilteredAnnotationLayoutStore.Instance.GetFilteredLayout("Dicom.Filtered.MR");
			layout = CopyLayout(layout, "Dicom.Filtered.RT");
			layout.Filters[0] = new KeyValuePair<string,string>("Modality", "RT");

			DicomFilteredAnnotationLayoutStore.Instance.Update(layout);
			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(layouts.Count, 9);

			layout = DicomFilteredAnnotationLayoutStore.Instance.GetFilteredLayout("Dicom.Filtered.RT");
			Assert.AreNotEqual(layout, null);

			DicomFilteredAnnotationLayoutStore.Instance.RemoveFilteredLayout("Dicom.Filtered.RT");
			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(layouts.Count, 8);

			layout = DicomFilteredAnnotationLayoutStore.Instance.GetFilteredLayout("Dicom.Filtered.RT");
			Assert.AreEqual(layout, null);

			layout = DicomFilteredAnnotationLayoutStore.Instance.GetFilteredLayout("Dicom.Filtered.MR");
			Assert.AreNotEqual(layout, null);

			layouts = new List<DicomFilteredAnnotationLayout>();
			layouts.Clear();
			layouts.Add(CreateLayout("Dicom.Filtered.RT", "Dicom.RT", "RT"));
			layouts.Add(CreateLayout("Dicom.Filtered.SC", "Dicom.SC", "SC"));
			layouts.Add(CreateLayout("Dicom.Filtered.US", "Dicom.US", "US"));
			layouts.Add(CreateLayout("Dicom.Filtered.ES", "Dicom.ES", "ES"));

			DicomFilteredAnnotationLayoutStore.Instance.Update(layouts);

			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(layouts.Count, 12);

			DicomFilteredAnnotationLayoutStore.Instance.RemoveFilteredLayout("Dicom.Filtered.RT");
			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(layouts.Count, 11);

			DicomFilteredAnnotationLayoutStore.Instance.RemoveFilteredLayout("Dicom.Filtered.SC");
			layouts = DicomFilteredAnnotationLayoutStore.Instance.FilteredLayouts;
			Assert.AreEqual(layouts.Count, 10);

			layout = DicomFilteredAnnotationLayoutStore.Instance.GetFilteredLayout("Dicom.Filtered.AllMatch");
			Assert.AreNotEqual(layout, null);
			Assert.AreEqual(layout.Filters.Count, 0);

			layout = DicomFilteredAnnotationLayoutStore.Instance.GetFilteredLayout("Dicom.Filtered.MR");
			Assert.AreEqual(layout.Filters.Count, 1);
		}

		DicomFilteredAnnotationLayout CopyLayout(DicomFilteredAnnotationLayout layout, string newIdentifier)
		{
			DicomFilteredAnnotationLayout newLayout = new DicomFilteredAnnotationLayout(newIdentifier, layout.MatchingLayoutIdentifier);
			foreach (KeyValuePair<string, string> filter in layout.Filters)
				newLayout.Filters.Add(filter);

			return newLayout;
		}

		DicomFilteredAnnotationLayout CreateLayout(string identifier, string matchingLayoutId, string modality)
		{
			DicomFilteredAnnotationLayout newLayout = new DicomFilteredAnnotationLayout(identifier, matchingLayoutId);
			if (modality != "")
				newLayout.Filters.Add(new KeyValuePair<string,string>("Modality", modality));
			
			return newLayout;
		}
	}
}

#endif