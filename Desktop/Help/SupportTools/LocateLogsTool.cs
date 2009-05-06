using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Help.SupportTools
{
	[MenuAction("locate", "global-menus/MenuHelp/MenuShowLogs", "Locate")]
	[GroupHint("locate", "Application.Help.Support")]
	[ExtensionOf(typeof (DesktopToolExtensionPoint))]
	public class LocateLogsTool : Tool<IDesktopToolContext>
	{
		public void Locate()
		{
			string logdir = Platform.LogDirectory;
			if (!string.IsNullOrEmpty(logdir) && Directory.Exists(logdir))
			{
				Process.Start(logdir);
			}
			return;
		}
	}
}