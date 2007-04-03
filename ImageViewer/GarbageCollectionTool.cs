using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.Threading;

namespace ClearCanvas.ImageViewer
{
	// This tool is basically a cheap hack to make sure that the garbage collector
	// runs a few times after a workspace is closed.  Performing a single GC 
	// when listening for a workspace removed event doesn't work since DotNetMagic
	// is still holding on to certain references at that point.  We have to wait
	// until the workspace is completely closed and all UI resources released
	// before we do the GC.  The easiest way to do that without hooking into 
	// the UI code itself is to get a timer to perform a GC a few times after
	// the workspace has been closed.

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class GarbageCollectionTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		private System.Threading.Timer _timer;
		private int _gcCycles;

		public GarbageCollectionTool()
		{
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			this.Context.DesktopWindow.WorkspaceManager.Workspaces.ItemRemoved += new EventHandler<WorkspaceEventArgs>(OnWorkspaceRemoved);

			TimerCallback timerCallback = new TimerCallback(OnTimer);

			AutoResetEvent autoReset = new AutoResetEvent(false);
			_timer = new System.Threading.Timer(timerCallback, autoReset, 0, 2000);
		}

		void OnWorkspaceRemoved(object sender, WorkspaceEventArgs e)
		{
			// When a workspace has been closed, we want the GC to run
			// a few times triggered by a timer to release what is often
			// a significant amount of managed memory.
			_gcCycles = 5;
		}

		void OnTimer(object autoReset)
		{
			// We don't care if _gcCycles is thread-safe, since the actual
			// value is arbitrary and it doesn't matter if it's off once in a while.
			// As long as it eventually hits zero, thus stopping garbage collection,
			// we're good.
			if (_gcCycles > 0)
			{
				_gcCycles--;
				GC.Collect();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_timer != null)
				_timer.Dispose();

			base.Dispose(disposing);
		}
	}
}
