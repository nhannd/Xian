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

		/// <summary>
		/// Releases all resources used by this <see cref="LogicalWorkspace"/>.
		/// </summary>
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

		private void OnImageSetAdded(object sender, ListEventArgs<IImageSet> e)
		{
			ImageSet imageSet = (ImageSet)e.Item;

			imageSet.ParentLogicalWorkspace = this;
			imageSet.ImageViewer = this.ImageViewer;
		}

		private void OnDrawing()
		{
			EventsHelper.Fire(_drawing, this, EventArgs.Empty);
		}
	}
}
