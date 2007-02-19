using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{

	[SettingsGroupDescription("Stores Filters for the selection of Text Annotation Layouts in a common place")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class DicomFilteredAnnotationLayoutStoreSettings
	{
		private DicomFilteredAnnotationLayoutStoreSettings()
		{
			ApplicationSettingsRegister.Instance.RegisterInstance(this);
		}

		~DicomFilteredAnnotationLayoutStoreSettings()
		{
			ApplicationSettingsRegister.Instance.UnregisterInstance(this);
		}
	}
}
