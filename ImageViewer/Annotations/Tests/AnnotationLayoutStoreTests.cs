#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations.Tests
{
	[TestFixture]
	public class AnnotationLayoutStoreTests
	{
		public AnnotationLayoutStoreTests()
		{
		}

		[Test]
		[Ignore("This test won't work unless the settings class is forced to use the LocalSettingsProvider (e.g. by commenting out the SettingsProvider attribute)")]
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
				new AnnotationBox(new RectangleF(0.0F, 0.0F, 0.5F, 0.1F), AnnotationItemProviderManager.Instance.GetAnnotationItem("Dicom.GeneralStudy.StudyDescription")));
			layout.AnnotationBoxGroups[0].AnnotationBoxes.Add(
				new AnnotationBox(new RectangleF(0.0F, 0.1F, 0.5F, 0.2F), AnnotationItemProviderManager.Instance.GetAnnotationItem("Dicom.GeneralStudy.StudyDescription")));

			return layout;
		}
	}
}

#endif