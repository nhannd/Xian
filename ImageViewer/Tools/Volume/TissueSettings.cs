using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public class TissueSettings : INotifyPropertyChanged
	{
		private bool _tissueVisible = true;

		private decimal _opacityMinValue = 0.0M;
		private decimal _opacityMaxValue = 1.0M;
		private decimal _opacityValue = 0.8M;

		private decimal _windowMinValue = 1;
		private decimal _windowMaxValue = 5000;
		private decimal _windowValue = 400;

		private decimal _levelMinValue = -2000;
		private decimal _levelMaxValue = 3000;
		private decimal _levelValue = 200;

		public TissueSettings()
		{
		}

		public bool TissueVisible
		{
			get { return _tissueVisible; }
			set
			{
				if (_tissueVisible != value)
				{
					_tissueVisible = value;
					OnPropertyChanged("TissueVisible");
				}
			}
		}

		#region Opacity properties


		public decimal OpacityMinValue
		{
			get { return _opacityMinValue; }
			set
			{
				if (_opacityMinValue != value)
				{
					_opacityMinValue = value;
					OnPropertyChanged("OpacityMinValue");
				}
			}
		}

		public decimal OpacityMaxValue
		{
			get { return _opacityMaxValue; }
			set
			{
				if (_opacityMaxValue != value)
				{
					_opacityMaxValue = value;
					OnPropertyChanged("OpacityMaxValue");
				}
			}
		}

		public decimal OpacityValue
		{
			get { return _opacityValue; }
			set
			{
				if (_opacityValue != value)
				{
					_opacityValue = value;
					OnPropertyChanged("OpacityValue");
				}
			}
		}

		#endregion

		#region Window properties

		public decimal WindowMinValue
		{
			get { return _windowMinValue; }
			set
			{
				if (_windowMinValue != value)
				{
					_windowMinValue = value;
					OnPropertyChanged("WindowMinValue");
				}
			}
		}

		public decimal WindowMaxValue
		{
			get { return _windowMaxValue; }
			set
			{
				if (_windowMaxValue != value)
				{
					_windowMaxValue = value;
					OnPropertyChanged("WindowMaxValue");
				}
			}
		}

		public decimal WindowValue
		{
			get { return _windowValue; }
			set
			{
				if (_windowValue != value)
				{
					_windowValue = value;
					OnPropertyChanged("WindowValue");
				}
			}
		}

		#endregion

		#region Level properties

		public decimal LevelMinValue
		{
			get { return _levelMinValue; }
			set
			{
				if (_levelMinValue != value)
				{
					_levelMinValue = value;
					OnPropertyChanged("LevelMinValue");
				}
			}
		}

		public decimal LevelMaxValue
		{
			get { return _levelMaxValue; }
			set
			{
				if (_levelMaxValue != value)
				{
					_levelMaxValue = value;
					OnPropertyChanged("LevelMaxValue");
				}
			}
		}

		public decimal LevelValue
		{
			get { return _levelValue; }
			set
			{
				if (_levelValue != value)
				{
					_levelValue = value;
					OnPropertyChanged("LevelValue");
				}
			}
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(
				  this,
				  new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
