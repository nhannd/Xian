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
using System.Text;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common.Utilities;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public sealed class StoredLayoutConfigurationSortByModality : IComparer<StoredLayoutConfiguration>
	{
		public StoredLayoutConfigurationSortByModality()
		{ 
		}

		#region IComparer<StoredLayoutConfiguration> Members

		public int Compare(StoredLayoutConfiguration x, StoredLayoutConfiguration y)
		{
			return x.Modality.CompareTo(y.Modality);
		}

		#endregion
	}

	public sealed class StoredLayoutConfiguration : INotifyPropertyChanged
	{
		private string _modality;
		private int _imageBoxRows;
		private int _imageBoxColumns;
		private int _tileRows;
		private int _tileColumns;

		private event PropertyChangedEventHandler _propertyChanged;

		internal StoredLayoutConfiguration(string modality, int imageBoxRows, int imageBoxColumns, int tileRows, int tileColumns)
		{
			_modality = (modality == null) ? "" : modality;
			_imageBoxRows = imageBoxRows;
			_imageBoxColumns = imageBoxColumns;
			_tileRows = tileRows;
			_tileColumns = tileColumns;
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		#endregion

		public int ImageBoxRows
		{
			get { return _imageBoxRows; }
			set
			{
				if (_imageBoxRows == value)
					return;

				_imageBoxRows = Math.Max(value, 1);
				_imageBoxRows = Math.Min(_imageBoxRows, LayoutConfigurationSettings.MaximumImageBoxRows);

				EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs("ImageBoxRows"));
			}
		}

		public int ImageBoxColumns
		{
			get { return _imageBoxColumns; }
			set
			{
				if (_imageBoxColumns == value)
					return;

				_imageBoxColumns = Math.Max(value, 1);
				_imageBoxColumns = Math.Min(_imageBoxColumns, LayoutConfigurationSettings.MaximumImageBoxColumns);

				EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs("ImageBoxColumns"));
			}
		}

		public int TileRows
		{
			get { return _tileRows; }
			set
			{
				if (_tileRows == value)
					return;

				_tileRows = Math.Max(value, 1);
				_tileRows = Math.Min(_tileRows, LayoutConfigurationSettings.MaximumTileRows);

				EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs("TileRows"));
			}
		}

		public int TileColumns
		{
			get { return _tileColumns; }
			set
			{
				if (_tileColumns == value)
					return;

				_tileColumns = Math.Max(value, 1);
				_tileColumns = Math.Min(_tileColumns, LayoutConfigurationSettings.MaximumTileColumns);

				EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs("TileColumns"));
			}
		}
	
		public string Modality
		{
			get { return _modality; }
		}

		public string Text
		{
			get
			{
				return (this.IsDefault) ? SR.LabelDefault : _modality;
			}
		}

		public bool IsDefault
		{
			get { return String.IsNullOrEmpty(_modality); }
		}
	}
}
