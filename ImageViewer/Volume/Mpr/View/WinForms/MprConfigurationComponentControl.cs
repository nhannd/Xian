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