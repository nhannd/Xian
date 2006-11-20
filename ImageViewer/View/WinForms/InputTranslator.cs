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
	public sealed class InputTranslator
	{
		private PropertyGetDelegate<Keys> _getModifiersDelegate;

		private Keys[] _consumeKeyStrokes = 
						{	Keys.ControlKey,
							Keys.LControlKey,
							Keys.RControlKey,
							Keys.ShiftKey,
							Keys.LShiftKey,
							Keys.RShiftKey,
						};

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

		private bool ConsumeKeyStroke(Keys keyCode)
		{
			foreach (Keys keyStroke in _consumeKeyStrokes)
			{
				if (keyCode == keyStroke)
					return true;
			}

			return false;
		}

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
			if (ConsumeKeyStroke(e.KeyCode))
				return null;

			return new KeyboardButtonMessage((XKeys)e.KeyData, KeyboardButtonMessage.ButtonActions.Down);
		}

		public IInputMessage OnKeyUp(KeyEventArgs e)
		{
			if (ConsumeKeyStroke(e.KeyCode))
				return null;

			return new KeyboardButtonMessage((XKeys)e.KeyData, KeyboardButtonMessage.ButtonActions.Up);
		}

		public IInputMessage PostProcessMessage(Message msg, bool alreadyHandled)
		{
			if (msg.Msg == 0x100 && alreadyHandled)
			{
				Keys keyData = (Keys)msg.WParam;
				if (ConsumeKeyStroke(keyData))
					return null;

				//when a keystroke gets handled by a control other than the tile, we release the capture.
				return new ReleaseCaptureMessage();
			}

			return null;
		}
	}
}
