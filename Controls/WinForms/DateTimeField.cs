using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop;

namespace ClearCanvas.Controls.WinForms
{
    public partial class DateTimeField : UserControl
    {
        private bool _nullable = false;
        private bool _showTime = false;
        private event EventHandler _valueChanged;

        public DateTimeField()
        {
            InitializeComponent();

            _checkBox.CheckedChanged += new EventHandler(CheckBoxCheckedChangedEventHandler);
            _dateTimePicker.ValueChanged += new EventHandler(DateTimePickerValueChangedEventHandler);
        }

        private void  DateTimePickerValueChangedEventHandler(object sender, EventArgs e)
        {
            FireValueChanged();
        }

        private void CheckBoxCheckedChangedEventHandler(object sender, EventArgs e)
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
                _dateTimePicker.Enabled = _checkBox.Checked;
            }
        }

        public bool ShowTime
        {
            get { return _showTime; }
            set 
            { 
                _showTime = value;
                _dateTimePicker.Format = DateTimePickerFormat.Custom;
                _dateTimePicker.CustomFormat = _showTime == true ? Format.DateTimeFormat : Format.DateFormat;
            }
        }

        public string LabelText
        {
            get { return _label.Text; }
            set { _label.Text = _checkBox.Text = value; }
        }

        public DateTime? Value
        {
            get
            {
                return _checkBox.Checked ? (DateTime?)_dateTimePicker.Value : null;
            }
            set
            {
                if (!TestNull(value))
                    _dateTimePicker.Value = (DateTime)value;

                _checkBox.Checked = !TestNull(value);
            }
        }

        public event EventHandler ValueChanged
        {
            add { _valueChanged += value; }
            remove { _valueChanged -= value; }
        }

        private static bool TestNull(object value)
        {
            return value == null || value == System.DBNull.Value;
        }
    }
}
