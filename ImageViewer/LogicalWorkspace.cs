using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer 
{
	/// <summary>
	/// A container for <see cref="IImageSet"/> objects.
	/// </summary>
	public class LogicalWorkspace : ILogicalWorkspace
	{
		private ImageSetCollection _imageSets = new ImageSetCollection();
        private IImageViewer _imageViewer;

        internal LogicalWorkspace(IImageViewer imageViewer)
		{
            _imageViewer = imageViewer;
			_imageSets.ItemAdded += new EventHandler<ImageSetEventArgs>(OnImageSetAdded);
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
				Platform.Log(e);
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

			_imageSets.ItemAdded -= new EventHandler<ImageSetEventArgs>(OnImageSetAdded);
			_imageSets = null;
		}

		#endregion

		/// <summary>
		/// Draws the <see cref="LogicalWorkspace"/>.
		/// </summary>
		public void Draw()
		{
			foreach (ImageSet imageSet in this.ImageSets)
				imageSet.Draw();
		}

		private void OnImageSetAdded(object sender, ImageSetEventArgs e)
		{
			ImageSet imageSet = e.ImageSet as ImageSet;

			imageSet.ParentLogicalWorkspace = this;
			imageSet.ImageViewer = this.ImageViewer;
		}
	}
}
