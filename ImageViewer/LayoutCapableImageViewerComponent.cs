using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public abstract class LayoutCapableImageViewerComponent : ImageViewerComponent
	{
		private ILayoutManager _layoutManager;

		protected LayoutCapableImageViewerComponent()
		{
			CreateLayoutManager();
			this.LayoutManager.SetImageViewer(this);
		}

		protected LayoutCapableImageViewerComponent(ILayoutManager layoutManager)
		{
			Platform.CheckForNullReference(layoutManager, "layoutManager");
			_layoutManager = layoutManager;
		}

		protected ILayoutManager LayoutManager
		{
			get { return _layoutManager; }
			set { _layoutManager = value; }
		}

		#region Disposal

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				if (_layoutManager != null)
					_layoutManager.Dispose();
			}
		}

		#endregion

		protected abstract void CreateLayoutManager();

	}
}
