#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
