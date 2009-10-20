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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClearCanvas.Controls.WinForms
{
	public class FolderLocationToolStripTextBox : ToolStripControlHost
	{
		private event EventHandler _keyEnterPressed;

		public FolderLocationToolStripTextBox() : base(new TextControl()) {}

		public FolderLocationToolStripTextBox(string name)
			: base(new TextControl(), name) {}

		public override string Text
		{
			get { return this.Control.Text; }
			set { this.Control.Text = value; }
		}

		protected new TextBoxBase Control
		{
			get { return (TextBoxBase) base.Control; }
		}

		public event EventHandler KeyEnterPressed
		{
			add { _keyEnterPressed += value; }
			remove { _keyEnterPressed -= value; }
		}

		protected virtual void OnKeyEnterPressed(EventArgs e)
		{
			if (_keyEnterPressed != null)
				_keyEnterPressed.Invoke(this, e);
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if (keyData == Keys.Enter)
				return true;
			return base.IsInputKey(keyData);
		}

		public override Size GetPreferredSize(Size constrainingSize)
		{
			if (this.Owner == null)
				return base.GetPreferredSize(constrainingSize);

			int width = this.Owner.DisplayRectangle.Width;
			int height = 0;
			int meCount = 0;

			foreach (ToolStripItem item in this.Owner.Items)
			{
				if (item is FolderLocationToolStripTextBox)
				{
					meCount++;
					width -= item.Margin.Horizontal;
					height = Math.Max(height, this.Control.Height + item.Margin.Vertical);
				}
				else
				{
					width = width - item.Width - item.Margin.Horizontal;
					height = Math.Max(height, item.GetPreferredSize(constrainingSize).Height + item.Margin.Vertical);
				}
			}

			if (meCount > 1)
				width /= meCount;
			return new Size(width, height);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.OnKeyEnterPressed(EventArgs.Empty);
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			base.OnKeyDown(e);
		}

		protected override void OnSubscribeControlEvents(Control control)
		{
			base.OnSubscribeControlEvents(control);
			control.TextChanged += OnHostedControlTextChanged;
		}

		protected override void OnUnsubscribeControlEvents(Control control)
		{
			control.TextChanged -= OnHostedControlTextChanged;
			base.OnUnsubscribeControlEvents(control);
		}

		private void OnHostedControlTextChanged(object sender, EventArgs e)
		{
			this.OnTextChanged(e);
		}

		/// <summary>
		/// A text control that will select all on initial click, tab and refocusing alt+tab
		/// </summary>
		/// <remarks>
		/// Kudos to JH for posting the winning formula for forcing all text selection on mouse click over at
		/// <see cref="http://stackoverflow.com/questions/97459/automatically-select-all-text-on-focus-in-winforms-textbox">Stack Overflow</see>
		/// </remarks>
		private class TextControl : TextBox
		{
			private readonly Size _defaultSize;
			private bool _alreadyFocused = false;

			public TextControl()
			{
				_defaultSize = base.DefaultSize;
			}

			protected override Size DefaultSize
			{
				get { return _defaultSize; }
			}

			protected override void OnLeave(EventArgs e)
			{
				base.OnLeave(e);
				_alreadyFocused = false;
			}

			protected override void OnGotFocus(EventArgs e)
			{
				base.OnGotFocus(e);
				if (MouseButtons == MouseButtons.None)
				{
					_alreadyFocused = true;
					base.SelectAll();
				}
			}

			protected override void OnMouseUp(MouseEventArgs mevent)
			{
				base.OnMouseUp(mevent);
				if (!_alreadyFocused && base.SelectionLength == 0)
				{
					_alreadyFocused = true;
					base.SelectAll();
				}
			}
		}
	}
}