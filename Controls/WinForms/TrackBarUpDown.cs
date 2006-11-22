using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Controls.WinForms
{
	public partial class TrackBarUpDown : UserControl
	{
		private bool _trackBarValueChanging;
		private bool _upDownValueChanging;

		public event EventHandler ValueChanged;
		public event EventHandler MinimumChanged;
		public event EventHandler MaximumChanged;


		public TrackBarUpDown()
		{
			InitializeComponent();

			_trackBar.ValueChanged += new EventHandler(OnTrackBarValueChanged);
			_numericUpDown.ValueChanged += new EventHandler(OnNumericUpDownValueChanged);
		}

		public int TrackBarIncrements
		{
			get { return _trackBar.Maximum; }
			set 
			{
				_trackBar.Maximum = value;
				_trackBar.TickFrequency = 10;
			}
		}

		public decimal Minimum
		{
			get { return _numericUpDown.Minimum; }
			set 
			{
				if (value > this.Maximum)
					throw new ArgumentException("Minimum must be less than maximum");

				if (_numericUpDown.Minimum != value)
				{
					_numericUpDown.Minimum = value;
					EventsHelper.Fire(this.MinimumChanged, this, EventArgs.Empty);
				}
			}
		}

		public decimal Maximum
		{
			get { return _numericUpDown.Maximum; }
			set
			{
				if (value < this.Minimum)
					throw new ArgumentException("Maximum must be greater than minimum");

				if (_numericUpDown.Maximum != value)
				{
					_numericUpDown.Maximum = value;
					EventsHelper.Fire(this.MaximumChanged, this, EventArgs.Empty);
				}
			}
		}

		public int DecimalPlaces
		{
			get { return _numericUpDown.DecimalPlaces; }
			set 
			{ 
				_numericUpDown.DecimalPlaces = value;
				_numericUpDown.Increment = (decimal) Math.Pow(10, -value);
			}
		}

		public decimal Value
		{
			get { return _numericUpDown.Value; }
			set 
			{
				if (_numericUpDown.Value != value)
				{
					_numericUpDown.Value = value;
					EventsHelper.Fire(this.ValueChanged, this, EventArgs.Empty);
				}
			}
		}

		private decimal Range
		{
			get { return this.Maximum - this.Minimum; }
		}

		void OnTrackBarValueChanged(object sender, EventArgs e)
		{
			if (!_trackBarValueChanging)
			{
				_trackBarValueChanging = true;

				decimal ratio = (decimal)_trackBar.Value / (decimal)this.TrackBarIncrements;
				decimal numericUpDownValue = ratio * this.Range + this.Minimum;
				this.Value = numericUpDownValue; // Math.Round(numericUpDownValue, this.DecimalPlaces);
				
				_trackBarValueChanging = false;
			}
		}

		void OnNumericUpDownValueChanged(object sender, EventArgs e)
		{
			if (!_upDownValueChanging)
			{
				_upDownValueChanging = true;

				decimal value = this.Value - this.Minimum;
				decimal ratio = value / this.Range;
				decimal trackBarValue = ratio * this.TrackBarIncrements;
				_trackBar.Value = (int)Math.Round(trackBarValue, 0);

				_upDownValueChanging = false;
				EventsHelper.Fire(this.ValueChanged, this, EventArgs.Empty);
			}
		}
	}
}
