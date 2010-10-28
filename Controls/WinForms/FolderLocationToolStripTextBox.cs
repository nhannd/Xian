#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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