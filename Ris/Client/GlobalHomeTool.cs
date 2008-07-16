using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using System.Threading;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class FolderSystemExtensionPoint : ExtensionPoint<IFolderSystem>
	{
	}

	[MenuAction("launch", "global-menus/Go/Home", "Launch")]
	[Tooltip("launch", "Go to home page")]
	[IconSet("launch", IconScheme.Colour, "Icons.GlobalHomeToolSmall.png", "Icons.GlobalHomeToolMedium.png", "Icons.GlobalHomeToolLarge.png")]
	[VisibleStateObserver("launch", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class GlobalHomeTool : WorklistPreviewHomeTool<FolderSystemExtensionPoint>
	{
		public override void Initialize()
		{
			base.Initialize();

			// automatically launch home page on startup, only if current user is a Staff
			if (LoginSession.Current.IsStaff)
			{
				Launch();
			}
		}

		public override string Title
		{
			get { return "Home"; }
		}

		protected override bool IsUserClosableWorkspace
		{
			get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Development.RestartHomepage); }
		}

		public bool Visible
		{
			get { return LoginSession.Current.IsStaff && this.HasFolderSystems && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Development.RestartHomepage); }
		}

		public event EventHandler VisibleChanged
		{
			add { }
			remove { }
		}
	}
}
