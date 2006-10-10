using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class XMouseEventArgs : EventArgs, ICloneable
	{
		private XMouseButtons _button;
		private int _clicks;
		private int _x;
		private int _y;
		private int _delta;
		private XKeys _modifierKeys;
		private IImageBox _selectedImageBox;
		private ITile _selectedTile;
		private IPresentationImage _selectedPresentationImage;
		private IDisplaySet _selectedDisplaySet;
		private IMouseCapture _mouseCapture;

		public XMouseEventArgs(XMouseButtons button, int clicks, int x, int y, int delta, XKeys modifierKeys)
		{
			Platform.CheckForNullReference(button, "button");
			Platform.CheckForNullReference(modifierKeys, "modifierKeys");

			_button = button;
			_clicks = clicks;
			_x = x;
			_y = y;
			_delta = delta;
			_modifierKeys = modifierKeys;
		}

		public XMouseButtons Button
		{
			get { return _button; }
		}

		public int Clicks
		{
			get { return _clicks; }
		}

		public int X
		{
			get { return _x; }
		}
		
		public int Y
		{
			get { return _y; }
		}
		
		public int Delta
		{
			get { return _delta; }
		}

		public XKeys ModifierKeys
		{
			get { return _modifierKeys; }
		}

		public IImageBox SelectedImageBox
		{
			get { return _selectedImageBox; }
			set 
			{ 
				Platform.CheckForNullReference(value, "SelectedImageBox");
				_selectedImageBox = value; 
			}
		}

		public ITile SelectedTile
		{
			get { return _selectedTile; }
			set 
			{ 
				Platform.CheckForNullReference(value, "SelectedTile");
				_selectedTile = value; 
			}
		}

		public IPresentationImage SelectedPresentationImage
		{
			get { return _selectedPresentationImage; }
			set 
			{ 
				Platform.CheckForNullReference(value, "SelectedPresentationImage");
				_selectedPresentationImage = value; 
			}
		}

		public IDisplaySet SelectedDisplaySet
		{
			get { return _selectedDisplaySet; }
			set 
			{ 
				Platform.CheckForNullReference(value, "SelectedDisplaySet");
				_selectedDisplaySet = value; 
			}
		}

		public IMouseCapture MouseCapture
		{
			get { return _mouseCapture; }
			set { _mouseCapture = value; }
		}

		#region ICloneable Members

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion
	}
}
