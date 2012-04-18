#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// </summary>
	[MenuAction("show", "global-menus/MenuTools/MenuLocalServer/MenuActivityMonitor", "Show")]
	[Tooltip("show", "TooltipActivityMonitor")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class ActivityMonitorTool : Tool<IDesktopToolContext>
	{
		private static bool _failureMonitorInstalled;

		private IWorkItemActivityMonitor _activityMonitor;
		private readonly HashSet<long> _failedWorkItems = new HashSet<long>();

		public override void Initialize()
		{
			base.Initialize();

			// install failure monitor only once, in the first desktop window (the main window)
			if (!_failureMonitorInstalled)
			{
				// alert user if there are any failed work items remaining in the work queue
				AlertTotalFailedWorkItems();

				_activityMonitor = WorkItemActivityMonitor.Create(true);
				_activityMonitor.WorkItemChanged += WorkItemChanged;
				_failureMonitorInstalled = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_activityMonitor != null)
				{
					_activityMonitor.WorkItemChanged -= WorkItemChanged;
					_activityMonitor.Dispose();
					_activityMonitor = null;
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

		private void WorkItemChanged(object sender, WorkItemChangedEventArgs e)
		{
			var item = e.ItemData;

			// check for a new failure, and raise an alert if necessary
			if (!_failedWorkItems.Contains(item.Identifier) && item.Status == WorkItemStatusEnum.Failed)
			{
				_failedWorkItems.Add(item.Identifier);

				var message = string.Format(SR.MessageWorkItemFailed, item.Request.ActivityType.GetDescription());
				this.Context.DesktopWindow.ShowAlert(AlertLevel.Error, message, SR.LinkOpenActivityMonitor, window => Show());
			}

			// if a previously failed item is re-tried, remove it from the set of failed items
			if (_failedWorkItems.Contains(item.Identifier) && item.Status != WorkItemStatusEnum.Failed)
			{
				_failedWorkItems.Remove(item.Identifier);
			}
		}

		private void AlertTotalFailedWorkItems()
		{
			Platform.GetService<IWorkItemService>(
				service =>
				{
					var failedItems = service.Query(new WorkItemQueryRequest { Status = WorkItemStatusEnum.Failed }).Items;
					if (!failedItems.Any())
						return;

					foreach (var item in failedItems)
					{
						_failedWorkItems.Add(item.Identifier);
					}

					var message = string.Format(SR.MessageTotalFailedWorkItems, _failedWorkItems.Count);
					this.Context.DesktopWindow.ShowAlert(AlertLevel.Error, message, SR.LinkOpenActivityMonitor, window => Show());
				});
		}
	}
}
