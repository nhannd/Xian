using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// </summary>
	[MenuAction("show", "global-menus/MenuTools/MenuActivityMonitor", "Show")]
	[Tooltip("show", "TooltipActivityMonitor")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class ActivityMonitorTool : Tool<IDesktopToolContext>
	{
		public void Show()
		{
            ActivityMonitorManager.Show(Context.DesktopWindow);
		}
	}
}
