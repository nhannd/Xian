using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.Collections.Generic;

namespace ClearCanvas.Desktop.Explorer
{
	[ExtensionPoint()]
	public class HealthcareArtifactExplorerExtensionPoint : ExtensionPoint<IHealthcareArtifactExplorer>
	{
	}

    [MenuAction("show", "global-menus/MenuFile/MenuFileSearch")]
	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarToolsStandardStudyCentre")]
	[ClickHandler("show", "Show")]
	[IconSet("show", IconScheme.Colour, "", "Icons.DashboardMedium.png", "Icons.DashboardLarge.png")]

    [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ExplorerTool : DesktopTool
	{
		TabComponentContainer _tabComponentContainer;
		List<IHealthcareArtifactExplorer> _healthcareArtifactExplorers;

		public ExplorerTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public void Show()
		{
			if (_tabComponentContainer == null)
			{
				_tabComponentContainer = new TabComponentContainer();

				HealthcareArtifactExplorerExtensionPoint xp = new HealthcareArtifactExplorerExtensionPoint();
				object[] extensions = xp.CreateExtensions();

				if (_healthcareArtifactExplorers == null)
				{
					_healthcareArtifactExplorers = new List<IHealthcareArtifactExplorer>();

					foreach (IHealthcareArtifactExplorer explorer in extensions)
					{
						_healthcareArtifactExplorers.Add(explorer);
					}
				}

				foreach (IHealthcareArtifactExplorer explorer in _healthcareArtifactExplorers)
				{
					TabPage tabPage = new TabPage(explorer.Name, explorer.Component);
					_tabComponentContainer.Pages.Add(tabPage);
				}
			}

			ApplicationComponent.LaunchAsWorkspace(this.Context.DesktopWindow, _tabComponentContainer, "Explorer", null);
			//ApplicationComponent.LaunchAsShelf(this.Context.DesktopWindow, _tabComponentContainer, "Explorer", ShelfDisplayHint.DockTop, null);
		}

	}
}
