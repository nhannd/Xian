using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Controls.WinForms
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

        private static string NullIfEmpty(string value)
        {
            return (value != null && value.Length == 0) ? null : value;
        }
    }
}
