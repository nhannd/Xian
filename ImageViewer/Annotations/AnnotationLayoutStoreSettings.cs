using System;
using System.Configuration;

namespace ClearCanvas.ImageViewer.Annotations
{
	// TODO add a description of the purpose of the settings group here
	[SettingsGroupDescription("Stores Annotation Layouts in a common place")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class AnnotationLayoutStoreSettings
	{
		public AnnotationLayoutStoreSettings()
		{
		}
	}
}
