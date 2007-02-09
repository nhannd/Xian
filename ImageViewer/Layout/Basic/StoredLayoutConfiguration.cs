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
		public const int MaximumImageBoxRows = 8;
		public const int MaximumImageBoxColumns = 10;
		public const int MaximumTileRows = 8;
		public const int MaximumTileColumns = 10;

		private string _modality;
		private int _imageBoxRows;
		private int _imageBoxColumns;
		private int _tileRows;
		private int _tileColumns;

		private event PropertyChangedEventHandler _propertyChanged;

		internal StoredLayoutConfiguration()
			: this("")
		{
		}

		internal StoredLayoutConfiguration(string modality)
			: this(modality, 1, 2, 1, 1)
		{
		}

		internal StoredLayoutConfiguration(string modality, int imageBoxRows, int imageBoxColumns, int tileRows, int tileColumns)
		{
			_modality = (modality == null) ? "" : modality;
			_imageBoxRows = imageBoxRows;
			_imageBoxColumns = imageBoxColumns;
			_tileRows = tileRows;
			_tileColumns = tileColumns;
		}

		public int ImageBoxRows
		{
			get { return _imageBoxRows; }
			set
			{
				if (_imageBoxRows == value)
					return;

				_imageBoxRows = Math.Max(value, 1);
				_imageBoxRows = Math.Min(_imageBoxRows, MaximumImageBoxRows);

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
				_imageBoxColumns = Math.Min(_imageBoxColumns, MaximumImageBoxColumns);

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
				_tileRows = Math.Min(_tileRows, MaximumTileRows);

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
				_tileColumns = Math.Min(_tileColumns, MaximumTileColumns);

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

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		#endregion
	}
}
