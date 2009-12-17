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

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A container for <see cref="IDisplaySet"/> objects.
	/// </summary>
	public class ImageSet : IImageSet
	{
		#region Private fields

		private DisplaySetCollection _displaySets = new DisplaySetCollection();
		private LogicalWorkspace _parentLogicalWorkspace;
		private IImageViewer _imageViewer;
		private ImageSetDescriptor _descriptor;
		private event EventHandler _drawing;
		private List<IDisplaySet> _displaySetCopies;

		#endregion
		
		/// <summary>
		/// Initializes a new instance of <see cref="ImageSet"/>.
		/// </summary>
		public ImageSet()
			: this(new BasicImageSetDescriptor())
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageSet"/>.
		/// </summary>
		public ImageSet(ImageSetDescriptor descriptor)
		{
			_displaySetCopies = new List<IDisplaySet>();

			_displaySets.ItemAdded += OnDisplaySetAdded;
			_displaySets.ItemChanging += OnDisplaySetChanging;
			_displaySets.ItemRemoved += OnDisplaySetRemoved;
			_displaySets.ItemChanged += OnDisplaySetChanged;

			Descriptor = descriptor;
		}

		internal void AddCopy(IDisplaySet copy)
		{
			_displaySetCopies.Add(copy);
		}

		internal void RemoveCopy(IDisplaySet copy)
		{
			if (_displaySetCopies != null)
				_displaySetCopies.Remove(copy);
		}

		internal IEnumerable<IDisplaySet> GetCopies()
		{
			foreach (IDisplaySet copy in _displaySetCopies)
				yield return copy;
		}

		#region IImageSet Members

		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="ImageSet"/> is not part of the 
		/// logical workspace yet.</value>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			internal set
			{
				_imageViewer = value;

				if (_imageViewer != null)
				{
					foreach (DisplaySet displaySet in this.DisplaySets)
						displaySet.ImageViewer = value;

					foreach (DisplaySet copy in _displaySetCopies)
						copy.ImageViewer = value;
				}
			}
		}

		/// <summary>
		/// Gets the parent <see cref="LogicalWorkspace"/>
		/// </summary>
		/// <value>The parent <see cref="ILogicalWorkspace"/> or <b>null</b> if the 
		/// <see cref="ImageSet"/> has not been added to an 
		/// <see cref="ILogicalWorkspace"/> yet.</value>
		public ILogicalWorkspace ParentLogicalWorkspace
		{
			get { return _parentLogicalWorkspace; }
			internal set { _parentLogicalWorkspace = value as LogicalWorkspace; }
		}

		/// <summary>
		/// Gets the collection of <see cref="IDisplaySet"/> objects that belong
		/// to this <see cref="ImageSet"/>.
		/// </summary>
		public DisplaySetCollection DisplaySets
		{
			get { return _displaySets; }
		}

		/// <summary>
		/// Gets a collection of linked <see cref="IDisplaySet"/> objects.
		/// </summary>
		public IEnumerable<IDisplaySet> LinkedDisplaySets
		{
			get
			{
				foreach (IDisplaySet displaySet in DisplaySets)
				{
					if (displaySet.Linked)
						yield return displaySet;
				}

				foreach (DisplaySet copy in _displaySetCopies)
				{
					if (copy.Linked)
						yield return copy;
				}
			}
		}

		IImageSetDescriptor IImageSet.Descriptor
		{
			get { return _descriptor; }
		}

		/// <summary>
		/// Gets the <see cref="IImageSetDescriptor"/> describing this <see cref="IImageSet"/>.
		/// </summary>
		public ImageSetDescriptor Descriptor
		{
			get { return _descriptor; }
			set
			{
				Platform.CheckForNullReference(value, "Descriptor");

				if (_descriptor != null)
					_descriptor.ImageSet = null;

				_descriptor = value;
				_descriptor.ImageSet = this;
			}
		}

		/// <summary>
		/// Gets or sets the name of the image set.
		/// </summary>
		public string Name
		{
			get { return _descriptor.Name; }
			set { _descriptor.Name = value; }
		}

		/// <summary>
		/// Gets or sets the patient information associated with the image set.
		/// </summary>
		public string PatientInfo
		{
			get { return _descriptor.PatientInfo; }
			set { _descriptor.PatientInfo = value; }
		}


		/// <summary>
		/// Gets or sets unique identifier for this <see cref="IImageSet"/>.
		/// </summary>
		public string Uid
		{
			get { return _descriptor.Uid; }
			set { _descriptor.Uid = value; }
		}

		/// <summary>
		/// Fires just before the <see cref="ImageSet"/> is actually drawn/rendered.
		/// </summary>
		public event EventHandler Drawing
		{
			add { _drawing += value; }
			remove { _drawing -= value; }
		}

		/// <summary>
		/// Draws the <see cref="ImageSet"/>.
		/// </summary>
		public void Draw()
		{
			OnDrawing();
			foreach (DisplaySet displaySet in this.DisplaySets)
				displaySet.Draw();

			foreach (DisplaySet copy in _displaySetCopies)
				copy.Draw();
		}

		#endregion

		/// <summary>
		/// Returns the name of the image set.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _descriptor.ToString();
		}

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by this <see cref="ImageSet"/>.
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
				DisposeDisplaySets();
			}
		}

		private void DisposeDisplaySets()
		{
			if (this.DisplaySets == null)
				return;

			List<IDisplaySet> displaySetCopies = _displaySetCopies;
			_displaySetCopies = null;

			foreach (DisplaySet copy in displaySetCopies)
				copy.Dispose();

			foreach (DisplaySet displaySet in this.DisplaySets)
				displaySet.Dispose();

			_displaySets.ItemAdded -= OnDisplaySetAdded;
			_displaySets.ItemChanging -= OnDisplaySetChanging;
			_displaySets.ItemRemoved -= OnDisplaySetRemoved;
			_displaySets.ItemChanged -= OnDisplaySetChanged;

			_displaySets = null;
		}

		#endregion

		private void OnDisplaySetAdded(object sender, ListEventArgs<IDisplaySet> e)
		{
			OnDisplaySetAdded((DisplaySet)e.Item);
		}


		private void OnDisplaySetChanged(object sender, ListEventArgs<IDisplaySet> e)
		{
			OnDisplaySetAdded((DisplaySet)e.Item);
		}

		private void OnDisplaySetChanging(object sender, ListEventArgs<IDisplaySet> e)
		{
			OnDisplaySetRemoved((DisplaySet)e.Item);
		}

		private void OnDisplaySetRemoved(object sender, ListEventArgs<IDisplaySet> e)
		{
			OnDisplaySetRemoved((DisplaySet)e.Item);
		}

		/// <summary>
		/// Called when a new <see cref="DisplaySet"/> has been added.
		/// </summary>
		protected virtual void OnDisplaySetAdded(DisplaySet displaySet)
		{
			displaySet.ParentImageSet = this;
			displaySet.ImageViewer = this.ImageViewer;
		}

		/// <summary>
		/// Called when a <see cref="DisplaySet"/> has been removed.
		/// </summary>
		protected virtual void OnDisplaySetRemoved(DisplaySet displaySet)
		{
			displaySet.ParentImageSet = null;
			displaySet.ImageViewer = null;
		}

		private void OnDrawing()
		{
			EventsHelper.Fire(_drawing, this, EventArgs.Empty);
		}
	}
}
