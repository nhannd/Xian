using System;
using System.Configuration;

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{

	// TODO add a description of the purpose of the settings group here
	[SettingsGroupDescription("Provides a common place to store filters for stored Annotation Layouts")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class DicomFilteredAnnotationLayoutSettings
	{

		public DicomFilteredAnnotationLayoutSettings()
		{
		}
	}
}
