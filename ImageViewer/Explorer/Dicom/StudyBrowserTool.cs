#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public abstract class StudyBrowserTool : Tool<IStudyBrowserToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChangedEvent;

		public override void Initialize()
		{
			base.Initialize();
			Context.SelectedStudyChanged += new EventHandler(OnSelectedStudyChanged);
			Context.SelectedServerChanged += new EventHandler(OnSelectedServerChanged);
		}

		protected virtual void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			if (Context.SelectedStudy != null)
				Enabled = true;
			else
				Enabled = false;
		}

		protected abstract void OnSelectedServerChanged(object sender, EventArgs e);

		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChangedEvent += value; }
			remove { _enabledChangedEvent -= value; }
		}

		protected int ProcessItemsAsync<T>(IEnumerable<T> studies, Action<T> processAction, bool cancelable)
		{
			var itemsToProcess = studies.ToList();	// make a copy
			var processedCount = 0;
			var task = new BackgroundTask(delegate(IBackgroundTaskContext context)
											{
												try
												{
													foreach (var item in itemsToProcess)
													{
														if (context.CancelRequested)
														{
															context.Cancel();
															return;
														}

														processAction(item);
														processedCount++;
														var msg = string.Format(SR.MessageProcessedItemsProgress, processedCount, itemsToProcess.Count);
														context.ReportProgress(new BackgroundTaskProgress(processedCount - 1, itemsToProcess.Count, msg));
													}
													context.Complete();
												}
												catch (Exception e)
												{
													context.Error(e);
												}
											}, cancelable);

			//note: any exceptions occurring on the background task will be re-thrown from this call
			ProgressDialog.Show(task, this.Context.DesktopWindow, true);

			return processedCount;
		}

	}
}
