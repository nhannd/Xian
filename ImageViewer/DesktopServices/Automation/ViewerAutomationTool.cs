using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.BaseTools;

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

			_viewer = base.ImageViewer;
			lock(_syncLock)
			{
				_tools.Add(this);
			}
		}

		protected override void Dispose(bool disposing)
		{
			lock(_syncLock)
			{
				_tools.Remove(this);
			}

			base.Dispose(disposing);
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
