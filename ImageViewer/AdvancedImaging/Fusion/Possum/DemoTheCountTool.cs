#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InteractiveGraphics;

#if DEBUG

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Possum
{
	[MenuAction("show", "global-menus/MenuDebug/Possum/Continuous", "TheCount")]
	[MenuAction("show2", "global-menus/MenuDebug/Possum/Marquee", "TheCount2")]
	[MenuAction("show3", "global-menus/MenuDebug/Possum/Blocks", "TheCount3")]
	[MenuAction("unshow", "global-menus/MenuDebug/Possum/Reset", "UnTheCount")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class TheCountTool : ImageViewerTool
	{
		private BackgroundTask _task;

		public void TheCount()
		{
			if (base.SelectedOverlayGraphicsProvider != null && _task == null)
			{
				_task = new BackgroundTask(ATasket, true);
				_task.Terminated += _task_Terminated;
				ProgressGraphic.Show(_task, base.SelectedOverlayGraphicsProvider.OverlayGraphics, true, ProgressBarGraphicStyle.Continuous);
			}
		}

		public void TheCount2()
		{
			if (base.SelectedOverlayGraphicsProvider != null && _task == null)
			{
				_task = new BackgroundTask(ATasket, true);
				_task.Terminated += _task_Terminated;
				ProgressGraphic.Show(_task, base.SelectedOverlayGraphicsProvider.OverlayGraphics, true, ProgressBarGraphicStyle.Marquee);
			}
		}

		public void TheCount3()
		{
			if (base.SelectedOverlayGraphicsProvider != null && _task == null)
			{
				_task = new BackgroundTask(ATasket, true);
				_task.Terminated += _task_Terminated;
				ProgressGraphic.Show(_task, base.SelectedOverlayGraphicsProvider.OverlayGraphics, true, ProgressBarGraphicStyle.Blocks);
			}
		}

		private void _task_Terminated(object sender, BackgroundTaskTerminatedEventArgs e)
		{
			if (_task != null)
			{
				_task.Terminated -= _task_Terminated;
				_task.Dispose();
				_task = null;
			}
		}

		public void UnTheCount()
		{
			if (_task != null)
			{
				try
				{
					_task.RequestCancel();
				}
				catch (InvalidOperationException) {}
			}
		}

		private static void ATasket(IBackgroundTaskContext context)
		{
			const int nMax = 600;
			for (int n = 0; n < nMax; n++)
			{
				if (context.CancelRequested)
				{
					try
					{
						return;
					}
					finally
					{
						context.Cancel();
					}
				}
				context.ReportProgress(new BackgroundTaskProgress(n, nMax, string.Format("Working Message {0:p0}", 1f*n/nMax)));
				Thread.Sleep(100);
			}
			context.ReportProgress(new BackgroundTaskProgress(nMax, nMax, "All done!"));
		}

		private static void ATisket(IBackgroundTaskContext context) {}
	}
}

#endif