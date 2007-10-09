using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Annotations
{
	// TODO add a description of the purpose of the settings group here
	[SettingsGroupDescription("Stores Text Annotation Layouts in a common place")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	internal sealed partial class AnnotationLayoutStoreSettings
	{
		private AnnotationLayoutStoreSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
