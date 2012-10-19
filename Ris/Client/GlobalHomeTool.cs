#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class FolderSystemExtensionPoint : ExtensionPoint<IFolderSystem>
	{
	}

	[MenuAction("launch", "global-menus/MenuFile/MenuHome", "Launch")]
	[Tooltip("launch", "Go to home page")]
	[IconSet("launch", IconScheme.Colour, "Icons.GlobalHomeToolSmall.png", "Icons.GlobalHomeToolMedium.png", "Icons.GlobalHomeToolLarge.png")]
	[VisibleStateObserver("launch", "Visible", "VisibleChanged")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.HomePage.View)]

	[MenuAction("toggleDowntimeMode", "global-menus/MenuTools/MenuDowntimeRecoveryMode", "ToggleDowntimeMode", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("toggleDowntimeMode", "DowntimeModeChecked", "DowntimeModeCheckedChanged")]
	[ActionPermission("toggleDowntimeMode", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.HomePage.View,
		ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Downtime.RecoveryOperations)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class GlobalHomeTool : WorklistPreviewHomeTool<FolderSystemExtensionPoint>
	{
		private static DesktopWindow _risWindow;

		public override void Initialize()
		{
			base.Initialize();

			// automatically launch home page on startup, only if current user is a Staff
			if (LoginSession.Current != null && LoginSession.Current.IsStaff 
				&& Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.HomePage.View)
				&& HomePageSettings.Default.ShowHomepageOnStartUp
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
			get { return DowntimeRecovery.InDowntimeRecoveryMode ? string.Format("{0} ({1})", SR.TitleHome, SR.TitleDowntimeRecovery) : SR.TitleHome; }
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
				return LoginSession.Current != null && LoginSession.Current.IsStaff &&
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
