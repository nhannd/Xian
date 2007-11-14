#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A container for <see cref="IPresentationImage"/> objects.
	/// </summary>
	public class DisplaySet : IDisplaySet
	{
		#region Private Fields

		private PresentationImageCollection _presentationImages;
		private IImageViewer _imageViewer;
		private ImageSet _parentImageSet;
		private ImageBox _imageBox;
		private List<IPresentationImage> _linkedPresentationImages = new List<IPresentationImage>();

		private bool _selected = false;
		private bool _linked = false;
		private string _name;
		private string _uid;
		private event EventHandler _drawing;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySet"/>.
		/// </summary>
		public DisplaySet() : this("","")
		{
			
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySet"/> with
		/// the specified parameters.
		/// </summary>
		public DisplaySet(string name, string uid)
		{
			_name = name ?? "";
			_uid = uid ?? "";
			this.PresentationImages.ItemAdded += new EventHandler<PresentationImageEventArgs>(OnPresentationImageAdded);
			this.PresentationImages.ItemRemoved += new EventHandler<PresentationImageEventArgs>(OnPresentationImageRemoved);
		}

		#region Properties
		
		/// <summary>
		/// Gets the collection of <see cref="IPresentationImage"/> objects that belong
		/// to this <see cref="DisplaySet"/>.
		/// </summary>
		public PresentationImageCollection PresentationImages
		{
			get 
			{
				if (_presentationImages == null)
					_presentationImages = new PresentationImageCollection();

				return _presentationImages; 
			}
		}

		// TODO (Norman): Consider returning IEnumerable
		// TODO (Norman): Also consider how to use yield instead of a whole collection

		/// <summary>
		/// Gets a collection of linked <see cref="IPresentationImage"/> objects.
		/// </summary>
		public ReadOnlyCollection<IPresentationImage> LinkedPresentationImages
		{
			get { return _linkedPresentationImages.AsReadOnly(); }
		}

		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="DisplaySet"/> is not part of the 
		/// logical workspace yet.</value>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			internal set 
			{
				_imageViewer = value;

				if (_imageViewer != null)
				{
					foreach (PresentationImage image in this.PresentationImages)
						image.ImageViewer = value;
				}
			}
		}

		/// <summary>
		/// Gets the parent <see cref="ImageSet"/>
		/// </summary>
		/// <value>The parent <see cref="ImageSet"/> or <b>null</b> if the 
		/// <see cref="DisplaySet"/> has not been added to an 
		/// <see cref="ImageSet"/> yet.</value>
		public IImageSet ParentImageSet
		{
			get { return _parentImageSet; }
			internal set { _parentImageSet = value as ImageSet; }
		}

		/// <summary>
		/// Gets the <see cref="IImageBox"/> associated with this <see cref="DisplaySet"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageBox "/> or <b>null</b> if the
		/// <see cref="DisplaySet"/> is not currently visible.</value>
		public IImageBox ImageBox
		{
			get { return _imageBox; }
			internal set { _imageBox = value as ImageBox; }
		}

		/// <summary>
		/// Gets or sets the name of the display set.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="DisplaySet"/> is visible.
		/// </summary>
		public bool Visible
		{
			get { return this.ImageBox != null; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="DisplaySet"/> is selected.
		/// </summary>
		public bool Selected
		{
			get { return _selected; }
			internal set
			{
				if (_selected != value)
				{
					_selected = value;

					if (_selected)
					{
						if (this.ImageViewer == null)
							return;

						this.ImageViewer.EventBroker.OnDisplaySetSelected(
							new DisplaySetSelectedEventArgs(this));
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ClearCanvas.ImageViewer.ImageBox"/> is
		/// linked.
		/// </summary>
		/// <value><b>true</b> if linked; <b>false</b> otherwise.</value>
		/// <remarks>
		/// Multiple display sets may be linked, allowing tools that can operate on
		/// multiple display sets to operate on all linked display sets simultaneously.  
		/// Note that the concept of linkage is slightly different from selection:
		/// it is possible for an <see cref="DisplaySet"/> to be 1) selected but not linked
		/// 2) linked but not selected and 3) selected and linked.
		/// </remarks>
		public bool Linked
		{
			get { return _linked; }
			set
			{
				if (_linked != value)
				{
					_linked = value;

					if (_linked)
						_parentImageSet.LinkDisplaySet(this);
					else
						_parentImageSet.UnlinkDisplaySet(this);
				}
			}
		}

		/// <summary>
		/// Gets or sets unique identifier for this <see cref="IDisplaySet"/>.
		/// </summary>
		public string Uid
		{
			get { return _uid; }
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by this <see cref="DisplaySet"/>.
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
				DisposePresentationImages();
			}
		}

		private void DisposePresentationImages()
		{
			if (this.PresentationImages == null)
				return;

			foreach (PresentationImage image in this.PresentationImages)
				image.Dispose();

			_presentationImages.ItemAdded -= new EventHandler<PresentationImageEventArgs>(OnPresentationImageAdded);
			_presentationImages.ItemRemoved -= new EventHandler<PresentationImageEventArgs>(OnPresentationImageAdded);
			_presentationImages = null;
		}

		/// <summary>
		/// Creates a clone of the <see cref="DisplaySet"/>.
		/// </summary>
		/// <returns>The cloned <see cref="DisplaySet"/>.</returns>
		public IDisplaySet Clone()
		{
			DisplaySet displaySet = new DisplaySet(this.Name, this.Uid);
			displaySet.ParentImageSet = this.ParentImageSet;

			foreach (IPresentationImage image in this.PresentationImages)
				displaySet.PresentationImages.Add(image.Clone());

			return displaySet;
		}

		/// <summary>
		/// Returns the name of the display set.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.Name;
		}

		/// <summary>
		/// Fires when the <see cref="DisplaySet"/> is about the be drawn.
		/// </summary>
		public event EventHandler Drawing
		{
			add { _drawing += value; }
			remove { _drawing -= value; }
		}

		/// <summary>
		/// Draws the <see cref="DisplaySet"/>.
		/// </summary>
		/// <remarks>The <see cref="DisplaySet"/> will only be drawn
		/// if it is currently visible.</remarks>
		public void Draw()
		{
			if (this.Visible)
			{
				OnDrawing();
				foreach (PresentationImage image in this.PresentationImages)
					image.Draw();
			}
		}

		internal void LinkPresentationImage(PresentationImage image)
		{
			_linkedPresentationImages.Add(image);
		}

		internal void UnlinkPresentation(PresentationImage image)
		{
			_linkedPresentationImages.Remove(image);
		}

		private void OnPresentationImageAdded(object sender, PresentationImageEventArgs e)
		{
			PresentationImage image = e.PresentationImage as PresentationImage;
			image.ParentDisplaySet = this;
			image.ImageViewer = this.ImageViewer;

			if (e.PresentationImage.Linked)
				_linkedPresentationImages.Add(e.PresentationImage);
		}

		private void OnPresentationImageRemoved(object sender, PresentationImageEventArgs e)
		{
			if (e.PresentationImage.Linked)
				_linkedPresentationImages.Remove(e.PresentationImage);
		}

		/// <summary>
		/// Raises the <see cref="Drawing"/> event.
		/// </summary>
		protected virtual void OnDrawing()
		{
			EventsHelper.Fire(_drawing, this, EventArgs.Empty);
		}
	}
}
