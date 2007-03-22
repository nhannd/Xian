using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Threading;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.ImageViewer.Services.DiskspaceManager;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public partial class DiskspaceManagerProcessor : IDiskspaceManagerService
    {
		private static DiskspaceManagerProcessor _instance;

        private string _driveName;
        private string _status;
        private float _lowWatermark;
        private float _highWatermark;
        private float _usedSpace;

        public DiskspaceManagerProcessor()
        {
            _driveName = DiskspaceManagerSettings.Instance.DriveName;
            _status = DiskspaceManagerSettings.Instance.Status;
            _lowWatermark = DiskspaceManagerSettings.Instance.LowWatermark;
            _highWatermark = DiskspaceManagerSettings.Instance.HighWatermark;
            _usedSpace = DiskspaceManagerSettings.Instance.UsedSpace;

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
                _driveName = "E";
                _status = "Start ...";
                _lowWatermark = 55.5F;
                _highWatermark = 88.8F;
                _usedSpace = 77.7F;

                _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
                _diskspaceManagerData = new DiskspaceManagerData();
                if (!FindDiskspaceManagerDBAccessExtensionPoint())
                    return;

                _stopSignal.Reset();
                // start up processing thread
                Thread t = new Thread(new ThreadStart(StartDiskspaceManager));
                t.Start();

                // wait for host's stop signal
                _stopSignal.WaitOne();

                // wait for processing thread to finish
                t.Join();

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
                _diskspaceManagerData.DeleteStudyInDBCompletedEvent -= DeleteStudyHandler;
                _diskspaceManagerData.OrderedStudyListReadyEvent -= ValidateStudyHandler;
                _stopSignal.Set();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void StartDiskspaceManager()
        {
            _diskspaceManagerData.OrderedStudyListReadyEvent += ValidateStudyHandler;
            _diskspaceManagerData.DeleteStudyInDBCompletedEvent += DeleteStudyHandler;
            while (!_stopSignal.WaitOne(_diskspaceManagerData.CheckingFrequency, true))
            {
                if (_diskspaceManagerData.IsProcessing)
                    continue;
                CheckDiskspace();
                if (_diskspaceManagerData.ReachHighWaterMark)
                {
                    _diskspaceManagerData.IsProcessing = true;
                    _diskspaceManagerData.FireOrderedStudyListRequired();
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
                Platform.Log("    Checking diskspace on drive (" + dmDriveInfo.DriveName + ") : " + dmDriveInfo.UsedSpace + "/" + dmDriveInfo.DriveSize
                    + " (" + dmDriveInfo.UsedSpacePercentage + "%) (Watermark: " + dmDriveInfo.LowWatermark + " ~ " + dmDriveInfo.HighWatermark + ")");
            }

        }

        private void DeleteStudyHandler(object sender, EventArgs args)
        {
            DeleteStudyOnDrive();
            _diskspaceManagerData.IsProcessing = false;
        }

        private void ValidateStudyHandler(object sender, EventArgs args)
        {
            ValidateOrderedStudyList();
            _diskspaceManagerData.FireDeleteStudyInDBRequired();
        }

        private void DeleteStudyOnDrive()
        {
            for (int i = 0; i < _diskspaceManagerData.DriveInfoList.Count; i++)
            {
                if (_diskspaceManagerData.DriveInfoList[i].ReachLowWaterMark)
                    continue;
                int deletedNumber = 0;
                long deletedSpace = 0;
                string fileName = "";
                foreach (DMStudyItem studyItem in _diskspaceManagerData.OrderedStudyList)
                {
                    if (studyItem.DriveID != i || !studyItem.Status.Equals(DiskspaceManagerStatus.DeletedFromDatabase))
                        continue;
                    bool isSuccessful = true;
                    foreach (DMSopItem sopItem in studyItem.SopItemList)
                    {
                        try
                        {
                            fileName = sopItem.LocationUri;
                            if (!File.Exists(fileName))
                            {
                                Platform.Log("    Studies deleted on disk warning: file does not exist (" + fileName + ")");
                            }
                            else
                                File.Delete(fileName);
                        }
                        catch (Exception e)
                        {
                            Platform.Log(e, LogLevel.Error);
                            isSuccessful = false;
                        }
                    }
                    if (isSuccessful)
                    {
                        string studyFolder = fileName.Substring(0, fileName.IndexOf(studyItem.StudyInstanceUID) + studyItem.StudyInstanceUID.Length);
                        if (Directory.Exists(studyFolder))
                        {
                            DirectoryInfo dinfo = new DirectoryInfo(studyFolder);
                            if (dinfo.GetFiles("*", SearchOption.AllDirectories).Length <= 0)
                                Directory.Delete(studyFolder, true);
                        }
                        deletedNumber += 1;
                        deletedSpace += studyItem.UsedSpace;
                        studyItem.Status = DiskspaceManagerStatus.DeletedFromDrive;
                        Platform.Log("    Studies deleted on drive " + _diskspaceManagerData.DriveInfoList[i].DriveName + " -> Study " + deletedNumber + ") DicomFiles: " + studyItem.SopItemList.Count + "; UsedSpace: " + studyItem.UsedSpace
                            + "; StudyInstanceUID: " + studyItem.StudyInstanceUID + "; StudyFolder: " + studyFolder);
                    }
                }
                _diskspaceManagerData.DriveInfoList[i].UsedSpace -= deletedSpace;
                if (deletedNumber > 0)
                    Platform.Log("    Total studies deleted on drive " + _diskspaceManagerData.DriveInfoList[i].DriveName + " -> Deleted Studies: " + deletedNumber + "; Deleted Space: " + deletedSpace
                        + " (Current Used Space: " + _diskspaceManagerData.DriveInfoList[i].UsedSpacePercentage + "%) (Watermark: " + _diskspaceManagerData.DriveInfoList[i].LowWatermark + " ~ " + _diskspaceManagerData.DriveInfoList[i].HighWatermark + ")");
            }
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
                //Platform.Log("    Validation for drive " + dmDriveInfo.DriveName + " Studies found: " + dmDriveInfo.DeletedStudyNumber + "; Used Space: " + dmDriveInfo.DeletedFileSpace
                //    + " (" + dmDriveInfo.UsedSpacePercentage + "%) (Watermark: " + dmDriveInfo.LowWatermark + " ~ " + dmDriveInfo.HighWatermark + ")");
            }
            return;
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
                        if (sopItem.LocationUri.StartsWith(_diskspaceManagerData.DriveInfoList[i].DriveName))
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

        #region Fields

        private DiskspaceManagerData _diskspaceManagerData;
        private EventWaitHandle _stopSignal;

        #endregion

        #region Properties

        public string DriveName
        {
            get { return _driveName; }
            set { _driveName = value; }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public float LowWatermark
        {
            get { return _lowWatermark; }
            set { _lowWatermark = value; }
        }

        public float HighWatermark
        {
            get { return _highWatermark; }
            set { _highWatermark = value; }
        }

        public float UsedSpace
        {
            get { return _usedSpace; }
            set { _usedSpace = value; }
        }

        #endregion

        #region IDiskspaceManagerService Members

        public GetServerSettingResponse GetServerSetting()
        {
            return new GetServerSettingResponse(DiskspaceManagerSettings.Instance.DriveName,
                                                DiskspaceManagerSettings.Instance.Status,
                                                DiskspaceManagerSettings.Instance.LowWatermark,
                                                DiskspaceManagerSettings.Instance.HighWatermark,
                                                DiskspaceManagerSettings.Instance.UsedSpace);
        }

        public void UpdateServerSetting(UpdateServerSettingRequest request)
        {
            DiskspaceManagerSettings.Instance.DriveName = request.DriveName;
            DiskspaceManagerSettings.Instance.Status = request.Status;
            DiskspaceManagerSettings.Instance.LowWatermark = request.LowWatermark;
            DiskspaceManagerSettings.Instance.HighWatermark = request.HighWatermark;
            DiskspaceManagerSettings.Instance.UsedSpace = request.UsedSpace;
            DiskspaceManagerSettings.Save();
        }

		#endregion

	}
}
