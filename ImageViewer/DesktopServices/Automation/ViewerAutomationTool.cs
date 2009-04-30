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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.DesktopServices.Automation
{
	/// <summary>
	/// For internal use only.
	/// </summary>
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ViewerAutomationTool : ImageViewerTool
	{
		private static readonly object _syncLock = new object();
		private static readonly List<ViewerAutomationTool> _tools;

		private readonly Guid _viewerId;
		private volatile IImageViewer _viewer;

		static ViewerAutomationTool()
		{
			_tools = new List<ViewerAutomationTool>();
		}

		public ViewerAutomationTool()
		{
			_viewerId = Guid.NewGuid();
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.DesktopWindow.Workspaces.ItemActivationChanged += OnWorkspaceActivationChanged;

			_viewer = base.ImageViewer;
			lock(_syncLock)
			{
				_tools.Add(this);
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.Context.DesktopWindow.Workspaces.ItemActivationChanged -= OnWorkspaceActivationChanged;

			lock(_syncLock)
			{
				_tools.Remove(this);
			}

			base.Dispose(disposing);
		}

		private void OnWorkspaceActivationChanged(object sender, ItemEventArgs<Workspace> e)
		{
			if (!e.Item.Active)
				return;

			IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(e.Item);
			if (viewer == base.ImageViewer)
			{
				//make the list of tools reflect the activation order, most recent first
				lock(_syncLock)
				{
					_tools.Remove(this);
					_tools.Insert(0, this);
				}
			}
		}

		internal static List<Guid> GetViewerIds()
		{
			lock(_syncLock)
			{
				return CollectionUtils.Map<ViewerAutomationTool, Guid>(_tools, 
					delegate(ViewerAutomationTool tool) { return tool._viewerId; });
			}
		}

		internal static Guid? GetViewerId(IImageViewer viewer)
		{
			lock (_syncLock)
			{
				ViewerAutomationTool foundTool =
					CollectionUtils.SelectFirst(_tools, delegate(ViewerAutomationTool tool) { return tool._viewer == viewer; });

				if (foundTool != null)
					return foundTool._viewerId;

				return null;
			}
		}

		internal static IImageViewer GetViewer(Guid viewerId)
		{
			lock (_syncLock)
			{
				ViewerAutomationTool foundTool =
					CollectionUtils.SelectFirst(_tools, delegate(ViewerAutomationTool tool) { return tool._viewerId == viewerId; });

				if (foundTool != null)
					return foundTool._viewer;

				return null;
			}
		}
	}
}
