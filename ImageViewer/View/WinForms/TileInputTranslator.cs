#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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

		public object OnLostFocus()
		{
			return new LostFocusMessage();
		}

		public object OnMouseLeave()
		{
			return new MouseLeaveMessage();
		}

		public object OnMouseMove(MouseEventArgs e)
		{
			return new TrackMousePositionMessage(e.Location);
		}

		public object OnMouseDown(MouseEventArgs e)
		{
			return new MouseButtonMessage(e.Location, (XMouseButtons)e.Button, MouseButtonMessage.ButtonActions.Down, (uint)e.Clicks, this.Control, this.Alt, this.Shift);
		}

		public object OnMouseUp(MouseEventArgs e)
		{
			return new MouseButtonMessage(e.Location, (XMouseButtons)e.Button, MouseButtonMessage.ButtonActions.Up, 0, this.Control, this.Alt, this.Shift);
		}

		public object OnMouseWheel(MouseEventArgs e)
		{
			return new MouseWheelMessage(e.Delta, this.Control, this.Alt, this.Shift);
		}

		public object OnKeyDown(KeyEventArgs e)
		{
			if (ConsumeKeyStroke(e.KeyCode))
				return null;

			return new KeyboardButtonMessage((XKeys)e.KeyData, KeyboardButtonMessage.ButtonActions.Down);
		}

		public object OnKeyUp(KeyEventArgs e)
		{
			if (ConsumeKeyStroke(e.KeyCode))
				return null;

			return new KeyboardButtonMessage((XKeys)e.KeyData, KeyboardButtonMessage.ButtonActions.Up);
		}

		public object PostProcessMessage(Message msg, bool alreadyHandled)
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

		public object PreProcessMessage(Message msg)
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
