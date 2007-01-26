using System;
using System.Configuration;

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{

	// TODO add a description of the purpose of the settings group here
	[SettingsGroupDescription("Stores Annotation Layout filters in a common place")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class DicomFilteredAnnotationLayoutStoreSettings
	{

		public DicomFilteredAnnotationLayoutStoreSettings()
		{
		}
	}
}
