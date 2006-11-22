using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public class TissueSettings : INotifyPropertyChanged
	{
		private bool _tissueVisible = true;

		private decimal _minimumOpacity = 0.0M;
		private decimal _maximumOpacity = 1.0M;
		private decimal _opacity = 1.0M;

		private decimal _minimumWindow = 1;
		private decimal _maximumWindow = 5000;
		private decimal _window = 500;

		private decimal _minimumLevel = -2000;
		private decimal _maximumLevel = 3000;
		private decimal _level = 400;

		private Color _minimumColor;
		private Color _maximumColor;

		private string[] _presets = {"Bone", "Blood", "Muscle", "Soft", "Lung"};
		private string _selectedPreset;

		public TissueSettings()
		{
		}

		public string[] Presets
		{
			get { return _presets; }
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


		public decimal MinimumOpacity
		{
			get { return _minimumOpacity; }
			set
			{
				if (_minimumOpacity != value)
				{
					_minimumOpacity = value;
					OnPropertyChanged("MinimumOpacity");
				}
			}
		}

		public decimal MaximumOpacity
		{
			get { return _maximumOpacity; }
			set
			{
				if (_maximumOpacity != value)
				{
					_maximumOpacity = value;
					OnPropertyChanged("MaximumOpacity");
				}
			}
		}

		public decimal Opacity
		{
			get { return _opacity; }
			set
			{
				if (_opacity != value)
				{
					_opacity = value;
					OnPropertyChanged("Opacity");
				}
			}
		}

		#endregion

		#region Window properties

		public decimal MinimumWindow
		{
			get { return _minimumWindow; }
			set
			{
				if (_minimumWindow != value)
				{
					_minimumWindow = value;
					OnPropertyChanged("MinimumWindow");
				}
			}
		}

		public decimal MaximumWindow
		{
			get { return _maximumWindow; }
			set
			{
				if (_maximumWindow != value)
				{
					_maximumWindow = value;
					OnPropertyChanged("MaximumWindow");
				}
			}
		}

		public decimal Window
		{
			get { return _window; }
			set
			{
				if (_window != value)
				{
					_window = value;
					OnPropertyChanged("Window");
				}
			}
		}

		#endregion

		#region Level properties

		public decimal MinimumLevel
		{
			get { return _minimumLevel; }
			set
			{
				if (_minimumLevel != value)
				{
					_minimumLevel = value;
					OnPropertyChanged("MinimumLevel");
				}
			}
		}

		public decimal MaximumLevel
		{
			get { return _maximumLevel; }
			set
			{
				if (_maximumLevel != value)
				{
					_maximumLevel = value;
					OnPropertyChanged("MaximumLevel");
				}
			}
		}

		public decimal Level
		{
			get { return _level; }
			set
			{
				if (_level != value)
				{
					_level = value;
					OnPropertyChanged("Level");
				}
			}
		}

		#endregion

		#region Color properties

		public Color MinimumColor
		{
			get { return _minimumColor; }
			set 
			{
				if (_minimumColor != value)
				{
					_minimumColor = value;
					OnPropertyChanged("MinimumColor");
				}
			}
		}

		public Color MaximumColor
		{
			get { return _maximumColor; }
			set
			{
				if (_maximumColor != value)
				{
					_maximumColor = value;
					OnPropertyChanged("MaximumColor");
				}
			}
		}

		#endregion

		public string SelectedPreset
		{
			get { return _selectedPreset; }
			set
			{
				if (_selectedPreset != value)
				{
					_selectedPreset = value;
					OnPropertyChanged("SelectedPreset");
				}
			}
		}

		public void SelectPreset(string preset)
		{
			this.SelectedPreset = preset;

			if (preset == "Bone")
			{
				this.Opacity = 1.0M;
				this.Window = 500;
				this.Level = 400;
				this.MinimumColor = Color.White;
				this.MaximumColor = Color.White;
			}
			else if (preset == "Blood")
			{
				this.Opacity = 0.2M;
				this.Window = 200;
				this.Level = 220;
				this.MinimumColor = Color.FromArgb(200, 4, 10);
				this.MaximumColor = Color.FromArgb(255, 255, 128);
			}
			else if (preset == "Muscle")
			{
				this.Opacity = 0.8M;
				this.Window = 200;
				this.Level = 100;
				this.MinimumColor = Color.FromArgb(233, 90, 94);
				this.MaximumColor = Color.FromArgb(233, 90, 94);
			}
			else if (preset == "Soft")
			{
				this.Opacity = 0.8M;
				this.Window = 500;
				this.Level = -240;
				this.MinimumColor = Color.FromArgb(251, 138, 96);
				this.MaximumColor = Color.FromArgb(251, 138, 96);
			}
			else
			{
				this.Opacity = 0.5M;
				this.Window = 600;
				this.Level = -500;
				this.MinimumColor = Color.FromArgb(254, 142, 126);
				this.MaximumColor = Color.FromArgb(254, 142, 126);
			}
		}

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
