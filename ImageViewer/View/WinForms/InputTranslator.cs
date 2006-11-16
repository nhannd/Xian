using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.InputManagement;
using System.Windows.Forms;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public sealed class InputTranslator : IInputTranslator
	{
		private PropertyGetDelegate<Keys> _getModifiersDelegate;

		public InputTranslator(PropertyGetDelegate<Keys> getModifiersDelegate)
		{
			Platform.CheckForNullReference(getModifiersDelegate, "getModifiersDelegate");

			_getModifiersDelegate = getModifiersDelegate;
		}

		private bool Control
		{
			get { return (_getModifiersDelegate() & Keys.Control) == Keys.Control; }
		}

		private bool Alt
		{
			get { return (_getModifiersDelegate() & Keys.Alt) == Keys.Alt; }
		}

		private bool Shift
		{
			get { return (_getModifiersDelegate() & Keys.Shift) == Keys.Shift; }
		}

		//#region IInputTranslator Members

		public IInputMessage OnMouseMove(MouseEventArgs e)
		{
			return new TrackMousePositionMessage(e.Location);
		}

		public IInputMessage OnMouseDown(MouseEventArgs e)
		{
			return new MouseButtonMessage(e.Location, (XMouseButtons)e.Button, MouseButtonMessage.ButtonActions.Down, this.Control, this.Alt, this.Shift);
		}

		public IInputMessage OnMouseUp(MouseEventArgs e)
		{
			return new MouseButtonMessage(e.Location, (XMouseButtons)e.Button, MouseButtonMessage.ButtonActions.Up, this.Control, this.Alt, this.Shift);
		}

		public IInputMessage OnMouseWheel(MouseEventArgs e)
		{
			return new MouseWheelMessage(e.Delta, this.Control, this.Alt, this.Shift);
		}

		public IInputMessage OnKeyDown(KeyEventArgs e)
		{
			return new KeyboardButtonMessage((XKeys)e.KeyData, KeyboardButtonMessage.ButtonActions.Down);
		}

		public IInputMessage OnKeyUp(KeyEventArgs e)
		{
			return new KeyboardButtonMessage((XKeys)e.KeyData, KeyboardButtonMessage.ButtonActions.Up);
		}
	}
}
