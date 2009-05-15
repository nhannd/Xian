using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Synchronization.View.WinForms
{
	public partial class SynchronizationToolConfigComponentControl : UserControl, INotifyPropertyChanged
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private readonly SynchronizationToolConfigComponent _component;
		private string _toleranceAngle;

		public SynchronizationToolConfigComponentControl(SynchronizationToolConfigComponent component)
		{
			InitializeComponent();

			_component = component;
			_toleranceAngle = _component.ParallelPlanesToleranceAngle.ToString();
			_txtToleranceAngle.DataBindings.Add("Text", this, "ToleranceAngle", false, DataSourceUpdateMode.OnPropertyChanged);
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

		public string ToleranceAngle
		{
			get { return _toleranceAngle; }
			set
			{
				if (_toleranceAngle != value)
				{
					_toleranceAngle = value;
					this.NotifyPropertyChanged("ToleranceAngle");

					float fValue;
					if (float.TryParse(_toleranceAngle, out fValue))
					{
						if (fValue >= 0 && fValue <= 15)
						{
							_component.ParallelPlanesToleranceAngle = fValue;
							_errorProvider.SetError(_pnlToleranceAngleControl, string.Empty);
						}
						else
						{
							// deliberately set a value out of range, so that the component will fail internal range validation and refuse to exit
							_component.ParallelPlanesToleranceAngle = -1;
							_errorProvider.SetError(_pnlToleranceAngleControl, SR.ErrorAngleOutOfRange);
						}
					}
					else
					{
						// deliberately set a value out of range, so that the component will fail internal range validation and refuse to exit
						_component.ParallelPlanesToleranceAngle = -1;
						_errorProvider.SetError(_pnlToleranceAngleControl, SR.ErrorInvalidNumberFormat);
					}
				}
			}
		}
	}
}