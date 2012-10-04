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
using System.Runtime.InteropServices;
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
        }

        private void FireValueChanged()
        {
        	// This is the fix for #7632
        	// There is an old (circa .NET 1) bug in the WinForms DateTimePicker control wrapper
        	// This bug causes the ValueChanged event of the control to not always fire (particularly when the user clicks the checkbox to clear the value)
        	// Fortunately, the bug is in the wrapper, not the underlying MFC control, so we manually trigger the event when we:
        	// 1. detect the WM_NOTIFY message which tells us when the UI initiates a value change
        	// 2. detect when our Value property setter changes the value through code
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
            	var oldValue = Value;

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

            	// trigger ValueChanged event manually if the value changed here
            	if (!oldValue.Equals(value))
            		FireValueChanged();
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

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			// perform special handling if it's a WM_NOTIFY message from the date time picker
			if (m.Msg == (int) WindowsMessages.WM_NOTIFY && m.WParam == _dateTimePicker.Handle)
			{
				// if the NMHDR indicates a UI-initiated value change, trigger our ValueChanged event
				const uint DTN_DATETIMECHANGE = 0xFFFFFD09;
				var nmhdr = (NMHDR) m.GetLParam(typeof (NMHDR));
				if (nmhdr.code == DTN_DATETIMECHANGE)
					FireValueChanged();
			}
		}

		#region NMHDR Struct

		// ReSharper disable InconsistentNaming

		/// <summary>
		/// Base lParam structure for WM_NOTIFY messages.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct NMHDR
		{
			public IntPtr hwndFrom;
			public uint idFrom;
			public uint code;
		}

		// ReSharper restore InconsistentNaming 

		#endregion
    }
}
