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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	/// <summary>
	/// A <see cref="TextBox"/> control designed for <see cref="EditBox"/>es.
	/// </summary>
	internal class EditBoxControl : TextBox
	{
		private EditBox _editBox;
		private bool _hasChanges = false;

		public EditBoxControl()
		{
			// we can't do a transparent background unless we user paint it...
			// base.SetStyle(ControlStyles.UserPaint, true);
			// base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			// base.BackColor = Color.FromArgb(128, 128, 128, 128);

			base.AcceptsReturn = false;
			base.AcceptsTab = false;
			base.BackColor = Color.Black;
			base.BorderStyle = BorderStyle.None;
			base.ForeColor = Color.Tomato;
			base.Multiline = true;
			base.Visible = false;
			base.WordWrap = false;
		}

		public EditBox EditBox
		{
			get { return _editBox; }
			set
			{
				if (_editBox != null)
				{
					base.Visible = false;
				}

				_editBox = value;

				if (_editBox != null)
				{
					base.Font = new Font(_editBox.FontName, _editBox.FontSize);
					base.Text = _editBox.Value;
					base.Bounds = ComputeEditBoxControlBounds(this, _editBox);
					base.Visible = true;
					base.Focus();
					base.SelectAll();
				}

				_hasChanges = false;
			}
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			base.OnPreviewKeyDown(e);

			if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
				e.IsInputKey = true;
			else if (e.KeyCode == Keys.Tab)
				e.IsInputKey = false;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);

			if (_editBox == null)
				return;

			_editBox.Value = base.Text;
			_hasChanges = true;
			base.Bounds = ComputeEditBoxControlBounds(this, _editBox);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);

			if (_editBox == null)
				return;

			// if the user clicks away, then we infer if the user meant to accept or cancel
			// based on if the user has actually typed into the control
			if (_hasChanges)
				_editBox.Accept();
			else 
				_editBox.Cancel();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (_editBox == null)
				return;

			if (e.KeyCode == Keys.Escape)
			{
				_editBox.Cancel();
				e.Handled = e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.Enter && e.Modifiers == 0)
			{
				_editBox.Accept();
				e.Handled = e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.Enter && !_editBox.Multiline)
			{
				e.Handled = e.SuppressKeyPress = true;
			}
		}

		private static Rectangle ComputeEditBoxControlBounds(Control control, EditBox editBox)
		{
			Size sz = control.GetPreferredSize(Size.Empty);
			sz = new Size(Math.Max(Math.Max(sz.Width, editBox.Size.Width), 50), Math.Max(Math.Max(sz.Height, editBox.Size.Height), 21));
			return RectangleUtilities.ConvertToRectangle(editBox.Location, sz);
		}
	}
}