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
using System.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	public sealed class SynchronizationToolSettingsHelper : INotifyPropertyChanged
	{
		public static readonly SynchronizationToolSettingsHelper Default = new SynchronizationToolSettingsHelper(SynchronizationToolSettings.Default);

		private event PropertyChangedEventHandler _propertyChanged;
		private readonly SynchronizationToolSettings _settings;
		private float _parallelPlanesToleranceAngleRadians = 0f;
		private bool _suspendSettingChangingEvent = false;

		private SynchronizationToolSettingsHelper(SynchronizationToolSettings settings)
		{
			_settings = settings;
			_settings.SettingChanging += OnSettingChanged;
			_parallelPlanesToleranceAngleRadians = (float) (_settings.ParallelPlanesToleranceAngle*Math.PI/180f);
		}

		private void OnSettingChanged(object sender, SettingChangingEventArgs e)
		{
			if (!_suspendSettingChangingEvent && e.SettingName == "ParallelPlanesToleranceAngle")
			{
				float value = (float) e.NewValue;
				_parallelPlanesToleranceAngleRadians = (float) (value*Math.PI/180f);
				this.NotifyPropertyChanged("ParallelPlanesToleranceAngleRadians");
				this.NotifyPropertyChanged("ParallelPlanesToleranceAngleDegrees");
			}
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		/// <summary>
		/// Gets or sets the maximum angle difference, in degrees, between two planes for synchronization tools to treat the planes as parallel.
		/// </summary>
		/// <seealso cref="ParallelPlanesToleranceAngleRadians"/>
		public float ParallelPlanesToleranceAngleDegrees
		{
			get { return _settings.ParallelPlanesToleranceAngle; }
			set
			{
				if (this.ParallelPlanesToleranceAngleDegrees != value)
				{
					_suspendSettingChangingEvent = true;
					try
					{
						_settings.ParallelPlanesToleranceAngle = value;
					}
					finally
					{
						_suspendSettingChangingEvent = false;
					}
					this.NotifyPropertyChanged("ParallelPlanesToleranceAngleDegrees");
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum angle difference, in radians, between two planes for synchronization tools to treat the planes as parallel.
		/// </summary>
		/// <seealso cref="ParallelPlanesToleranceAngleDegrees"/>
		public float ParallelPlanesToleranceAngleRadians
		{
			get { return _parallelPlanesToleranceAngleRadians; }
			set
			{
				if (_parallelPlanesToleranceAngleRadians != value)
				{
					_parallelPlanesToleranceAngleRadians = value;

					_suspendSettingChangingEvent = true;
					try
					{
						_settings.ParallelPlanesToleranceAngle = (float) (value*180f/Math.PI);
					}
					finally
					{
						_suspendSettingChangingEvent = false;
					}

					this.NotifyPropertyChanged("ParallelPlanesToleranceAngleRadians");
				}
			}
		}
	}
}