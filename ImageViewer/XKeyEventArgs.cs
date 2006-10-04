using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class XKeyEventArgs : EventArgs 
	{
		private XKeys _keyData;
		private bool _handled = false;
		private PresentationImage _selectedPresentationImage = null;
		private DisplaySet _selectedDisplaySet = null;

		public XKeyEventArgs (XKeys keyData)
		{
			Platform.CheckForNullReference(keyData, "keyData");
			this._keyData = keyData;
		}

		public virtual bool Alt 
		{
			get 
			{
				return (_keyData == XKeys.Alt);
			}
		}
		
		public bool Control 
		{
			get 
			{
				return (_keyData == XKeys.Control);
			}
		}
		
		public bool Handled 
		{
			get 
			{
				return _handled;
			}
			set 
			{
				_handled = value;
			}
		}
		
		public XKeys KeyCode 
		{
			get 
			{
				return _keyData & XKeys.KeyCode;
			}
		}
		
		public XKeys KeyData 
		{
			get 
			{
				return _keyData;
			}
		}
		
		public int KeyValue 
		{
			get 
			{
				return Convert.ToInt32(_keyData);
			}
		}
		
		public XKeys Modifiers 
		{
			get 
			{
				XKeys returnKeys = new XKeys();
				
				if(_keyData == XKeys.Alt)
					returnKeys = XKeys.Alt;
				
				if(_keyData == XKeys.Control)
					returnKeys = returnKeys | XKeys.Control;
				
				if(_keyData == XKeys.Shift)
					returnKeys = returnKeys | XKeys.Shift;
				
				return returnKeys;
			}
		}
		
		public virtual bool Shift 
		{
			get 
			{
				return (_keyData == XKeys.Shift);
			}
		}

		public PresentationImage SelectedPresentationImage
		{
			get { return _selectedPresentationImage; }
			set 
			{
				Platform.CheckForNullReference(value, "SelectedPresentationImage");
				_selectedPresentationImage = value; 
			}
		}

		public DisplaySet SelectedDisplaySet
		{
			get { return _selectedDisplaySet; }
			set 
			{
				Platform.CheckForNullReference(value, "SelectedDisplaySet");
				_selectedDisplaySet = value; 
			}
		}
	}
}
