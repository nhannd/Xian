#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
    public partial class TextField : UserControl
    {
        public TextField()
        {
            InitializeComponent();
        }

        /*
        public string Value
        {
            get { return NullIfEmpty(_textBox.Text); }
            set { _textBox.Text = value; }
        }
        */
        [Category("Text Field")]
        public string Value
        {
            get 
            {
                return _textBox.IsNull == false ? 
                    (string)_textBox.Value : 
                    _textBox.NullTextReturnValue; 
            } 
            set 
            { 
                _textBox.Value = 
                    value == string.Empty ? null : value; 
            }
        }

        [DefaultValue(false)]
        public bool ReadOnly
        {
            get
            {
                return _textBox.ReadOnly;
            }
            set
            {
                _textBox.ReadOnly = value;
            }
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
        
        /// <summary>
        /// Set/Get the text field mask.   See System.Windows.Forms.MaskedTextBox.Mask for details on setting the Mask value
        /// </summary>
        /// <seealso cref="System.Windows.Forms.MaskedTextBox.Mask"/>
        [Category("Masked Text Field")]
        [Description("See System.Windows.Forms.MaskedTextBox.Mask Property")]
        public string Mask
        {
            get { return _textBox.EditMask; }
            set { _textBox.EditMask = value; }
        }

        // private static string NullIfEmpty(string value)
        // {
            // return (value != null && value.Length == 0) ? null : value;
        // }
    }
}
