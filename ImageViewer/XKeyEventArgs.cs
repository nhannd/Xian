using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Summary description for XKeyEventArgs.
	/// </summary>
	public class XKeyEventArgs : EventArgs 
	{
		private XKeys _KeyData;
		private bool _Handled = false;
		private PresentationImage _SelectedPresentationImage = null;
		private DisplaySet _SelectedDisplaySet = null;

		public XKeyEventArgs (XKeys keyData)
		{
			Platform.CheckForNullReference(keyData, "keyData");
			this._KeyData = keyData;
		}

		public virtual bool Alt 
		{
			get 
			{
				return (_KeyData == XKeys.Alt);
			}
		}
		
		public bool Control 
		{
			get 
			{
				return (_KeyData == XKeys.Control);
			}
		}
		
		public bool Handled 
		{
			get 
			{
				return _Handled;
			}
			set 
			{
				_Handled = value;
			}
		}
		
		public XKeys KeyCode 
		{
			get 
			{
				return _KeyData & XKeys.KeyCode;
			}
		}
		
		public XKeys KeyData 
		{
			get 
			{
				return _KeyData;
			}
		}
		
		public int KeyValue 
		{
			get 
			{
				return Convert.ToInt32(_KeyData);
			}
		}
		
		public XKeys Modifiers 
		{
			get 
			{
				XKeys returnKeys = new XKeys();
				
				if(_KeyData == XKeys.Alt)
					returnKeys = XKeys.Alt;
				
				if(_KeyData == XKeys.Control)
					returnKeys = returnKeys | XKeys.Control;
				
				if(_KeyData == XKeys.Shift)
					returnKeys = returnKeys | XKeys.Shift;
				
				return returnKeys;
			}
		}
		
		public virtual bool Shift 
		{
			get 
			{
				return (_KeyData == XKeys.Shift);
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
	}
}
