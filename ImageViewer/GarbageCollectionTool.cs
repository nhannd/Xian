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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using Timer=System.Threading.Timer;

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


	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	internal class GarbageCollectionTool : Tool<IDesktopToolContext>
	{
		public GarbageCollectionTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.DesktopWindow.Workspaces.ItemClosed += OnWorkspaceClosed;
		}

		protected override void Dispose(bool disposing)
		{
			this.Context.DesktopWindow.Workspaces.ItemClosed -= OnWorkspaceClosed;
			base.Dispose(disposing);
		}

		void OnWorkspaceClosed(object sender, ItemEventArgs<Workspace> e)
		{
			// When a workspace has been closed, we want the GC to run
			// a few times to release what is often a significant amount of managed memory.

			WaitCallback del = delegate
			                   	{
									for (int i = 0; i < 5; ++i)
									{
										Thread.Sleep(500);
										GC.Collect();
									}
			                   	};

			ThreadPool.QueueUserWorkItem(del);
		}
	}
}
