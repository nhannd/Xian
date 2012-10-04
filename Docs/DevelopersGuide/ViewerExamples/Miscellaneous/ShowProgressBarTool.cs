#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

// ... (other using namespace statements here)

namespace MyPlugin.Miscellaneous
{
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/ShowProgressBarTool", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (DesktopToolExtensionPoint))]
	public class ShowProgressBarTool : Tool<IDesktopToolContext>
	{
		public void Apply()
		{
			BackgroundTask task = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
					{
						for (int i = 0; i < 255; i++)
						{
							// BackgroundTaskProgress takes a percentage as an integer between 0 and 100
							int percent = (int) ((i/255.0)*100);

							BackgroundTaskProgress progress = new BackgroundTaskProgress(percent, string.Format("This is a status message ({0} out of 255)", i));
							context.ReportProgress(progress);

							if (context.CancelRequested)
								break;

							// Do a chunk of a large, time consuming task
							Thread.Sleep(100);
						}
					}, true); // this true indicates that the task is cancellable; you can make it false if you want

			ProgressDialog.Show(task, // task is the object we created earlier
			                    base.Context.DesktopWindow, // this is the desktop window we want to show everything on
			                    true, // true if the progress bar should auto-close the task is done
			                    ProgressBarStyle.Blocks); // progress bar style: Blocks, Continuous, or Marquee
		}
	}
}