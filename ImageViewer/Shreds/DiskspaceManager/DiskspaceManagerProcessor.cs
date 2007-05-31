using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Threading;

using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.ImageViewer.Services.DiskspaceManager;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    internal class DiskspaceManagerProcessor : IDiskspaceManagerService
    {
		#region Private Fields

		private static DiskspaceManagerProcessor _instance;

		private object _threadSyncLock = new object();
		private Thread _processingThread;
		private bool _stop;
		private bool _settingsChanged;

		private object _settingsSyncLock = new object();
		private DMDriveInfo _currentDriveInfo;

		private DiskspaceManagerData _diskspaceManagerData;

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
            try
            {
                _diskspaceManagerData = new DiskspaceManagerData();

				_diskspaceManagerData.OrderedStudyListReadyEvent += ValidateStudyHandler;
				_diskspaceManagerData.DeleteStudyInDBCompletedEvent += DeleteStudyCompletedHandler;

                if (!FindDiskspaceManagerDBAccessExtensionPoint())
                    return;

				lock (_threadSyncLock)
				{
					_stop = false;
					// start up processing thread
					_processingThread = new Thread(new ThreadStart(StartDiskspaceManager));
					_processingThread.Start();

					Monitor.Wait(_threadSyncLock); //wait for the thread to actually start.
				}
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void StopProcessor()
        {
            try
            {
				if (_processingThread != null)
				{
					lock (_threadSyncLock)
					{
						_stop = true;
						Monitor.Pulse(_threadSyncLock);
					}

					// wait for processing thread to finish
					_processingThread.Join();
					_processingThread = null;
				}

				_diskspaceManagerData.DeleteStudyInDBCompletedEvent -= DeleteStudyCompletedHandler;
				_diskspaceManagerData.OrderedStudyListReadyEvent -= ValidateStudyHandler;
			}
            catch (Exception e)
            {
                throw e;
            }
        }

		private void StartDiskspaceManager()
		{
			lock (_threadSyncLock)
			{
				Monitor.Pulse(_threadSyncLock); //signal that the thread has actually started.

				do
				{
					_settingsChanged = false;

					UpdateCurrentDriveInfo();
					CheckConfigurationSettings();
					CheckDiskspace();

					int waitTime = 5000;
					lock (_settingsSyncLock)
					{
						//there is currently no shred startup order, so we wait 
						//5 seconds until the Local Data Store service is up and running.
						if (_currentDriveInfo != null)
							waitTime = _diskspaceManagerData.CheckingFrequency;
					}

					Monitor.Wait(_threadSyncLock, waitTime);
					
					if (_stop)
						break;

					if (_settingsChanged || _currentDriveInfo == null)
						continue;

					if (_diskspaceManagerData.ReachHighWaterMark)
					{
						_diskspaceManagerData.IsProcessing = true;
						_diskspaceManagerData.FireOrderedStudyListRequired();
					}
				}
				while (!_stop);
			}
		}

		public void UpdateCurrentDriveInfo()
		{
			string driveName = null;
			LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();
			try
			{
				client.Open();

				LocalDataStoreServiceConfiguration configuration = client.GetConfiguration();
				driveName = Path.GetPathRoot(configuration.StorageDirectory);
				driveName = driveName.TrimEnd(new char[] { Platform.PathSeparator });

				client.Close();
			}
			catch (Exception e)
			{
				client.Abort();
				Platform.Log(e);
				driveName = null;
			}

			lock (_settingsSyncLock)
			{
				if (driveName != null)
				{
					foreach (DMDriveInfo dmDriveInfo in _diskspaceManagerData.DriveInfoList)
					{
						if (String.Compare(dmDriveInfo.DriveName, driveName, true) == 0)
						{
							_currentDriveInfo = dmDriveInfo;
							return;
						}
					}
				}

				_currentDriveInfo = null;
			}
		}

		private void CheckConfigurationSettings()
		{
			lock (_settingsSyncLock)
			{
				_diskspaceManagerData.CheckingFrequency = DiskspaceManagerSettings.Instance.CheckFrequency * 60000;

				if (_currentDriveInfo != null)
				{
					_currentDriveInfo.LowWatermark = DiskspaceManagerSettings.Instance.LowWatermark;
					_currentDriveInfo.HighWatermark = DiskspaceManagerSettings.Instance.HighWatermark;
				}
			}
		}

        private void CheckDiskspace()
        {
			if (_diskspaceManagerData == null || _diskspaceManagerData.DriveInfoList == null || _diskspaceManagerData.DriveInfoList.Count <= 0)
				return;

			foreach (DMDriveInfo dmDriveInfo in _diskspaceManagerData.DriveInfoList)
			{
				dmDriveInfo.init();
				ManagementObject drive = new ManagementObject("win32_logicaldisk.deviceid='" + dmDriveInfo.DriveName + "'");
				drive.Get();
				dmDriveInfo.DriveSize = long.Parse(drive["Size"].ToString());
				dmDriveInfo.UsedSpace = dmDriveInfo.DriveSize - long.Parse(drive["FreeSpace"].ToString());

				// debug data
				// Platform.Log("    Checking diskspace on drive (" + dmDriveInfo.DriveName + ") : " + dmDriveInfo.UsedSpace + "/" + dmDriveInfo.DriveSize
				//    + " (" + dmDriveInfo.UsedSpacePercentage + "%) (Watermark: " + dmDriveInfo.LowWatermark + " ~ " + dmDriveInfo.HighWatermark + ")");
			}
        }

        private void DeleteStudyCompletedHandler(object sender, EventArgs args)
        {
			_diskspaceManagerData.IsProcessing = false;
        }

        private void ValidateStudyHandler(object sender, EventArgs args)
        {
            ValidateOrderedStudyList();
            _diskspaceManagerData.FireDeleteStudyInDBRequired();
        }

        private void ValidateOrderedStudyList()
        {
			//Platform.Log("    Validation for DICOM files on drive:");
			foreach (DMStudyItem studyItem in _diskspaceManagerData.OrderedStudyList)
			{
				CheckStudyItem(studyItem);
				if (_diskspaceManagerData.EnoughDeletedFiles)
					break;
			}
			foreach (DMDriveInfo dmDriveInfo in _diskspaceManagerData.DriveInfoList)
			{
				if (!dmDriveInfo.ReachHighWaterMark)
					continue;

				Platform.Log("    Validation for drive " + dmDriveInfo.DriveName + " Studies found: " + dmDriveInfo.DeletedStudyNumber + "; Used Space: " + dmDriveInfo.DeletedFileSpace
					+ " (" + dmDriveInfo.UsedSpacePercentage + "%) (Watermark: " + dmDriveInfo.LowWatermark + " ~ " + dmDriveInfo.HighWatermark + ")");
			}
        }

        private void CheckStudyItem(DMStudyItem studyItem)
        {
            long usedspace = 0;
            foreach (DMSopItem sopItem in studyItem.SopItemList)
            {
                if (usedspace <= 0)
                {
                    studyItem.DriveID = -1;
                    for (int i = 0; i < _diskspaceManagerData.DriveInfoList.Count; i++)
                    {
                        if (sopItem.LocationUri.StartsWith(_diskspaceManagerData.DriveInfoList[i].DriveName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (_diskspaceManagerData.DriveInfoList[i].ReachHighWaterMark)
                                studyItem.DriveID = i;
                            break;
                        }
                    }
                    if (studyItem.DriveID == -1 || _diskspaceManagerData.DriveInfoList[studyItem.DriveID].EnoughDeletedFiles)
                        return;
                }
                if (!File.Exists(sopItem.LocationUri))
                    return;
                FileInfo dfile = new FileInfo(sopItem.LocationUri);
                usedspace += dfile.Length;
            }
            studyItem.UsedSpace = usedspace;
            _diskspaceManagerData.DriveInfoList[studyItem.DriveID].DeletedFileSpace += usedspace;
            _diskspaceManagerData.DriveInfoList[studyItem.DriveID].DeletedStudyNumber += 1;
            studyItem.Status = DiskspaceManagerStatus.ExistsOnDrive;
            //Platform.Log("    A#: " + studyItem.AccessionNumber + "; UsedSpace: " + studyItem.UsedSpace + "; StoreTime: " + studyItem.StoreTime
            //    + "; DICOMFiles: " + studyItem.SopItemList.Count + "; StudyInstanceUID: " + studyItem.StudyInstanceUID);
            return;
        }

        private bool FindDiskspaceManagerDBAccessExtensionPoint()
        {
            DiskspaceManagerDBAccessExtensionPoint xp = new DiskspaceManagerDBAccessExtensionPoint();
            object[] dmObjects = xp.CreateExtensions();
            foreach (object dmObject in dmObjects)
            {
                if (dmObject is DiskspaceManagerDBAccess)
                {
                    DiskspaceManagerDBAccess studyDataAccess = (DiskspaceManagerDBAccess)dmObject;
                    studyDataAccess.SetComponent(_diskspaceManagerData);
                    return true;
                }
            }
            return false;
        }

        #region IDiskspaceManagerService Members

		public DiskspaceManagerServiceInformation GetServiceInformation()
        {
			// need to synchronize reading of the properties from currentDriveInfo object.
			lock (_settingsSyncLock)
			{
				if (_currentDriveInfo == null)
					throw new InvalidDataException(SR.ExceptionUnableToDetermineDrive);

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
			}

			//don't block on this call, just in case the diskspace manager is currently operating.
			ThreadPool.QueueUserWorkItem(delegate(object nothing)
			{
				lock (_threadSyncLock)
				{
					_settingsChanged = true;
					Monitor.Pulse(_threadSyncLock);
				}
			});
        }

		#endregion
	}
}
