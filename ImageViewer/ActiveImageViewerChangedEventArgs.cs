using System;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for when <see cref="ImageViewerToolComponent.OnActiveImageViewerChanged"/>
	/// is called.
	/// </summary>
	public sealed class ActiveImageViewerChangedEventArgs : EventArgs
	{
		private IImageViewer _deactivatedImageViewer;
		private IImageViewer _activatedImageViewer;

		internal ActiveImageViewerChangedEventArgs(
			IImageViewer activatedImageViewer,
			IImageViewer deactivatedImageViewer)
		{
			_activatedImageViewer = activatedImageViewer;
			_deactivatedImageViewer = deactivatedImageViewer;
		}

		/// <summary>
		/// Gets the deactivated <see cref="IImageViewer"/>.
		/// </summary>
		public IImageViewer DeactivatedImageViewer
		{
			get { return _deactivatedImageViewer; }
		}

		/// <summary>
		/// Gets the activated <see cref="IImageViewer"/>.
		/// </summary>
		public IImageViewer ActivatedImageViewer
		{
			get { return _activatedImageViewer; }
		}
	}

}
