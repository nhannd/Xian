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

	[MenuAction("launch", "global-menus/MenuFile/Home", "Launch")]
	[Tooltip("launch", "Go to home page")]
	[IconSet("launch", IconScheme.Colour, "Icons.GlobalHomeToolSmall.png", "Icons.GlobalHomeToolMedium.png", "Icons.GlobalHomeToolLarge.png")]
	[VisibleStateObserver("launch", "Visible", "VisibleChanged")]

	[MenuAction("toggleDowntimeMode", "global-menus/MenuTools/Downtime Mode", "ToggleDowntimeMode", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("toggleDowntimeMode", "DowntimeModeChecked", "DowntimeModeCheckedChanged")]
	[ActionPermission("toggleDowntimeMode", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Downtime.RecoveryOperations)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class GlobalHomeTool : WorklistPreviewHomeTool<FolderSystemExtensionPoint>
	{
        private static DesktopWindow _risWindow;

		public override void Initialize()
		{
			base.Initialize();

			// automatically launch home page on startup, only if current user is a Staff
			if (LoginSession.Current.IsStaff && HomePageSettings.Default.ShowHomepageOnStartUp
                && _risWindow == null)
			{
				Launch();

                // bug 3087: remember which window is the RIS window, so that we don't launch this
                // in the viewer window
                _risWindow = this.Context.DesktopWindow;
			}
		}

		public void ToggleDowntimeMode()
		{
			DowntimeRecovery.InDowntimeRecoveryMode = !DowntimeRecovery.InDowntimeRecoveryMode;
			this.Restart();
		}

		public event EventHandler DowntimeModeCheckedChanged
		{
			add { DowntimeRecovery.InDowntimeRecoveryModeChanged += value; }
			remove { DowntimeRecovery.InDowntimeRecoveryModeChanged -= value; }
		}

		public bool DowntimeModeChecked
		{
			get { return DowntimeRecovery.InDowntimeRecoveryMode; }
		}

		public override string Title
		{
			get { return DowntimeRecovery.InDowntimeRecoveryMode ? "Home (Downtime Recovery)" : "Home"; }
		}

		protected override bool IsUserClosableWorkspace
		{
			get { return !HomePageSettings.Default.PreventHomepageFromClosing; }
		}

		public bool Visible
		{
			get
            {
                // bug 3087: only visible in the RIS window
                return LoginSession.Current.IsStaff && 
                    (_risWindow == null || _risWindow == this.Context.DesktopWindow);
            }
		}

		public event EventHandler VisibleChanged
		{
			add { }
			remove { }
		}
	}
}
