#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Common.WorkItem;
using N = System.Collections.Generic.Dictionary<bool, ClearCanvas.ImageViewer.StudyManagement.ProgressBarColor>;
using M = System.Collections.Generic.Dictionary<ClearCanvas.ImageViewer.Common.WorkItem.WorkItemStatusEnum, System.Collections.Generic.Dictionary<bool, ClearCanvas.ImageViewer.StudyManagement.ProgressBarColor>>;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal class ActivityMonitorProgressBar
	{
		private static readonly M _stateMap = new M
		{
															// has failures					// no failures
			{ WorkItemStatusEnum.Pending,			new N {{true, ProgressBarColor.Yellow}, {false, ProgressBarColor.Green}} },
			{ WorkItemStatusEnum.InProgress,		new N {{true, ProgressBarColor.Yellow}, {false, ProgressBarColor.Green}} },
			{ WorkItemStatusEnum.Idle,				new N {{true, ProgressBarColor.Yellow}, {false, ProgressBarColor.Green}} },
			{ WorkItemStatusEnum.Canceling,			new N {{true, ProgressBarColor.Yellow}, {false, ProgressBarColor.Yellow}} },
			{ WorkItemStatusEnum.Canceled,			new N {{true, ProgressBarColor.Yellow},	{false, ProgressBarColor.Yellow}} },
			{ WorkItemStatusEnum.Complete,			new N {{true, ProgressBarColor.Red},	{false, ProgressBarColor.Green}} },
			{ WorkItemStatusEnum.Failed,			new N {{true, ProgressBarColor.Red},	{false, ProgressBarColor.Red}} },

			// these states should not appear in the work item list at all, but if they were to for some reason, we'll make them yellow
			{ WorkItemStatusEnum.DeleteInProgress,	new N {{true, ProgressBarColor.Yellow},	{false, ProgressBarColor.Green}} },
			{ WorkItemStatusEnum.Deleted,			new N {{true, ProgressBarColor.Yellow},	{false, ProgressBarColor.Green}} },
		};

		public static ProgressBarColor GetColor(WorkItemProgress progress, WorkItemStatusEnum status)
		{
			var hasFailures = progress.PercentFailed > 0;
			return _stateMap[status][hasFailures];
		}
	}
}
