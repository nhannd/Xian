using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer 
{
	/// <summary>
	/// A container for <see cref="IImageSet"/> objects.
	/// </summary>
	public class LogicalWorkspace : ILogicalWorkspace
	{
		private ImageSetCollection _imageSets = new ImageSetCollection();
        private IImageViewer _imageViewer;
		private event EventHandler _drawing;

        internal LogicalWorkspace(IImageViewer imageViewer)
		{
            _imageViewer = imageViewer;
			_imageSets.ItemAdded += OnImageSetAdded;
		}

		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
		}

		/// <summary>
		/// Gets a collection of <see cref="IImageSet"/> objects that belong to
		/// this <see cref="ILogicalWorkspace"/>
		/// </summary>
		public ImageSetCollection ImageSets
		{
			get { return _imageSets; }
		}

		#region Disposal

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeImageSets();
			}
		}

		private void DisposeImageSets()
		{
			if (this.ImageSets == null)
				return;

			foreach (ImageSet imageSet in this.ImageSets)
				imageSet.Dispose();

			_imageSets.ItemAdded -= OnImageSetAdded;
			_imageSets = null;
		}

		#endregion

		/// <summary>
		/// Fires just before the <see cref="LogicalWorkspace"/> is actually drawn/rendered.
		/// </summary>
		public event EventHandler Drawing
		{
			add { _drawing += value; }
			remove { _drawing -= value; }
		}

		/// <summary>
		/// Draws the <see cref="LogicalWorkspace"/>.
		/// </summary>
		public void Draw()
		{
			OnDrawing();

			foreach (ImageSet imageSet in this.ImageSets)
				imageSet.Draw();
		}

		private void OnImageSetAdded(object sender, ImageSetEventArgs e)
		{
			ImageSet imageSet = e.ImageSet as ImageSet;

			imageSet.ParentLogicalWorkspace = this;
			imageSet.ImageViewer = this.ImageViewer;
		}

		private void OnDrawing()
		{
			EventsHelper.Fire(_drawing, this, EventArgs.Empty);
		}
	}
}
