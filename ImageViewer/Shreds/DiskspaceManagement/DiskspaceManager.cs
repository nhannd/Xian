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
            _deletedFileSpace = 0;

        }

        public override void Start()
        {
            _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
            _component = new DiskspaceManagementComponent();
            if (!FindRetrieveStudyListExtensionPoint())
                return;

            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked");
            _stopSignal.Reset();

            // start up processing thread
            Thread t = new Thread(new ThreadStart(StartDiskspaceManager));
            t.Start();

            // wait for host's stop signal
            _stopSignal.WaitOne();

            // wait for processing thread to finish
            t.Join(); 
            
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: ... Start invoked on port " + this.ServicePort.ToString());
            //throw new Exception("The method or operation is not implemented.");
        }

        public override void Stop()
        {
            _component.OrderedStudyListReadyEvent -= DeleteStudyHandler;
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
            _component.OrderedStudyListReadyEvent += DeleteStudyHandler;
            while (!_stopSignal.WaitOne(6000, true))
            {
                if (_component.IsProcessing)
                    continue;
                GetDiskSpace();
                if (ReachHighWaterMark)
                {
                    _component.IsProcessing = true;
                    _component.FireOrderedStudyListRequired();
                }
            }
        }

        private void GetDiskSpace()
        {
            if (_component == null || _component.DataStoreDrives == null || _component.DataStoreDrives.Length <= 0)
                return;

            _driveName = _component.DataStoreDrives[0];
            ManagementObject drive = new ManagementObject("win32_logicaldisk.deviceid='" + _driveName + "'");
            drive.Get();
            _component.DriveSize = long.Parse(drive["Size"].ToString());
            _component.UsedSpace = _component.DriveSize - long.Parse(drive["FreeSpace"].ToString());
            Platform.Log("==========================================");
            Platform.Log("        " + DateTime.Now.ToString() + " > Drive (" + DriveName + ") : " + _component.UsedSpace + "/" + _component.DriveSize
                + " (" + UsedSpacePercentage + "%)");

        }

        private void DeleteStudyHandler(object sender, EventArgs args)
        {

            ValidateOrderedStudyList();
            _component.IsProcessing = false;
        }

        private void ValidateOrderedStudyList()
        {

            foreach (DMStudyItem studyItem in _component.OrderedStudyList)
            {
                CheckStudyItem(studyItem);
                Platform.Log("    A#: " + studyItem.AccessionNumber + "; UsedSpace: " + studyItem.UsedSpace + "; CreatedTimeStamp: " + studyItem.CreatedTimeStamp
                    + "; Status: " + studyItem.Status + "; DICOMFiles: " + studyItem.SopItemList.Count + "; StudyInstanceUID: " + studyItem.StudyInstanceUID);
            }
        }

        private void CheckStudyItem(DMStudyItem studyItem)
        {
            long usedspace = 0;
            foreach (DMSopItem sopItem in studyItem.SopItemList)
            {
                string locationUri = sopItem.LocationUri.Replace("file://localhost/", "");
                if (!locationUri.StartsWith(_component.DataStoreDrives[0]) || !File.Exists(locationUri))
                    return;
                FileInfo dfile = new FileInfo(locationUri);
                usedspace += dfile.Length;
                //Platform.Log("        " + sopItem.LocationUri + " (" + dfile.Length + " bytes, " + sopItem.SopInstanceUID + ")");
            }
            studyItem.UsedSpace = usedspace;
            studyItem.Status = DiskspaceManagementStatus.ExistsOnLocalDrive;
            return;
        }

        private bool FindRetrieveStudyListExtensionPoint()
        {
            RetrieveStudyListExtensionPoint xp = new RetrieveStudyListExtensionPoint();
            object[] dmObjects = xp.CreateExtensions(); // ListExtensions(); // .CreateExtensions();
            foreach (object dmObject in dmObjects)
            {
                if (dmObject is RetrieveStudyList)
                {
                    RetrieveStudyList retrieveStudyList = (RetrieveStudyList)dmObject;
                    retrieveStudyList.SetComponent(_component);
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
        private string _driveName;
        private long _deletedFileSpace;

        public string DriveName
        {
            get { return _driveName; }
            set { _driveName = value; }
        }

        public long DeletedFileSpace
        {
            get { return _deletedFileSpace; }
            set { _deletedFileSpace = value; }
        }

        public bool EnoughDeletedFiles
        {
            get { return (100 * (_component.UsedSpace - _deletedFileSpace) / _component.DriveSize) <= _component.LowWatermark ? true : false; }
        }

        public decimal UsedSpacePercentage
        {
            get { return new decimal(100 * _component.UsedSpace / _component.DriveSize); }
        }

        public bool ReachHighWaterMark
        {
            get { return UsedSpacePercentage >= _component.HighWatermark ? true : false; }
        }

        public bool ReachLowWaterMark
        {
            get { return UsedSpacePercentage <= _component.LowWatermark ? true : false; }
        }

        #endregion

    }
}
