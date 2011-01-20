#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class TimestampField : UserControl
    {
        public event EventHandler ValueChanged;

        public TimestampField()
        {
            InitializeComponent();
            _datePicker.ValueChanged += OnValueChanged;
            _timePicker.ValueChanged += OnValueChanged;

            if (!DesignMode)
            {
                try
                {
                    _datePicker.CustomFormat = Format.DateFormat;
                    _timePicker.CustomFormat = Format.TimeFormat;
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Unable to set custom date/time format");
                }
            }
        }

        [Browsable(true)]
        [Category("Label")]
        public string TextLabel
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value == string.Empty ? null : value;
            }
        }

        public DateTime MaxValue
        {
            get { return _datePicker.MaxDate; }
            set { _datePicker.MaxDate = value; }
        }

        public DateTime MinValue
        {
            get { return _datePicker.MinDate; }
            set { _datePicker.MinDate = value; }
        }

        public DateTime? Value
        {
            get
            {
                if (!_datePicker.Checked)
                    return null;

                return _datePicker.Value.Date + _timePicker.Value.TimeOfDay;
                
            }

            set
            {
                _datePicker.Checked = value != null;
                if (value != null)
                {
                    _datePicker.Value = value.Value;
                    _timePicker.Value = value.Value;
                    _timePicker.Enabled = true;
                }
                else
                {
                    _timePicker.Enabled = false;
                }

                if (ValueChanged != null)
                    EventsHelper.Fire(ValueChanged, this, EventArgs.Empty);
            }
        }

        private void OnDatePickerMouseUp(object sender, MouseEventArgs e)
        {
            UpdateState();
        }

        private void UpdateState()
        {
            if (!_datePicker.Checked)
            {
                Value = null;
            } 
            else
            {
                Value = _datePicker.Value.Date + _timePicker.Value.TimeOfDay;
            }
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            UpdateState();
        }
    }
}
