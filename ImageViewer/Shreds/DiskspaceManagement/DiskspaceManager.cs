using System;
using System.Collections.Generic;
using System.IO;
using System.Management; 
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class DiskspaceManager : WcfShred 
    {
        public DiskspaceManager()
        {
            _className = this.GetType().ToString();
            _serviceEndPointName = "DiskspaceManager";
        }

        public override void Start()
        {
            _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
            _component = new DiskspaceManagementComponent();
            if (!FindStudyDataAccessExtensionPoint())
                return;

            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked");
            _stopSignal.Reset();

            Platform.Log("  Port#: " + this.ServicePort.ToString());

            StartHost<DiskspaceManagerShredServiceData, IDiskspaceManagementShredInterface>(_serviceEndPointName, "DiskspaceManager");

            // start up processing thread
            Thread t = new Thread(new ThreadStart(StartDiskspaceManager));
            t.Start();

            // wait for host's stop signal
            _stopSignal.WaitOne();

            // wait for processing thread to finish
            t.Join();

            // stop hosting the service
            StopHost(_serviceEndPointName);

            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: ... Start invoked on port " + this.ServicePort.ToString());
            //throw new Exception("The method or operation is not implemented.");
        }

        public override void Stop()
        {
            _component.DeleteStudyInDBCompletedEvent -= DeleteStudyHandler;
            _component.OrderedStudyListReadyEvent -= ValidateStudyHandler;
            _stopSignal.Set();
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: ... Stop invoked");
        }

        public override string GetDisplayName()
        {
            return "DiskspaceManager";
        }

        public override string GetDescription()
        {
            return "Maintain disk space";
        }

        private void StartDiskspaceManager()
        {
            _component.OrderedStudyListReadyEvent += ValidateStudyHandler;
            _component.DeleteStudyInDBCompletedEvent += DeleteStudyHandler;
            while (!_stopSignal.WaitOne(300000, true))
            {
                if (_component.IsProcessing)
                    continue;
                CheckDiskspace();
                if (_component.ReachHighWaterMark)
                {
                    //_deletedFileSpace = 0;
                    //_deletedStudyNumber = 0;
                    _component.IsProcessing = true;
                    _component.FireOrderedStudyListRequired();
                }
            }
        }

        private void CheckDiskspace()
        {
            if (_component == null || _component.DriveInfoList == null || _component.DriveInfoList.Count <= 0)
                return;

            foreach (DMDriveInfo dmDriveInfo in _component.DriveInfoList)
            {
                dmDriveInfo.init();
                ManagementObject drive = new ManagementObject("win32_logicaldisk.deviceid='" + dmDriveInfo.DriveName + "'");
                drive.Get();
                dmDriveInfo.DriveSize = long.Parse(drive["Size"].ToString());
                dmDriveInfo.UsedSpace = dmDriveInfo.DriveSize - long.Parse(drive["FreeSpace"].ToString());
                //Platform.Log("    Checking diskspace on drive (" + dmDriveInfo.DriveName + ") : " + dmDriveInfo.UsedSpace + "/" + dmDriveInfo.DriveSize
                //    + " (" + dmDriveInfo.UsedSpacePercentage + "%) (Watermark: " + dmDriveInfo.LowWatermark + " ~ " + dmDriveInfo.HighWatermark + ")");
            }

        }

        private void DeleteStudyHandler(object sender, EventArgs args)
        {
            DeleteStudyOnDrive();
            _component.IsProcessing = false;
        }

        private void ValidateStudyHandler(object sender, EventArgs args)
        {
            ValidateOrderedStudyList();
            _component.FireDeleteStudyInDBRequired();
        }

        private void DeleteStudyOnDrive()
        {
            for (int i = 0; i < _component.DriveInfoList.Count; i++)
            {
                if (_component.DriveInfoList[i].ReachLowWaterMark)
                    continue;
                int deletedNumber = 0;
                long deletedSpace = 0;
                string fileName = "";
                foreach (DMStudyItem studyItem in _component.OrderedStudyList)
                {
                    if (studyItem.DriveID != i || !studyItem.Status.Equals(DiskspaceManagementStatus.DeletedFromDatabase))
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
                        studyItem.Status = DiskspaceManagementStatus.DeletedFromDrive;
                        Platform.Log("    Studies deleted on drive " + _component.DriveInfoList[i].DriveName + " -> Study " + deletedNumber + ") DicomFiles: " + studyItem.SopItemList.Count + "; UsedSpace: " + studyItem.UsedSpace
                            + "; StudyInstanceUID: " + studyItem.StudyInstanceUID + "; StudyFolder: " + studyFolder);
                    }
                }
                _component.DriveInfoList[i].UsedSpace -= deletedSpace;
                if (deletedNumber > 0)
                    Platform.Log("    Total studies deleted on drive " + _component.DriveInfoList[i].DriveName + " -> Deleted Studies: " + deletedNumber + "; Deleted Space: " + deletedSpace
                        + " (Current Used Space: " + _component.DriveInfoList[i].UsedSpacePercentage + "%) (Watermark: " + _component.DriveInfoList[i].LowWatermark + " ~ " + _component.DriveInfoList[i].HighWatermark + ")");
            }
        }

        private void ValidateOrderedStudyList()
        {
            //Platform.Log("    Validation for DICOM files on drive:");
            foreach (DMStudyItem studyItem in _component.OrderedStudyList)
            {
                CheckStudyItem(studyItem);
                if (_component.EnoughDeletedFiles)
                    break;
            }
            foreach (DMDriveInfo dmDriveInfo in _component.DriveInfoList)
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
                    for (int i = 0; i < _component.DriveInfoList.Count; i++)
                    {
                        if (sopItem.LocationUri.StartsWith(_component.DriveInfoList[i].DriveName))
                        {
                            if (_component.DriveInfoList[i].ReachHighWaterMark)
                                studyItem.DriveID = i;
                            break;
                        }
                    }
                    if (studyItem.DriveID == -1 || _component.DriveInfoList[studyItem.DriveID].EnoughDeletedFiles)
                        return;
                }
                if (!File.Exists(sopItem.LocationUri))
                    return;
                FileInfo dfile = new FileInfo(sopItem.LocationUri);
                usedspace += dfile.Length;
            }
            studyItem.UsedSpace = usedspace;
            _component.DriveInfoList[studyItem.DriveID].DeletedFileSpace += usedspace;
            _component.DriveInfoList[studyItem.DriveID].DeletedStudyNumber += 1;
            studyItem.Status = DiskspaceManagementStatus.ExistsOnDrive;
            //Platform.Log("    A#: " + studyItem.AccessionNumber + "; UsedSpace: " + studyItem.UsedSpace + "; StoreTime: " + studyItem.StoreTime
            //    + "; DICOMFiles: " + studyItem.SopItemList.Count + "; StudyInstanceUID: " + studyItem.StudyInstanceUID);
            return;
        }

        private bool FindStudyDataAccessExtensionPoint()
        {
            StudyDataAccessExtensionPoint xp = new StudyDataAccessExtensionPoint();
            object[] dmObjects = xp.CreateExtensions(); 
            foreach (object dmObject in dmObjects)
            {
                if (dmObject is StudyDataAccess)
                {
                    StudyDataAccess studyDataAccess = (StudyDataAccess)dmObject;
                    studyDataAccess.SetComponent(_component);
                    return true;
                }
            }
            return false;
        }

        #region Fields

        private EventWaitHandle _stopSignal;
        private DiskspaceManagementComponent _component;
        private readonly string _className;
        private readonly string _serviceEndPointName;

        #endregion

    }
}
