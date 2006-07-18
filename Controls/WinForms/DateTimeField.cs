using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Controls.WinForms
{
    public partial class DateTimeField : UserControl
    {
        private bool _nullable = false;
        private event EventHandler _valueChanged;

        public DateTimeField()
        {
            InitializeComponent();

            _checkBox.CheckedChanged += new EventHandler(_checkBox_CheckedChanged);
            _dateTimePicker.ValueChanged += new EventHandler(_dateTimePicker_ValueChanged);
        }

        private void  _dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            FireValueChanged();
        }

        private void _checkBox_CheckedChanged(object sender, EventArgs e)
        {
            _dateTimePicker.Enabled = _checkBox.Checked;
            FireValueChanged();
        }

        private void FireValueChanged()
        {
            if(_valueChanged != null)
            {
                _valueChanged(this, new EventArgs());
            }
        }

        public bool Nullable
        {
            get { return _nullable; }
            set
            {
                _nullable = value;
                _label.Visible = !_nullable;
                _checkBox.Visible = _nullable;
            }
        }

        public string LabelText
        {
            get { return _label.Text; }
            set { _label.Text = _checkBox.Text = value; }
        }

        public DateTime? Value
        {
            get { return _dateTimePicker.Value; }
            set
            {
                if (_dateTimePicker.Value != value)
                {
                    if (value != null)
                        _dateTimePicker.Value = (DateTime)value;
                    else
                        _checkBox.Checked = false;
                }
            }
        }

        public event EventHandler ValueChanged
        {
            add { _valueChanged += value; }
            remove { _valueChanged -= value; }
        }
    }
}
