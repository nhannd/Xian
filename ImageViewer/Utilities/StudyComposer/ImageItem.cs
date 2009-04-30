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

using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	/// <summary>
	/// A <see cref="StudyComposerItemBase{T}"/> that wraps a <see cref="SopInstanceNode"/> in a <see cref="StudyBuilder"/> tree.
	/// </summary>
	public class ImageItem : StudyComposerItemBase<SopInstanceNode>
	{
		private readonly string _igKey;
		private string _name = SR.FormatStudyComposerGenericImageLabelCaption;

		public ImageItem(SopInstanceNode sopInstance, IPresentationImage pImage)
		{
			base.Node = sopInstance;

			int num = sopInstance.DicomData[DicomTags.InstanceNumber].GetInt32(0, -1);
			if (num >= 0)
				_name = string.Format(SR.FormatStudyComposerImageLabelCaption, num);

			string seriesDesc = sopInstance.DicomData[DicomTags.SeriesDescription].GetString(0, "");
			if (seriesDesc.Length > 0)
				_name = string.Format("{0}: {1}", seriesDesc, _name);

			_igKey = sopInstance.InstanceUid;

			if (pImage != null)
			{
				//_cache.LoadIcon(_igKey, delegate() { return _helper.CreateImageIcon(pImage); }, this.RefreshIcon);
				base.Icon = (Image)_helper.CreateImageIcon(pImage);// _cache[_igKey];
			}
		}

		private ImageItem(ImageItem source) : this(source.Node.Copy(), null)
		{
			this._name = source._name;
			this._igKey = source._igKey;
			base.Icon = (Image) source.Icon.Clone();// _cache[_igKey];
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public ImageItem Copy()
		{
			return new ImageItem(this);
		}

		private void RefreshIcon() {
			//base.Icon = _cache[_igKey];
		}

		#region Overrides

		/// <summary>
		/// Gets or sets the name label of this item.
		/// </summary>
		public override string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets a short, multi-line description of the item that contains ancillary information.
		/// </summary>
		public override string Description
		{
			get { return base.Node.InstanceUid; }
		}

		/// <summary>
		/// Regenerates the icon for a specific icon size.
		/// </summary>
		/// <param name="iconSize">The <see cref="Size"/> of the icon to generate.</param>
		public override void UpdateIcon(Size iconSize) {
			Image _baseIcon = this.Icon;// _cache[_igKey];
			if (_baseIcon.Size != iconSize)
			{
				// if requesting a new icon size
				_helper.IconSize = iconSize;
				base.Icon = _helper.CreateImageIcon(_baseIcon);
			}
			else
			{
				base.Icon = _baseIcon;
			}
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override StudyComposerItemBase<SopInstanceNode> Clone()
		{
			return this.Copy();
		}

		protected override void OnNodePropertyChanged(string propertyName)
		{
			base.OnNodePropertyChanged(propertyName);
			if (propertyName == "InstanceUid")
				FirePropertyChanged("Description");
		}

		#endregion

		#region Statics

		private static readonly IconHelper _helper = new IconHelper(64, 64);
		//private static readonly IconCache _cache = new IconCache();

		static ImageItem()
		{
			_helper.IconSize = new Size(64, 64);
		}

		internal static void ClearIconCache()
		{
			//_cache.Clear();
		}

		#endregion
	}
}