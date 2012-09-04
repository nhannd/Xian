#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// </summary>
	[MenuAction("show", "global-menus/MenuTools/MenuActivityMonitor", "Show")]
	[Tooltip("show", "TooltipActivityMonitor")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class ActivityMonitorTool : Tool<IDesktopToolContext>
	{
		private static bool _watchersInstalled;

		private ActivityMonitorFailureWatcher _failureWatcher;
		private LocalServerWatcher _localServerWatcher;
		private bool _maxDiskSpaceExceeded;

		public override void Initialize()
		{
			base.Initialize();

			// install watchers only once, in the first desktop window (the main window)
			if (!_watchersInstalled)
			{
				_watchersInstalled = true;
				_failureWatcher = new ActivityMonitorFailureWatcher(this.Context.DesktopWindow, Show);
				_failureWatcher.Initialize();

				_localServerWatcher = LocalServerWatcher.Instance;
				_localServerWatcher.DiskSpaceUsageChanged += LocalServerWatcherOnDiskSpaceUsageChanged;
				CheckDiskspaceUsageExceeded();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_failureWatcher != null)
				{
					_failureWatcher.Dispose();
					_failureWatcher = null;
				}
				if (_localServerWatcher != null)
				{
					_localServerWatcher.DiskSpaceUsageChanged -= LocalServerWatcherOnDiskSpaceUsageChanged;
					_localServerWatcher = null;
				}
			}
			base.Dispose(disposing);
		}

		public override IActionSet Actions
		{
			get
			{
				if (!WorkItemActivityMonitor.IsSupported)
					return new ActionSet();

				return base.Actions;
			}
		}

		public void Show()
		{
			ActivityMonitorManager.Show(Context.DesktopWindow);
		}

		private void LocalServerWatcherOnDiskSpaceUsageChanged(object sender, EventArgs eventArgs)
		{
			CheckDiskspaceUsageExceeded();
		}

		private void CheckDiskspaceUsageExceeded()
		{
			if (_localServerWatcher.IsMaximumDiskspaceUsageExceeded == _maxDiskSpaceExceeded)
				return;

			_maxDiskSpaceExceeded = _localServerWatcher.IsMaximumDiskspaceUsageExceeded;
			if (_maxDiskSpaceExceeded)
			{
				this.Context.DesktopWindow.ShowAlert(AlertLevel.Warning,
													 SR.WarningMaximumDiskUsageExceeded,
													 SR.LinkOpenStorageConfiguration,
													 delegate
													 {
														 ActivityMonitorQuickLink.LocalStorageConfiguration.Invoke(
															 this.Context.DesktopWindow);
													 },
													 true);
			}
		}
	}
}
