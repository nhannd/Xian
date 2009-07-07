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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Synchronization.View.WinForms
{
	public partial class SynchronizationToolConfigurationComponentControl : UserControl, INotifyPropertyChanged
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private readonly SynchronizationToolConfigurationComponent _component;
		private string _toleranceAngle;

		public SynchronizationToolConfigurationComponentControl(SynchronizationToolConfigurationComponent component)
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