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
using System.ComponentModel;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class TextAreaField : UserControl
    {
        public TextAreaField()
        {
            InitializeComponent();
        }

        
        public string Value
        {
            get { return NullIfEmpty(_textBox.Text); }
            set { _textBox.Text = value; }
        }
        
        public event EventHandler ValueChanged
        {
            add { _textBox.TextChanged += value; }
            remove { _textBox.TextChanged -= value; }
        }

        public string LabelText
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return _textBox.ReadOnly; }
            set { _textBox.ReadOnly = value; }
        }

        [DefaultValue(true)]
        public bool WordWrap
        {
            get { return _textBox.WordWrap; }
            set { _textBox.WordWrap = value; }
        }

        [DefaultValue(ScrollBars.None)]
        public ScrollBars ScrollBars
        {
            get { return _textBox.ScrollBars; }
            set { _textBox.ScrollBars = value; }
        }

		[DefaultValue(32767)]
		public int MaximumLength
		{
			get { return _textBox.MaxLength; }
			set { _textBox.MaxLength = value; }
		}

        private static string NullIfEmpty(string value)
        {
            return (value != null && value.Length == 0) ? null : value;
        }

		private void _textBox_DragEnter(object sender, DragEventArgs e)
		{
			if (_textBox.ReadOnly)
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

		private void _textBox_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.Text))
			{
				string dropString = (String)e.Data.GetData(typeof(String));

				// Insert string at the current keyboard cursor
				int currentIndex = _textBox.SelectionStart;
				_textBox.Text = string.Concat(
					_textBox.Text.Substring(0, currentIndex),
					dropString,
					_textBox.Text.Substring(currentIndex));
			}
		}
    }
}
