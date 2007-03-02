using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// An <see cref="ImageViewerComponent"/> with image layout capability.
	/// </summary>
	public abstract class LayoutCapableImageViewerComponent : ImageViewerComponent
	{
		private ILayoutManager _layoutManager;

		/// <summary>
		/// Initializes a new instance of <see cref="LayoutCapableImageViewerComponent"/>.
		/// </summary>
		protected LayoutCapableImageViewerComponent()
		{
			_layoutManager = CreateLayoutManager();
			this.LayoutManager.SetImageViewer(this);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="LayoutCapableImageViewerComponent"/>
		/// with the specified <see cref="ILayoutManager"/>.
		/// </summary>
		protected LayoutCapableImageViewerComponent(ILayoutManager layoutManager)
		{
			Platform.CheckForNullReference(layoutManager, "layoutManager");
			_layoutManager = layoutManager;
		}

		/// <summary>
		/// Gets the <see cref="ILayoutManager"/>
		/// </summary>
		protected ILayoutManager LayoutManager
		{
			get { return _layoutManager; }
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

		/// <summary>
		/// Creates an <see cref="ILayoutManager"/>.
		/// </summary>
		protected abstract ILayoutManager CreateLayoutManager();
	}
}
