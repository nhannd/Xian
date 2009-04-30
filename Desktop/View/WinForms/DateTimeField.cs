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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class DateTimeField : UserControl
    {
        private bool _showTime = false;
        private bool _showDate = true;
        private event EventHandler _valueChanged;

        public DateTimeField()
        {
            InitializeComponent();

            _dateTimePicker.ValueChanged += new EventHandler(DateTimePickerValueChangedEventHandler);
        }

        private void DateTimePickerValueChangedEventHandler(object sender, EventArgs e)
        {
            FireValueChanged();
        }

        private void FireValueChanged()
        {
            EventsHelper.Fire(_valueChanged, this, EventArgs.Empty);
        }

        [DefaultValue(false)]
        public bool Nullable
        {
            get { return _dateTimePicker.ShowCheckBox; }
            set
            {
                _dateTimePicker.ShowCheckBox = value;
            }
        }

        [DefaultValue(true)]
        public bool ShowDate
        {
            get { return _showDate; }
            set
            {
                _showDate = value;
                UpdateFormat();
            }
        }

        [DefaultValue(false)]
        public bool ShowTime
        {
            get { return _showTime; }
            set 
            { 
                _showTime = value;
                UpdateFormat();
            }
        }

        public string LabelText
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        public DateTime? Value
        {
            get
            {
                return _dateTimePicker.Checked ? (DateTime?)_dateTimePicker.Value : null;
            }
            set
            {
                bool isNull = TestNull(value);
                if (!isNull)
                {
                    _dateTimePicker.Value = (DateTime)value;
                }
                else
                {
                    // can't set the value (will get an exception)
                }

                _dateTimePicker.Checked = !isNull;
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

            UpdateFormat();
        }

        private void UpdateFormat()
        {
            // if this is only a time control, use a spin control
            _dateTimePicker.ShowUpDown = (_showTime && !_showDate);

            if (this.DesignMode)
                return;

            // set the display format
            _dateTimePicker.Format = DateTimePickerFormat.Custom;
            if (_showDate && _showTime)
                _dateTimePicker.CustomFormat = Format.DateTimeFormat;
            else if (_showDate)
                _dateTimePicker.CustomFormat = Format.DateFormat;
            else if (_showTime)
                _dateTimePicker.CustomFormat = Format.TimeFormat;
            else
                _dateTimePicker.CustomFormat = "";
        }

    }
}
