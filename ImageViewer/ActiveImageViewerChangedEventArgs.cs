using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	public sealed class ActiveImageViewerChangedEventArgs : EventArgs
	{
		private IImageViewer _deactivatedImageViewer;
		private IImageViewer _activatedImageViewer;

		public ActiveImageViewerChangedEventArgs(
			IImageViewer activatedImageViewer,
			IImageViewer deactivatedImageViewer)
		{
			_activatedImageViewer = activatedImageViewer;
			_deactivatedImageViewer = deactivatedImageViewer;
		}

		public IImageViewer DeactivatedImageViewer
		{
			get { return _deactivatedImageViewer; }
		}

		public IImageViewer ActivatedImageViewer
		{
			get { return _activatedImageViewer; }
		}
	}

}
