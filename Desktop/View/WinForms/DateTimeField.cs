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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
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

        private void DateTimePickerValueChangedEventHandler(object sender, EventArgs e)
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
                if (!this.DesignMode)
                {
                    _dateTimePicker.Format = DateTimePickerFormat.Custom;
                    _dateTimePicker.CustomFormat = _showTime == true ? Format.DateTimeFormat : Format.DateFormat;
                }
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

		public DateTime Maximum
		{
			get { return _dateTimePicker.MaxDate; }
			set	{ _dateTimePicker.MaxDate = value; }
		}

		public DateTime Minimum
		{
			get { return _dateTimePicker.MinDate; }
			set { _dateTimePicker.MinDate = value; }
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

        private void DateTimeField_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            _dateTimePicker.Format = DateTimePickerFormat.Custom;
            _dateTimePicker.CustomFormat = Format.DateFormat;
        }
    }
}
