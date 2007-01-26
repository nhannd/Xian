using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace ClearCanvas.ImageViewer.Annotations.Dicom.Tests
{
	[TestFixture]
	public class DicomFilteredAnnotationLayoutStoreTests
	{
		public DicomFilteredAnnotationLayoutStoreTests()
		{ 
		}

		[Test]
		[Ignore("This test won't work unless the settings class is forced to use the LocalSettingsProvider")]
		public void Test()
		{
			string filepath = String.Format(".\\{0}", DicomFilteredAnnotationLayoutStore.DefaultSettingsFile);
			if (File.Exists(filepath))
				File.Move(filepath, ".\\tmp.xml");

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

			if (File.Exists(".\\tmp.xml"))
				File.Move(".\\tmp.xml", filepath);

			DicomFilteredAnnotationLayoutStore.Instance.Clear();

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
