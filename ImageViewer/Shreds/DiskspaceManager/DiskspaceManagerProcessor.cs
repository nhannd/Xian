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
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.DiskspaceManager;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    internal sealed class DiskspaceManagerProcessor : IDiskspaceManagerService
    {
		#region Private Fields

		private static DiskspaceManagerProcessor _instance;

		private volatile Thread _processingThread;
		private volatile bool _stop;

		private readonly object _settingsSyncLock = new object();
		private volatile DMDriveInfo _currentDriveInfo;
		private volatile int _checkingFrequency;

    	private bool _enforceStudyLimit = false;
		private volatile int _studyLimit = int.MaxValue;
		private volatile int _lastStudyCount = 0;

		private volatile bool _settingsChanged;

		#endregion

		private DiskspaceManagerProcessor()
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
			_processingThread = new Thread(StartDiskspaceManager);
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

					CheckUsage(true, Timeout.Infinite);

					if (_currentDriveInfo.BytesOverHighWaterMark > 0 || IsStudyLimitExceeded())
					{
						RemoveStudies();
						CheckUsage(true, Timeout.Infinite);
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}
			while (!_stop);
		}

		private bool IsStudyLimitExceeded()
		{
			return _lastStudyCount > _studyLimit;
		}

    	private void CheckUsage(bool log, int maxWaitMilliseconds)
		{
			try
			{
				_currentDriveInfo.Refresh(maxWaitMilliseconds);
				_lastStudyCount = 0;
				using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
				{
					_lastStudyCount = (int)reader.GetStudyCount();
				}

				if (log)
					Platform.Log(LogLevel.Info, String.Format(SR.FormatCheckUsage, _currentDriveInfo.UsedSpacePercentage, _currentDriveInfo.HighWatermark, _currentDriveInfo.LowWatermark, _lastStudyCount));
			}
			catch (DataStoreException e)
			{
				Platform.Log(LogLevel.Error, e, "Failed to retrieve number of studies from data store.");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Failed to check current disk space usage.");
			}
		}

		private void RemoveStudies()
		{
			Platform.Log(LogLevel.Info, SR.MessageBeginDeleting);

			List<IStudy> studies = new List<IStudy>();
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				studies.AddRange(reader.GetStudiesByStoreTime(false));
			}

			long totalExpectedFreeSpace = 0;
			List<string> deleteStudyUids = new List<string>();

			foreach (IStudy study in studies)
			{
				long expectedFreeSpace = 0;

				try
				{
					foreach (ISopInstance sopInstance in study.GetSopInstances())
					{
						System.IO.FileInfo info;
						info = new System.IO.FileInfo(sopInstance.GetLocationUri().LocalDiskPath);
						if (info.Exists)
							expectedFreeSpace += info.Length;
					}
				}
				catch (Exception e)
				{
					string formatMessage = "Failed to determine current disk space usage for study '{0}'; only studies processed up to this point will be deleted.";
					Platform.Log(LogLevel.Error, e, formatMessage, study.StudyInstanceUid);
					break;
				}

				totalExpectedFreeSpace += expectedFreeSpace;
				deleteStudyUids.Add(study.StudyInstanceUid);

				if (_enforceStudyLimit)
				{
					//keep adding studies to delete until the max# is reached.
					int numberOfStudiesAfterDelete = _lastStudyCount - deleteStudyUids.Count;
					if (numberOfStudiesAfterDelete > _studyLimit)
						continue;
					else
						break;
				}

				if (totalExpectedFreeSpace >= _currentDriveInfo.BytesOverLowWaterMark)
					break;
			}

			if (deleteStudyUids.Count == 0)
			{
				Platform.Log(LogLevel.Info, SR.MessageNothingToDelete); 
				return;
			}

			DeleteInstancesRequest request = new DeleteInstancesRequest();
			request.DeletePriority = DeletePriority.Low;
			request.InstanceLevel = InstanceLevel.Study;
			request.InstanceUids = deleteStudyUids;

			Platform.Log(LogLevel.Info, String.Format(SR.FormatDeletionRequest, deleteStudyUids.Count, totalExpectedFreeSpace));

			//we want to stop quickly, so have the callback poll us every 100 ms.
			LocalDataStoreDeletionHelper.DeleteInstancesAndWait(request, 100, delegate 
				{	//only quit if we're stopping the service.
					return !_stop;
				});
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

				_enforceStudyLimit = DiskspaceManagerSettings.Instance.EnforceStudyLimit;
				if (_enforceStudyLimit)
					_studyLimit = DiskspaceManagerSettings.Instance.StudyLimit;
				else
					_studyLimit = int.MaxValue;
			}
		}

        #region IDiskspaceManagerService Members

		public DiskspaceManagerServiceInformation GetServiceInformation()
        {
			// need to synchronize reading of the properties from currentDriveInfo object.
			lock (_settingsSyncLock)
			{
				Platform.CheckMemberIsSet(_currentDriveInfo, "_currentDriveInfo");
				CheckUsage(false, 5000);

				DiskspaceManagerServiceInformation returnInformation = new DiskspaceManagerServiceInformation();
				returnInformation.DriveName = _currentDriveInfo.DriveName;
				returnInformation.DriveSize = _currentDriveInfo.DriveSize;
				returnInformation.UsedSpace = _currentDriveInfo.UsedSpace;
				returnInformation.LowWatermark = DiskspaceManagerSettings.Instance.LowWatermark;
				returnInformation.HighWatermark = DiskspaceManagerSettings.Instance.HighWatermark;
				returnInformation.CheckFrequency = DiskspaceManagerSettings.Instance.CheckFrequency;
				returnInformation.StudyCount = _lastStudyCount;
				returnInformation.MinStudyLimit = DiskspaceManagerSettings.Instance.MinStudyLimit;
				returnInformation.MaxStudyLimit = DiskspaceManagerSettings.Instance.MaxStudyLimit;
				returnInformation.EnforceStudyLimit = DiskspaceManagerSettings.Instance.EnforceStudyLimit;
				returnInformation.StudyLimit = DiskspaceManagerSettings.Instance.StudyLimit;

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
				DiskspaceManagerSettings.Instance.EnforceStudyLimit = newConfiguration.EnforceStudyLimit;
				DiskspaceManagerSettings.Instance.StudyLimit = newConfiguration.StudyLimit;
				DiskspaceManagerSettings.Save();

				_settingsChanged = true;
				Monitor.Pulse(_settingsSyncLock);
			}
        }

		#endregion
	}
}
