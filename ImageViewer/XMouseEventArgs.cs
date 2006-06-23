using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Summary description for XMouseEventArgs.
	/// </summary>
	public class XMouseEventArgs : EventArgs, ICloneable
	{
		private XMouseButtons m_Button;
		private int m_Clicks;
		private int m_X;
		private int m_Y;
		private int m_Delta;
		private XKeys m_ModifierKeys;
		private ImageBox m_SelectedImageBox;
		private Tile m_SelectedTile;
		private PresentationImage m_SelectedPresentationImage;
		private DisplaySet m_SelectedDisplaySet;

		public XMouseEventArgs(XMouseButtons button, int clicks, int x, int y, int delta, XKeys modifierKeys)
		{
			Platform.CheckForNullReference(button, "button");
			Platform.CheckForNullReference(modifierKeys, "modifierKeys");

			m_Button = button;
			m_Clicks = clicks;
			m_X = x;
			m_Y = y;
			m_Delta = delta;
			m_ModifierKeys = modifierKeys;
		}

		public XMouseButtons Button
		{
			get { return m_Button; }
		}

		public int Clicks
		{
			get { return m_Clicks; }
		}

		public int X
		{
			get { return m_X; }
		}
		
		public int Y
		{
			get { return m_Y; }
		}
		
		public int Delta
		{
			get { return m_Delta; }
		}

		public XKeys ModifierKeys
		{
			get { return m_ModifierKeys; }
		}

		public ImageBox SelectedImageBox
		{
			get { return m_SelectedImageBox; }
			set 
			{ 
				Platform.CheckForNullReference(value, "SelectedImageBox");
				m_SelectedImageBox = value; 
			}
		}
		
		public Tile SelectedTile
		{
			get { return m_SelectedTile; }
			set 
			{ 
				Platform.CheckForNullReference(value, "SelectedTile");
				m_SelectedTile = value; 
			}
		}

		public PresentationImage SelectedPresentationImage
		{
			get { return m_SelectedPresentationImage; }
			set 
			{ 
				Platform.CheckForNullReference(value, "SelectedPresentationImage");
				m_SelectedPresentationImage = value; 
			}
		}

		public DisplaySet SelectedDisplaySet
		{
			get { return m_SelectedDisplaySet; }
			set 
			{ 
				Platform.CheckForNullReference(value, "SelectedDisplaySet");
				m_SelectedDisplaySet = value; 
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
