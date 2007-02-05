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

    [MenuAction("show", "global-menus/MenuFile/MenuFileSearch", KeyStroke = XKeys.Control | XKeys.S)]
	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarToolsStandardExplorer")]
	[ClickHandler("show", "Show")]
	[IconSet("show", IconScheme.Colour, "", "Icons.DashboardMedium.png", "Icons.DashboardLarge.png")]
	[GroupHint("show", "Application.Browsing.Explorer")]

    [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ExplorerTool : Tool<IDesktopToolContext>
	{
		TabComponentContainer _tabComponentContainer;
		List<IHealthcareArtifactExplorer> _healthcareArtifactExplorers;
		IWorkspace _workspace;

		public ExplorerTool()
		{
		}

		public override void Initialize()
		{
			Show();
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

				_workspace = ApplicationComponent.LaunchAsWorkspace(
					this.Context.DesktopWindow,
					_tabComponentContainer,
					SR.TitleExplorer,
					delegate
					{
						_workspace = null;
						_tabComponentContainer = null;
						_healthcareArtifactExplorers.Clear();
						_healthcareArtifactExplorers = null;
					});

			}
			else
			{
				_workspace.Activate();
			}
		}

	}
}
