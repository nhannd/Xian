using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Summary description for XMouseEventArgs.
	/// </summary>
	public class XMouseEventArgs : EventArgs, ICloneable
	{
		private XMouseButtons _Button;
		private int _Clicks;
		private int _X;
		private int _Y;
		private int _Delta;
		private XKeys _ModifierKeys;
		private ImageBox _SelectedImageBox;
		private Tile _SelectedTile;
		private PresentationImage _SelectedPresentationImage;
		private DisplaySet _SelectedDisplaySet;

		public XMouseEventArgs(XMouseButtons button, int clicks, int x, int y, int delta, XKeys modifierKeys)
		{
			Platform.CheckForNullReference(button, "button");
			Platform.CheckForNullReference(modifierKeys, "modifierKeys");

			_Button = button;
			_Clicks = clicks;
			_X = x;
			_Y = y;
			_Delta = delta;
			_ModifierKeys = modifierKeys;
		}

		public XMouseButtons Button
		{
			get { return _Button; }
		}

		public int Clicks
		{
			get { return _Clicks; }
		}

		public int X
		{
			get { return _X; }
		}
		
		public int Y
		{
			get { return _Y; }
		}
		
		public int Delta
		{
			get { return _Delta; }
		}

		public XKeys ModifierKeys
		{
			get { return _ModifierKeys; }
		}

		public ImageBox SelectedImageBox
		{
			get { return _SelectedImageBox; }
			set 
			{ 
				Platform.CheckForNullReference(value, "SelectedImageBox");
				_SelectedImageBox = value; 
			}
		}
		
		public Tile SelectedTile
		{
			get { return _SelectedTile; }
			set 
			{ 
				Platform.CheckForNullReference(value, "SelectedTile");
				_SelectedTile = value; 
			}
		}

		public PresentationImage SelectedPresentationImage
		{
			get { return _SelectedPresentationImage; }
			set 
			{ 
				Platform.CheckForNullReference(value, "SelectedPresentationImage");
				_SelectedPresentationImage = value; 
			}
		}

		public DisplaySet SelectedDisplaySet
		{
			get { return _SelectedDisplaySet; }
			set 
			{ 
				Platform.CheckForNullReference(value, "SelectedDisplaySet");
				_SelectedDisplaySet = value; 
			}
		}

		#region ICloneable Members

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion
	}
}
