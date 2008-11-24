#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.ComponentModel;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public interface IRichTextBoxOwner
	{
		/// <summary>
		/// Get the RichTextBox control
		/// </summary>
		RichTextBox GetRichTextBox();

		/// <summary>
		/// Use this method instead of the RichTextBox.SelectedText
		/// </summary>
		/// <param name="text"></param>
		void SetSelectedText(string text);
	}

	public partial class RichTextField : UserControl, IRichTextBoxOwner
	{
		public RichTextField()
		{
			InitializeComponent();
		}

		public string Value
		{
			get { return NullIfEmpty(_richTextBox.Text); }
			set { _richTextBox.Text = value; }
		}

		public event EventHandler ValueChanged
		{
			add { _richTextBox.TextChanged += value; }
			remove { _richTextBox.TextChanged -= value; }
		}

		public string LabelText
		{
			get { return _label.Text; }
			set { _label.Text = value; }
		}

		[DefaultValue(false)]
		public bool ReadOnly
		{
			get { return _richTextBox.ReadOnly; }
			set { _richTextBox.ReadOnly = value; }
		}

		[DefaultValue(true)]
		public bool WordWrap
		{
			get { return _richTextBox.WordWrap; }
			set { _richTextBox.WordWrap = value; }
		}

		[DefaultValue(RichTextBoxScrollBars.None)]
		public RichTextBoxScrollBars ScrollBars
		{
			get { return _richTextBox.ScrollBars; }
			set { _richTextBox.ScrollBars = value; }
		}

		[DefaultValue(32767)]
		public int MaximumLength
		{
			get { return _richTextBox.MaxLength; }
			set { _richTextBox.MaxLength = value; }
		}

		private static string NullIfEmpty(string value)
		{
			return (value != null && value.Length == 0) ? null : value;
		}

		private void _richTextBox_DragEnter(object sender, DragEventArgs e)
		{
			if (_richTextBox.ReadOnly)
			{
				e.Effect = DragDropEffects.None;
				return;
			}

			if (e.Data.GetDataPresent(DataFormats.Text))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void _richTextBox_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.Text))
			{
				string dropString = (String)e.Data.GetData(typeof(String));

				// Insert string at the current keyboard cursor
				int currentIndex = _richTextBox.SelectionStart;
				_richTextBox.Text = string.Concat(
					_richTextBox.Text.Substring(0, currentIndex),
					dropString,
					_richTextBox.Text.Substring(currentIndex));
			}
		}

		#region IRichTextBoxOwner Members

		public RichTextBox GetRichTextBox()
		{
			return _richTextBox;
		}

		public void SetSelectedText(string text)
		{
			_richTextBox.SelectedText = text;
		}

		#endregion
	}
}
