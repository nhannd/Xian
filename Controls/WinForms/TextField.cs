using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Controls.WinForms
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
