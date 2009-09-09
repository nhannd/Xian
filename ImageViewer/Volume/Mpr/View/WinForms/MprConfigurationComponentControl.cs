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

			_txtSliceSpacing.DataBindings.Add("Text", this, "SliceSpacingFactor", true, DataSourceUpdateMode.OnPropertyChanged);
			_chkAutoSliceSpacing.DataBindings.Add("Checked", component, "AutoSliceSpacing", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _chkAutoSliceSpacing_CheckedChanged(object sender, EventArgs e)
		{
			_txtSliceSpacing.Enabled = !_chkAutoSliceSpacing.Checked;
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
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
						if (fValue >= float.Epsilon && fValue <= 5)
						{
							_component.SliceSpacingFactor = fValue;
							_errorProvider.SetError(_txtSliceSpacing, string.Empty);
						}
						else
						{
							// deliberately set a value out of range, so that the component will fail internal range validation and refuse to exit
							_component.SliceSpacingFactor = -1;
							_errorProvider.SetError(_txtSliceSpacing, SR.ErrorSliceSpacingOutOfRange);
						}
					}
					else
					{
						// deliberately set a value out of range, so that the component will fail internal range validation and refuse to exit
						_component.SliceSpacingFactor = -1;
						_errorProvider.SetError(_txtSliceSpacing, SR.ErrorInvalidNumberFormat);
					}
				}
			}
		}
	}
}