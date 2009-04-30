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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A container for <see cref="IPresentationImage"/> objects.
	/// </summary>
	[Cloneable(true)]
	public class DisplaySet : IDisplaySet
	{
		#region DisplaySetMemento class

		private class DisplaySetMemento
		{
			public readonly IComparer<IPresentationImage> Comparer;

			public DisplaySetMemento(IDisplaySet displaySet)
			{
				Comparer = displaySet.PresentationImages.Comparer;
			}

			public bool ComparersEqual(IComparer<IPresentationImage> x, IComparer<IPresentationImage> y)
			{
				if (x == y)
					return true;

				if (x == null || y == null)
					return false;

				return x.GetType() == y.GetType();
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is DisplaySetMemento)
				{
					DisplaySetMemento other = (DisplaySetMemento) obj;
					return ComparersEqual(Comparer, other.Comparer);
				}

				return false;
			}
		}

		#endregion

		#region Private Fields

		[CloneIgnore]
		private IImageViewer _imageViewer;
		//[CloneCopyReference]
		[CloneIgnore]
		private ImageSet _parentImageSet;
		[CloneIgnore]
		private ImageBox _imageBox;
		[CloneIgnore]
		private bool _selected = false;
		[CloneIgnore]
		private bool _linked = false;
		private string _name;
		private string _description;
		private int _number;
		private string _uid;
		private event EventHandler _drawing;
		private PresentationImageCollection _presentationImages;
		
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
				{
					_presentationImages = new PresentationImageCollection();
					_presentationImages.ItemAdded += OnPresentationImageAdded;
				}

				return _presentationImages; 
			}
		}

		/// <summary>
		/// Gets a collection of linked <see cref="IPresentationImage"/> objects.
		/// </summary>
		public IEnumerable<IPresentationImage> LinkedPresentationImages
		{
			get
			{
				foreach (IPresentationImage image in PresentationImages)
				{
					if (image.Linked)
						yield return image;
				}
			}
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
		/// Gets the name of the display set.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		public int Number
		{
			get { return _number; }
			set { _number = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="DisplaySet"/> is visible.
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
				if (_linked == value)
					return;

				_linked = value;
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
				if (ParentImageSet != null)
					((ImageSet)ParentImageSet).RemoveCopy(this);

				DisposePresentationImages();
			}
		}

		private void DisposePresentationImages()
		{
			if (this.PresentationImages == null)
				return;

			foreach (PresentationImage image in this.PresentationImages)
				image.Dispose();

			_presentationImages.ItemAdded -= OnPresentationImageAdded;
			_presentationImages = null;
		}

		/// <summary>
		/// Creates a fresh copy of the <see cref="IDisplaySet"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="IDisplaySet"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		public IDisplaySet CreateFreshCopy()
		{
			DisplaySet displaySet = new DisplaySet(this.Name, this.Uid);
			displaySet.ParentImageSet = this.ParentImageSet;
			displaySet.Number = Number;
			displaySet.Description = Description;

			foreach (IPresentationImage image in this.PresentationImages)
				displaySet.PresentationImages.Add(image.CreateFreshCopy());

			displaySet.PresentationImages.Comparer = PresentationImages.Comparer;

			if (ParentImageSet != null)
				((ImageSet)ParentImageSet).AddCopy(displaySet);

			return displaySet;
		}

		/// <summary>
		/// Creates a deep copy of the <see cref="IDisplaySet"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="IDisplaySet"/>s may not return null from this method.
		/// </remarks>
		public IDisplaySet Clone()
		{
			try
			{
				DisplaySet clone = CloneBuilder.Clone(this) as DisplaySet;
				//if (ParentImageSet != null)
				//    ((ImageSet)ParentImageSet).AddCopy(clone);

				if (clone != null)
				{
					if (ImageViewer != null)
						ImageViewer.EventBroker.OnCloneCreated(new CloneCreatedEventArgs(this, clone));
				}

				return clone;
			}
			catch (Exception e)
			{
				throw new DisplaySetCloningException(this, e);
			}
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

		private void OnPresentationImageAdded(object sender, ListEventArgs<IPresentationImage> e)
		{
			PresentationImage image = (PresentationImage) e.Item;
			image.ParentDisplaySet = this;
			image.ImageViewer = this.ImageViewer;
		}

		/// <summary>
		/// Raises the <see cref="Drawing"/> event.
		/// </summary>
		protected virtual void OnDrawing()
		{
			EventsHelper.Fire(_drawing, this, EventArgs.Empty);
		}

		[CloneInitialize]
		private void Initialize(DisplaySet source, ICloningContext context)
		{
			context.CloneFields(source, this);

			foreach (IPresentationImage image in source.PresentationImages)
			{
				IPresentationImage clone = image.Clone();
				if (clone != null)
					PresentationImages.Add(clone);
			}

			//keep the sort order.
			PresentationImages.Comparer = source.PresentationImages.Comparer;
		}

		#region IMemorable Members

		/// <summary>
		/// Captures the state of an object.
		/// </summary>
		/// <remarks>
		/// The implementation of <see cref="IMemorable.CreateMemento"/> should return an
		/// object containing enough state information so that
		/// when <see cref="IMemorable.SetMemento"/> is called, the object can be restored
		/// to the original state.
		/// </remarks>
		public virtual object CreateMemento()
		{
			return new DisplaySetMemento(this);
		}

		/// <summary>
		/// Restores the state of an object.
		/// </summary>
		/// <param name="memento">The object that was
		/// originally created with <see cref="IMemorable.CreateMemento"/>.</param>
		/// <remarks>
		/// The implementation of <see cref="IMemorable.SetMemento"/> should return the 
		/// object to the original state captured by <see cref="IMemorable.CreateMemento"/>.
		/// </remarks>
		public virtual void SetMemento(object memento)
		{
			DisplaySetMemento displaySetMemento = memento as DisplaySetMemento;
			Platform.CheckForInvalidCast(displaySetMemento, "displaySetMemento", typeof(DisplaySetMemento).FullName);

			if (displaySetMemento.Comparer != null)
				this.PresentationImages.Sort(displaySetMemento.Comparer);
		}

		#endregion
	}
}
