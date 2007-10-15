#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;
using System.Management;
using System.Threading;

using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.ImageViewer.Services.DiskspaceManager;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.DataStore;
using System.Collections.ObjectModel;
using System.ServiceModel;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    internal sealed class DiskspaceManagerProcessor : IDiskspaceManagerService
    {
		private class StudySortComparer : IComparer<IStudy>
		{
			public StudySortComparer()
			{
			}

			#region IComparer<IStudy> Members

			public int Compare(IStudy x, IStudy y)
			{
				Study xStudy = x as Study;
				Study yStudy = y as Study;

				if (xStudy == null)
				{
					if (yStudy == null)
						return 0;
					else
						return -1;
				}
				else
				{
					if (yStudy == null)
						return 1;
				}

				return (xStudy.StoreTime < yStudy.StoreTime) ? -1 : 1;
			}

			#endregion
		}

		#region Private Fields

		private static DiskspaceManagerProcessor _instance;

		private Thread _processingThread;
		private volatile bool _stop;

		private object _settingsSyncLock = new object();
		private DMDriveInfo _currentDriveInfo;
		private int _checkingFrequency;
		private bool _settingsChanged;

		#endregion

		public DiskspaceManagerProcessor()
        {
        }

        public static DiskspaceManagerProcessor Instance
		{
			get
			{
				if (_instance == null)
                    _instance = new DiskspaceManagerProcessor();

				return _instance;
			}
            set
            {
                _instance = value;
            }
		}

        public void StartProcessor()
        {
			_stop = false;
			// start up processing thread
			_processingThread = new Thread(new ThreadStart(StartDiskspaceManager));
			_processingThread.IsBackground = true;
			_processingThread.Priority = ThreadPriority.BelowNormal;
			_processingThread.Start();
        }

        public void StopProcessor()
        {
			_stop = true;
			lock (_settingsSyncLock)
			{
				Monitor.Pulse(_settingsSyncLock);
			}
			_processingThread.Join();
			_processingThread = null;
        }

		private void StartDiskspaceManager()
		{
			do
			{
				try
				{
					int waitTime = 0;

					try
					{
						UpdateCurrentDriveInfo();
						CheckConfigurationSettings();
					}
					catch (EndpointNotFoundException)
					{
						//there is currently no shred startup order, so we wait 
						//5 seconds until the Local Data Store service is up and running.
						waitTime = 5000;
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
					}

					lock (_settingsSyncLock)
					{
						if (waitTime == 0)
							waitTime = _checkingFrequency;

						if (!_stop)
							Monitor.Wait(_settingsSyncLock, waitTime);
					}

					if (_stop)
						break;

					if (_settingsChanged || _currentDriveInfo == null)
						continue;

					CheckDiskspace();

					if (_currentDriveInfo.BytesOverHighWaterMark > 0)
					{
						RemoveStudies();
						CheckDiskspace();
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}
			while (!_stop);
		}

		private void CheckDiskspace()
		{
			_currentDriveInfo.Refresh();

			Platform.Log(LogLevel.Info, String.Format(SR.FormatCheckUsage, _currentDriveInfo.UsedSpacePercentage, _currentDriveInfo.HighWatermark, _currentDriveInfo.LowWatermark));
		}

		private bool RemoveStudies()
		{
			Platform.Log(LogLevel.Info, SR.MessageBeginDeleting);

			List<IStudy> studies = new List<IStudy>();
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				studies.AddRange(reader.GetStudies());
			}

			studies.Sort(new StudySortComparer());

			long totalExpectedFreeSpace = 0;
			List<string> deleteStudyUids = new List<string>();

			foreach (IStudy study in studies)
			{
				try
				{
					long expectedFreeSpace = 0;
					foreach (ISopInstance sopInstance in study.GetSopInstances())
					{
						System.IO.FileInfo info;
						try
						{
							info = new System.IO.FileInfo(sopInstance.GetLocationUri().LocalDiskPath);
							if (info.Exists)
								expectedFreeSpace += info.Length;
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e);
						}
					}

					totalExpectedFreeSpace += expectedFreeSpace;

					deleteStudyUids.Add(study.GetStudyInstanceUid());

					if (totalExpectedFreeSpace >= _currentDriveInfo.BytesOverLowWaterMark)
						break;
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			studies = null; //let this get gc'ed.

			if (deleteStudyUids.Count == 0)
			{
				Platform.Log(LogLevel.Info, SR.MessageNothingToDelete); 
				return false;
			}

			DeleteInstancesRequest request = new DeleteInstancesRequest();
			request.DeletePriority = DeletePriority.Low;
			request.InstanceLevel = InstanceLevel.Study;
			request.InstanceUids = deleteStudyUids;

			Platform.Log(LogLevel.Info, String.Format(SR.FormatDeletionRequest, deleteStudyUids.Count, totalExpectedFreeSpace));

			try
			{
				//we want to stop quickly, so have the callback poll us every 100 ms.
				LocalDataStoreDeletionHelper.DeleteInstancesAndWait(request, 100,
					delegate(LocalDataStoreDeletionHelper.DeletionProgressInformation progressInformation)
					{
						//only quit if we're stopping the service.
						return !_stop;
					});
			}
			catch
			{
				throw;
			}

			return true;
		}

		private void UpdateCurrentDriveInfo()
		{
			string driveName = null;
			LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();
			try
			{
				client.Open();

				LocalDataStoreServiceConfiguration configuration = client.GetConfiguration();
				driveName = System.IO.Path.GetPathRoot(configuration.StorageDirectory);
				driveName = driveName.TrimEnd(new char[] { Platform.PathSeparator });

				client.Close();
			}
			catch
			{
				client.Abort();
				driveName = null;
				throw;
			}

			if (driveName != null)
			{
				lock (_settingsSyncLock)
				{
					try
					{
						if (_currentDriveInfo == null || String.Compare(_currentDriveInfo.DriveName, driveName, true) != 0)
							_currentDriveInfo = new DMDriveInfo(driveName.ToUpper());
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
						_currentDriveInfo = null;
					}
				}
			}
		}

		private void CheckConfigurationSettings()
		{
			lock (_settingsSyncLock)
			{
				_settingsChanged = false;
				_checkingFrequency = DiskspaceManagerSettings.Instance.CheckFrequency * 60000;

				if (_currentDriveInfo != null)
				{
					_currentDriveInfo.LowWatermark = DiskspaceManagerSettings.Instance.LowWatermark;
					_currentDriveInfo.HighWatermark = DiskspaceManagerSettings.Instance.HighWatermark;
				}
			}
		}

        #region IDiskspaceManagerService Members

		public DiskspaceManagerServiceInformation GetServiceInformation()
        {
			// need to synchronize reading of the properties from currentDriveInfo object.
			lock (_settingsSyncLock)
			{
				Platform.CheckMemberIsSet(_currentDriveInfo, "_currentDriveInfo");
				_currentDriveInfo.Refresh();

				DiskspaceManagerServiceInformation returnInformation = new DiskspaceManagerServiceInformation();
				returnInformation.DriveName = _currentDriveInfo.DriveName;
				returnInformation.DriveSize = _currentDriveInfo.DriveSize;
				returnInformation.UsedSpace = _currentDriveInfo.UsedSpace;
				returnInformation.LowWatermark = DiskspaceManagerSettings.Instance.LowWatermark;
				returnInformation.HighWatermark = DiskspaceManagerSettings.Instance.HighWatermark;
				returnInformation.CheckFrequency = DiskspaceManagerSettings.Instance.CheckFrequency;

				return returnInformation;
			}
        }

		public void UpdateServiceConfiguration(DiskspaceManagerServiceConfiguration newConfiguration)
        {
			lock (_settingsSyncLock)
			{
				DiskspaceManagerSettings.Instance.LowWatermark = newConfiguration.LowWatermark;
				DiskspaceManagerSettings.Instance.HighWatermark = newConfiguration.HighWatermark;
				DiskspaceManagerSettings.Instance.CheckFrequency = newConfiguration.CheckFrequency;
				DiskspaceManagerSettings.Save();

				_settingsChanged = true;
				Monitor.Pulse(_settingsSyncLock);
			}
        }

		#endregion
	}
}
