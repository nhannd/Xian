#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using Timer = ClearCanvas.Common.Utilities.Timer;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal class LocalServerWatcher
	{
		public static LocalServerWatcher Instance { get; private set; }

		static LocalServerWatcher()
		{
			Instance = new LocalServerWatcher();
		}

		private class AsyncState
		{
			public SynchronizationContext SynchronizationContext;
			public StorageConfiguration CurrentStorageConfiguration;
			public DicomServerConfiguration CurrentDicomServerConfiguration;
		}

		private Timer _refreshTimer;

		private DicomServerConfiguration _dicomServerConfiguration;
		private StorageConfiguration _storageConfiguration;

		public event EventHandler DicomServerConfigurationChanged;
		public event EventHandler StudyStorageConfigurationChanged;
		public event EventHandler DiskSpaceUsageChanged;

		private LocalServerWatcher()
		{
			_refreshTimer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(20));
			_refreshTimer.Start();
		}

		//public void Start()
		//{
		//    if (_refreshTimer != null)
		//        return;

		//    _refreshTimer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(20));
		//    _refreshTimer.Start();
		//}

		//public void Dispose()
		//{
		//    if (_refreshTimer == null)
		//        return;

		//    _refreshTimer.Dispose();
		//    _refreshTimer = null;
		//}

		private DicomServerConfiguration DicomServerConfiguration
		{
			get { return _dicomServerConfiguration ?? (_dicomServerConfiguration = DicomServer.GetConfiguration()); }
		}

		private StorageConfiguration StorageConfiguration
		{
			get { return _storageConfiguration ?? (_storageConfiguration = StudyStore.GetConfiguration()); }
		}

		#region Dicom Server

		public string AETitle
		{
			get { return DicomServerConfiguration.AETitle; }
		}

		public string HostName
		{
			get { return DicomServerConfiguration.HostName; }
		}

		public int Port
		{
			get { return DicomServerConfiguration.Port; }
		}

		#endregion

		#region Study Storage Configuration

		public string FileStoreDirectory
		{
			get { return StorageConfiguration.FileStoreDirectory; }
		}

		//TODO (Marmot): at some point, we may want to show this on the meter.
		//public double MinimumFreeSpacePercent
		//{
		//    get { return StorageConfiguration.MinimumFreeSpacePercent; }
		//}

		public bool IsMaximumDiskspaceUsageExceeded
		{
			get { return StorageConfiguration.IsMaximumUsedSpaceExceeded; }
		}

		#endregion

		#region Disk Space

		public long DiskSpaceTotal
		{
			get { return StorageConfiguration.FileStoreDiskSpace.TotalSpace; }
		}

		public long DiskSpaceUsed
		{
			get { return StorageConfiguration.FileStoreDiskSpace.UsedSpace; }
		}

		public double DiskSpaceUsedPercent
		{
			get { return StorageConfiguration.FileStoreDiskSpace.UsedSpacePercent; }
		}

		#endregion

		private void OnTimerElapsed(object state)
		{
			var asyncState = new AsyncState
			{
				SynchronizationContext = SynchronizationContext.Current,
				CurrentDicomServerConfiguration = _dicomServerConfiguration,
				CurrentStorageConfiguration = _storageConfiguration
			};

			ThreadPool.QueueUserWorkItem(UpdateConfigurationsAsync, asyncState);
		}

		private void UpdateConfigurationsAsync(object state)
		{
			var asyncState = (AsyncState)state;
			//This stuff can actually add up over time because it's hitting the database so frequently.
			//Better to do it asynchronously.
			var storageConfiguration = StudyStore.GetConfiguration();
			var dicomServerConfiguration = DicomServer.GetConfiguration();

			if (!Equals(asyncState.CurrentDicomServerConfiguration, dicomServerConfiguration))
				asyncState.SynchronizationContext.Post(ignore => NotifyDicomServerConfigurationChanged(dicomServerConfiguration), null);

			var storageConfigurationChanged = HasStorageConfigurationChanged(asyncState.CurrentStorageConfiguration, storageConfiguration);
			//Access all the disk usage properties here, since they can take some time.
			bool diskUsageChanged = HasDiskUsageChanged(asyncState.CurrentStorageConfiguration, storageConfiguration);

			if (storageConfigurationChanged)
				asyncState.SynchronizationContext.Post(ignore => NotifyStorageConfigurationChanged(storageConfiguration), null);
			else if (diskUsageChanged)
				asyncState.SynchronizationContext.Post(ignore => NotifyDiskUsageChanged(storageConfiguration), null);
		}

		private bool HasStorageConfigurationChanged(StorageConfiguration old, StorageConfiguration @new)
		{
			return old == null
				   || old.FileStoreDirectory != @new.FileStoreDirectory
				   || Math.Abs(old.MinimumFreeSpacePercent - @new.MinimumFreeSpacePercent) > 0.0001;
		}

		private bool HasDiskUsageChanged(StorageConfiguration old, StorageConfiguration @new)
		{
			//We don't want to cause updates when the disk usage has changed non-significantly.
			return old == null
				   || Math.Abs(old.FileStoreDiskSpace.UsedSpacePercent - @new.FileStoreDiskSpace.UsedSpacePercent) > 0.0001
				   || old.FileStoreDiskSpace.TotalSpace != @new.FileStoreDiskSpace.TotalSpace;
		}

		private void NotifyDicomServerConfigurationChanged(DicomServerConfiguration dicomServerConfiguration)
		{
			if (_refreshTimer == null)
				return;

			_dicomServerConfiguration = dicomServerConfiguration;
			EventsHelper.Fire(DicomServerConfigurationChanged, this, EventArgs.Empty);
		}

		private void NotifyStorageConfigurationChanged(StorageConfiguration storageConfiguration)
		{
			if (_refreshTimer == null)
				return;

			_storageConfiguration = storageConfiguration;
			EventsHelper.Fire(StudyStorageConfigurationChanged, this, EventArgs.Empty);
			//The file store directory may have changed, so just update this, too.
			EventsHelper.Fire(DiskSpaceUsageChanged, this, EventArgs.Empty);
		}

		private void NotifyDiskUsageChanged(StorageConfiguration storageConfiguration)
		{
			if (_refreshTimer == null)
				return;

			//We still reassign this value, even if it's only the disk usage that's changed because
			//the DiskSpace class caches its values, so we need to swap it out for a new one.
			_storageConfiguration = storageConfiguration;
			EventsHelper.Fire(DiskSpaceUsageChanged, this, EventArgs.Empty);
		}
	}
}
