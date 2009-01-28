using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	internal class DesktopWindowTool : Tool<IDesktopToolContext>
	{
		public DesktopWindowTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			KeyImageClipboard.OnDesktopWindowOpened(Context.DesktopWindow);
		}

		protected override void Dispose(bool disposing)
		{
			if (Context != null)
				KeyImageClipboard.OnDesktopWindowClosed(Context.DesktopWindow);

			base.Dispose(disposing);
		}
	}
}
