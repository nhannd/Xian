using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Externals
{
	public interface IExternalPropertiesView : IView
	{
		void SetExternalLauncher(IExternal externalLauncher);
	}
}