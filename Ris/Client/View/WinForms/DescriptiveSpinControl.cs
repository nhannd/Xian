#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public partial class DescriptiveSpinControl : UserControl
	{
		public DescriptiveSpinControl()
		{
			InitializeComponent();

			_numericUpDown.Maximum = int.MaxValue;
			_numericUpDown.Minimum = int.MinValue;
			_numericUpDown.ValueChanged += delegate { _textBox.Text = this.Format(_numericUpDown.Value); };

			// When the description is clicked, give away focus to the spin control.
			_textBox.Enter += delegate { _numericUpDown.Focus(); };

			// The textbox is always in read-only mode.  Therefore manually change colour with enablement.
			this.EnabledChanged += delegate { SetTextBoxBackColour(); };

			// Default formatting is to return the numeric value.
			this.Format = delegate(decimal value) { return value.ToString(); };

			// Set initial textBox back colour.
			SetTextBoxBackColour();
		}

		public Converter<decimal, string> Format { get; set; }

		[DefaultValue(int.MaxValue)]
		public decimal Maximum
		{
			get { return _numericUpDown.Maximum; }
			set { _numericUpDown.Maximum = value; }
		}

		[DefaultValue(int.MinValue)]
		public decimal Minimum
		{
			get { return _numericUpDown.Minimum; }
			set { _numericUpDown.Minimum = value; }
		}

		[DefaultValue(0)]
		public decimal Value
		{
			get { return _numericUpDown.Value; }
			set
			{
				_numericUpDown.Value = value;
				_textBox.Text = this.Format(value);
			}
		}

		public event EventHandler ValueChanged
		{
			add { _numericUpDown.ValueChanged += value; }
			remove { _numericUpDown.ValueChanged -= value; }
		}

		private void SetTextBoxBackColour()
		{
			_textBox.BackColor = this.Enabled ? SystemColors.Window : SystemColors.Control;
		}
	}
}
