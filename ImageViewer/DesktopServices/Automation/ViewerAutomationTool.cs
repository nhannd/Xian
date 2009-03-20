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
