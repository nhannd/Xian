#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Configuration
{
	internal class SettingsStoreWatcher : IDisposable
	{
		private Timer _timer;
		private bool _isStoreOnline;
		private event EventHandler _isStoreOnlineChanged;

		public SettingsStoreWatcher()
		{
			UpdateIsStoreOnline();
		}

		public bool IsStoreOnline
		{
			get
			{
				UpdateIsStoreOnline();
				return _isStoreOnline;
			}
		}
		
		public event EventHandler IsStoreOnlineChanged
		{
			add
			{
				if (_isStoreOnlineChanged == null)
					StartWatching();

				_isStoreOnlineChanged += value;
			}
            remove
            {
            	_isStoreOnlineChanged -= value;
            	
				if (_isStoreOnlineChanged == null)
					StopWatching();
            }
		}

		private void StopWatching()
		{
			StopTimer();
		}

		private void StartWatching()
		{
			if (!SettingsStore.IsSupported)
				return;

			UpdateIsStoreOnline();
			StartTimer();
		}

		private void UpdateIsStoreOnline()
		{
			bool current = _isStoreOnline;
			_isStoreOnline = SettingsStore.IsStoreOnline;
			if (_isStoreOnline != current)
				EventsHelper.Fire(_isStoreOnlineChanged, this, EventArgs.Empty);
		}

		private void StartTimer()
		{
			if (_timer != null)
				return;

			_timer = new Timer(unused => UpdateIsStoreOnline(), null, TimeSpan.FromSeconds(5));
			_timer.Start();
		}

		private void StopTimer()
		{
			if (_timer == null)
				return;

			_timer.Stop();
			_timer.Dispose();
			_timer = null;
		}

		#region IDisposable Members

		public void Dispose()
		{
			StopTimer();
		}

		#endregion
	}
}
