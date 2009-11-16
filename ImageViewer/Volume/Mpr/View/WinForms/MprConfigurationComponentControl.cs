using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Volume.Mpr.Configuration;

namespace ClearCanvas.ImageViewer.Volume.Mpr.View.WinForms
{
	public partial class MprConfigurationComponentControl : UserControl, INotifyPropertyChanged
	{
		private PropertyChangedEventHandler _propertyChanged;
		private MprConfigurationComponent _component;
		private string _sliceSpacingFactor;

		public MprConfigurationComponentControl(MprConfigurationComponent component)
		{
			InitializeComponent();

			_component = component;

			_sliceSpacingFactor = _component.SliceSpacingFactor.ToString();

			_txtProportionalSliceSpacing.DataBindings.Add("Text", this, "SliceSpacingFactor", true, DataSourceUpdateMode.OnPropertyChanged);
			_txtProportionalSliceSpacing.DataBindings.Add("Enabled", this, "ProportionalSliceSpacing", true, DataSourceUpdateMode.OnPropertyChanged);
			_radAutomaticSliceSpacing.DataBindings.Add("Checked", this, "AutomaticSliceSpacing", false, DataSourceUpdateMode.OnPropertyChanged);
			_radProportionalSliceSpacing.DataBindings.Add("Checked", this, "ProportionalSliceSpacing", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));

			if (propertyName == "ProportionalSliceSpacing")
			{
				// this block fixes the validation failure if the user inputs invalid data in the text box
				// then toggles the radio button selection to automatic

				if (this.ProportionalSliceSpacing)
				{
					// force trigger the code to set error messages in the GUI
					string sliceSpacingFactor = this.SliceSpacingFactor;
					this.SliceSpacingFactor = string.Empty;
					this.SliceSpacingFactor = sliceSpacingFactor;
				}
				else
				{
					// clear any errors that may be in the component
					if (!string.IsNullOrEmpty(_errorProvider.GetError(_pnlProportionalSliceSpacing)))
					{
						_component.SliceSpacingFactor = 1f;
						_errorProvider.SetError(_pnlProportionalSliceSpacing, string.Empty);
					}
				}
			}
		}

		public bool AutomaticSliceSpacing
		{
			get { return _component.AutoSliceSpacing; }
			set
			{
				if (_component.AutoSliceSpacing != value)
				{
					_component.AutoSliceSpacing = value;
					this.NotifyPropertyChanged("AutomaticSliceSpacing");
					this.NotifyPropertyChanged("ProportionalSliceSpacing");
				}
			}
		}

		public bool ProportionalSliceSpacing
		{
			get { return !_component.AutoSliceSpacing; }
			set
			{
				if (!_component.AutoSliceSpacing != value)
				{
					_component.AutoSliceSpacing = !value;
					this.NotifyPropertyChanged("AutomaticSliceSpacing");
					this.NotifyPropertyChanged("ProportionalSliceSpacing");
				}
			}
		}

		public string SliceSpacingFactor
		{
			get { return _sliceSpacingFactor; }
			set
			{
				if (_sliceSpacingFactor != value)
				{
					_sliceSpacingFactor = value;
					this.NotifyPropertyChanged("SliceSpacingFactor");

					float fValue;
					if (float.TryParse(_sliceSpacingFactor, out fValue))
					{
						if (fValue > 0 && fValue <= 5)
						{
							_component.SliceSpacingFactor = fValue;
							_errorProvider.SetError(_pnlProportionalSliceSpacing, string.Empty);
						}
						else
						{
							// deliberately set a value out of range, so that the component will fail internal range validation and refuse to exit
							_component.SliceSpacingFactor = -1;
							_errorProvider.SetError(_pnlProportionalSliceSpacing, SR.ErrorSliceSpacingOutOfRange);
						}
					}
					else
					{
						// deliberately set a value out of range, so that the component will fail internal range validation and refuse to exit
						_component.SliceSpacingFactor = -1;
						_errorProvider.SetError(_pnlProportionalSliceSpacing, SR.ErrorInvalidNumberFormat);
					}
				}
			}
		}
	}
}