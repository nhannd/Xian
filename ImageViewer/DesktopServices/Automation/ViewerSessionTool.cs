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
	public class ViewerSessionTool : ImageViewerTool
	{
		private static readonly object _syncLock = new object();
		private static readonly List<ViewerSessionTool> _sessions;

		private readonly Guid _sessionId;
		private volatile IImageViewer _viewer;

		static ViewerSessionTool()
		{
			_sessions = new List<ViewerSessionTool>();
		}

		public ViewerSessionTool()
		{
			_sessionId = Guid.NewGuid();
		}

		public override void Initialize()
		{
			base.Initialize();

			_viewer = base.ImageViewer;
			lock(_syncLock)
			{
				_sessions.Add(this);
			}
		}

		protected override void Dispose(bool disposing)
		{
			lock(_syncLock)
			{
				_sessions.Remove(this);
			}

			base.Dispose(disposing);
		}

		internal static Guid? GetSessionId(IImageViewer viewer)
		{
			lock (_syncLock)
			{
				ViewerSessionTool foundTool =
					CollectionUtils.SelectFirst(_sessions, delegate(ViewerSessionTool tool) { return tool._viewer == viewer; });

				if (foundTool != null)
					return foundTool._sessionId;

				return null;
			}
		}

		internal static IImageViewer GetViewer(Guid sessionId)
		{
			lock (_syncLock)
			{
				ViewerSessionTool foundTool =
					CollectionUtils.SelectFirst(_sessions, delegate(ViewerSessionTool tool) { return tool._sessionId == sessionId; });

				if (foundTool != null)
					return foundTool._viewer;

				return null;
			}
		}
	}
}
