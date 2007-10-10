using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	internal sealed class TileInputTranslator
	{
		private TileControl _tileControl;

		private Keys[] _consumeKeyStrokes = 
						{	Keys.ControlKey,
							Keys.LControlKey,
							Keys.RControlKey,
							Keys.ShiftKey,
							Keys.LShiftKey,
							Keys.RShiftKey,
							Keys.Menu
						};

		public TileInputTranslator(TileControl tileControl)
		{
			_tileControl = tileControl;
		}

		private Keys Modifiers
		{
			get { return System.Windows.Forms.Control.ModifierKeys; }
		}

		private Point MousePositionScreen
		{
			get { return System.Windows.Forms.Control.MousePosition; }
		}

		private Point MousePositionClient
		{
			get { return _tileControl.PointToClient(this.MousePositionScreen); }
		}

		private MouseButtons MouseButtons
		{
			get { return System.Windows.Forms.Control.MouseButtons; }
		}

		private bool Control
		{
			get { return (this.Modifiers & Keys.Control) == Keys.Control; }
		}

		private bool Alt
		{
			get { return (this.Modifiers & Keys.Alt) == Keys.Alt; }
		}

		private bool Shift
		{
			get { return (this.Modifiers & Keys.Shift) == Keys.Shift; }
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

		public IInputMessage OnMouseLeave()
		{
			return new MouseLeaveMessage();
		}

		public IInputMessage OnMouseMove(MouseEventArgs e)
		{
			return new TrackMousePositionMessage(e.Location);
		}

		public IInputMessage OnMouseDown(MouseEventArgs e)
		{
			return new MouseButtonMessage(e.Location, (XMouseButtons)e.Button, MouseButtonMessage.ButtonActions.Down, (uint)e.Clicks, this.Control, this.Alt, this.Shift);
		}

		public IInputMessage OnMouseUp(MouseEventArgs e)
		{
			return new MouseButtonMessage(e.Location, (XMouseButtons)e.Button, MouseButtonMessage.ButtonActions.Up, 0, this.Control, this.Alt, this.Shift);
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
				if (!ConsumeKeyStroke(keyData))
				{
					//when a keystroke gets handled by a control other than the tile, we release the capture.
					return new ReleaseCaptureMessage();
				}
			}

			return null;
		}

		public IInputMessage PreProcessMessage(Message msg)
		{
			if (msg.Msg == 0x100)
			{
				Keys keyData = (Keys)msg.WParam;
				if (!ConsumeKeyStroke(keyData))
					return new KeyboardButtonDownPreview((XKeys)msg.WParam);
			}

			return null;
		}
	}
}
