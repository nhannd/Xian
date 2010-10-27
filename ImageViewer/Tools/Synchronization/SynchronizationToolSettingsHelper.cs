#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		private volatile float _parallelPlanesToleranceAngleRadians = -1;

		private SynchronizationToolSettingsHelper(SynchronizationToolSettings settings)
		{
			_settings = settings;
			_settings.PropertyChanged += OnSettingChanged;
		}

		private void OnSettingChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "ParallelPlanesToleranceAngle")
				return;
			
			_parallelPlanesToleranceAngleRadians = -1;
			NotifyPropertyChanged("ParallelPlanesToleranceAngleRadians");
			NotifyPropertyChanged("ParallelPlanesToleranceAngleDegrees");
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
		/// Gets the maximum angle difference, in degrees, between two planes for synchronization tools to treat the planes as parallel.
		/// </summary>
		/// <seealso cref="ParallelPlanesToleranceAngleRadians"/>
		public float ParallelPlanesToleranceAngleDegrees
		{
			get
			{
				var value = _settings.ParallelPlanesToleranceAngle;
				return value == 0F ? 100*float.Epsilon : value;
			}
		}

		/// <summary>
		/// Gets the maximum angle difference, in radians, between two planes for synchronization tools to treat the planes as parallel.
		/// </summary>
		/// <seealso cref="ParallelPlanesToleranceAngleDegrees"/>
		public float ParallelPlanesToleranceAngleRadians
		{
			get
			{
				if (_parallelPlanesToleranceAngleRadians < 0)
					_parallelPlanesToleranceAngleRadians = ParallelPlanesToleranceAngleDegrees*(float)Math.PI/180F;
				return _parallelPlanesToleranceAngleRadians;
			}
		}
	}
}