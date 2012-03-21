using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// </summary>
	[MenuAction("show", "global-menus/MenuTools/ActivityMonitor", "Show")]
	[Tooltip("show", "ActivityMonitor")]
	//[GroupHint("show", "Tools.Layout.HangingProtocols")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	[Obfuscation(Exclude = true, Feature = "renaming", ApplyToMembers = false)]
	public class ActivityMonitorTool : Tool<IDesktopToolContext>
	{
		private ActivityMonitorComponent _component;
		private Workspace _workspace;

		public void Show()
		{
			if (_component == null)
			{
				_component = new ActivityMonitorComponent();
				_workspace = ApplicationComponent.LaunchAsWorkspace(this.Context.DesktopWindow, _component, "ActivityMonitor");
				_workspace.Closed += ((sender, args) =>
										{
											_component = null;
											_workspace = null;
										});
			}
			else
			{
				_workspace.Activate();
			}
		}
	}
}
