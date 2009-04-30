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
using System.Text;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	/// <summary>
	/// A <see cref="StudyComposerItemBase{T}"/> that wraps a <see cref="SeriesNode"/> in a <see cref="StudyBuilder"/> tree.
	/// </summary>
	public class SeriesItem : StudyComposerItemBase<SeriesNode>
	{
		private readonly ImageItemCollection _images;

		public SeriesItem(SeriesNode series)
		{
			base.Node = series;
			_images = new ImageItemCollection(series.Images);
			_images.ListChanged += new ListChangedEventHandler(OnChildListChanged);
		}

		private SeriesItem(SeriesItem source) : this(source.Node.Copy(false))
		{
			this.Icon = (Image) source.Icon.Clone();
			foreach (ImageItem image in source.Images)
			{
				this.Images.Add(image.Copy());
			}
		}

		public ImageItemCollection Images
		{
			get { return _images; }
		}

		public void InsertItems(ImageItem[] images)
		{
			foreach (ImageItem item in images)
			{
				this.Images.Add(item);
			}
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public SeriesItem Copy()
		{
			return new SeriesItem(this);
		}

		private void OnChildListChanged(object sender, ListChangedEventArgs e)
		{
			switch(e.ListChangedType)
			{
				case ListChangedType.Reset:
				case ListChangedType.ItemAdded:
				case ListChangedType.ItemDeleted:
				case ListChangedType.ItemMoved:
				case ListChangedType.ItemChanged:
					UpdateIcon();
					break;
			}
		}

		// The key image is the middle image of the series
		private Image GetKeyImage()
		{
			if (_images != null && _images.Count > 0)
				return _images[(_images.Count - 1)/2].Icon;
			return null;
		}

		#region Overrides

		/// <summary>
		/// Gets or sets the name label of this item.
		/// </summary>
		public override string Name
		{
			get { return base.Node.Description; }
			set { base.Node.Description = value; }
		}

		/// <summary>
		/// Gets a short, multi-line description of the item that contains ancillary information.
		/// </summary>
		public override string Description
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (base.Node.DateTime.HasValue)
					sb.AppendLine(base.Node.DateTime.Value.ToString());
				sb.AppendLine(base.Node.InstanceUid);
				return sb.ToString();
			}
		}

		protected override void OnNodePropertyChanged(string propertyName)
		{
			base.OnNodePropertyChanged(propertyName);

			if (propertyName == "Description")
				base.FirePropertyChanged("Name");
			else if (propertyName == "DateTime" || propertyName == "InstanceUid")
				base.FirePropertyChanged("Description");
		}

		/// <summary>
		/// Regenerates the icon for a specific icon size.
		/// </summary>
		/// <param name="iconSize">The <see cref="Size"/> of the icon to generate.</param>
		public override void UpdateIcon(Size iconSize)
		{
			Image keyImage = GetKeyImage();
			_helper.IconSize = iconSize;
			base.Icon = _helper.CreateStackIcon(keyImage);
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override StudyComposerItemBase<SeriesNode> Clone()
		{
			return this.Copy();
		}

		#endregion

		#region Statics

		private static readonly IconHelper _helper = new IconHelper();

		static SeriesItem()
		{
			_helper.IconSize = new Size(64, 64);
			_helper.StackSize = 3;
			_helper.StackOffset = new Size(5, -5);
		}

		#endregion
	}
}