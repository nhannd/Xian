using System;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model
{
	/// <summary>
	/// Summary description for XKeyEventArgs.
	/// </summary>
	public class XKeyEventArgs : EventArgs 
	{
		private XKeys m_KeyData;
		private bool m_Handled = false;
		private PresentationImage m_SelectedPresentationImage = null;
		private DisplaySet m_SelectedDisplaySet = null;

		public XKeyEventArgs (XKeys keyData)
		{
			Platform.CheckForNullReference(keyData, "keyData");
			this.m_KeyData = keyData;
		}

		public virtual bool Alt 
		{
			get 
			{
				return (m_KeyData == XKeys.Alt);
			}
		}
		
		public bool Control 
		{
			get 
			{
				return (m_KeyData == XKeys.Control);
			}
		}
		
		public bool Handled 
		{
			get 
			{
				return m_Handled;
			}
			set 
			{
				m_Handled = value;
			}
		}
		
		public XKeys KeyCode 
		{
			get 
			{
				return m_KeyData & XKeys.KeyCode;
			}
		}
		
		public XKeys KeyData 
		{
			get 
			{
				return m_KeyData;
			}
		}
		
		public int KeyValue 
		{
			get 
			{
				return Convert.ToInt32(m_KeyData);
			}
		}
		
		public XKeys Modifiers 
		{
			get 
			{
				XKeys returnKeys = new XKeys();
				
				if(m_KeyData == XKeys.Alt)
					returnKeys = XKeys.Alt;
				
				if(m_KeyData == XKeys.Control)
					returnKeys = returnKeys | XKeys.Control;
				
				if(m_KeyData == XKeys.Shift)
					returnKeys = returnKeys | XKeys.Shift;
				
				return returnKeys;
			}
		}
		
		public virtual bool Shift 
		{
			get 
			{
				return (m_KeyData == XKeys.Shift);
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
	}
}
