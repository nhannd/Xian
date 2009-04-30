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
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	/// <summary>
	/// A generic base class for items in the <see cref=StudyComposerComponent"/> tree.
	/// </summary>
	public abstract class StudyComposerItemBase<T> : IStudyComposerItem, IGalleryItem, ICloneable
		where T : StudyBuilderNode
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private Image _icon = null;
		private T _node;

		/// <summary>
		/// Constructs a new <see cref="StudyComposerItemBase{T}"/>.
		/// </summary>
		internal StudyComposerItemBase() {}

		~StudyComposerItemBase()
		{
			// disconnect the node event handler if we still have a reference to the node
			if (this.Node != null)
				_node.PropertyChanged -= Node_PropertyChanged;
		}

		/// <summary>
		/// Indicates that a property on the node has changed, and that any views should refresh its display of the item.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		/// <summary>
		/// Gets or sets the name label of this item.
		/// </summary>
		public abstract string Name { get; set; }

		/// <summary>
		/// Gets a short, multi-line description of the item that contains ancillary information.
		/// </summary>
		public abstract string Description { get; }

		/// <summary>
		/// Gets the <see cref="StudyBuilderNode"/> node that is encapsulated by this <see cref="StudyComposerItemBase{T}"/>.
		/// </summary>
		public T Node
		{
			get { return _node; }
			protected set
			{
				if (_node != value)
				{
					if (_node != null)
						_node.PropertyChanged -= Node_PropertyChanged;

					_node = value;

					if (_node != null)
						_node.PropertyChanged += Node_PropertyChanged;

					FirePropertyChanged("Node");
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="StudyBuilderNode"/> node that is encapsulated by this <see cref="StudyComposerItemBase{T}"/>.
		/// </summary>
		/// <remarks>
		/// Returns the same value as <see cref="Node"/>, but hidden to provide the strongly-typed alternative.
		/// </remarks>
		StudyBuilderNode IStudyComposerItem.Node
		{
			get { return this.Node; }
		}

		/// <summary>
		/// Gets the <see cref="StudyBuilderNode"/> node that is encapsulated by this <see cref="StudyComposerItemBase{T}"/>.
		/// </summary>
		/// <remarks>
		/// Returns the same value as <see cref="Node"/>, but hidden to provide the strongly-typed alternative, as well as to rename
		/// the field to make it more clear that the item is the encapsulated node.
		/// </remarks>
		object IGalleryItem.Item
		{
			get { return this.Node; }
		}

		/// <summary>
		/// Gets an <see cref="Image"/> icon that can be used to represent the item in thumbnail views.
		/// </summary>
		public Image Icon
		{
			get { return _icon; }
			protected set
			{
				if (_icon != value)
				{
					_icon = value;
					FirePropertyChanged("Icon");
				}
			}
		}

		/// <summary>
		/// Gets an <see cref="Image"/> icon that can be used to represent the item in thumbnail views.
		/// </summary>
		/// <remarks>
		/// Returns the same value as <see cref="Icon"/>, but hidden to provide the strongly-typed alternative, as well as to rename
		/// the field to make it more clear that this has nothing to do with the <see cref="SeriesItem.Images"/> field, which lists
		/// the images in a series.
		/// </remarks>
		Image IGalleryItem.Image
		{
			get { return this.Icon; }
		}

		/// <summary>
		/// Regenerates the icon for a specific icon size.
		/// </summary>
		/// <param name="iconSize">The <see cref="Size"/> of the icon to generate.</param>
		public abstract void UpdateIcon(Size iconSize);

		/// <summary>
		/// Regenerates the icon for the default icon size (64x64).
		/// </summary>
		public void UpdateIcon()
		{
			UpdateIcon(new Size(64, 64));
		}

		/// <summary>
		/// Gets a string representation of the item.
		/// </summary>
		/// <remarks>In most cases, this is simply the name label of the item.</remarks>
		/// <returns>A string representation of the item.</returns>
		public override string ToString()
		{
			return this.Name;
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public abstract StudyComposerItemBase<T> Clone();

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		IStudyComposerItem IStudyComposerItem.Clone()
		{
			return this.Clone();
		}

		private void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnNodePropertyChanged(e.PropertyName);
		}

		protected virtual void OnNodePropertyChanged(string propertyName) {}

		/// <summary>
		/// Raises the <see cref="PropertyChanged"/> event for the given property.
		/// </summary>
		/// <param name="propertyName"></param>
		protected void FirePropertyChanged(string propertyName)
		{
			if (_propertyChanged != null)
				_propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}