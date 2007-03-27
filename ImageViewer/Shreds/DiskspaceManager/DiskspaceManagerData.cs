using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    /// <summary>
    /// Extension point for views onto <see cref="DiskspaceManagerData"/>
    /// </summary>
    public interface IDiskspaceManager
    {
        void SetComponent(DiskspaceManagerData diskspaceManagerData);
        DMStudyItemList OrderedStudyList { get; set;}
        bool IsProcessing { get; set;}
        event EventHandler DeleteStudyInDBRequiredEvent;
        event EventHandler OrderedStudyListRequiredEvent;

    }

    /// <summary>
    /// DiskspaceManagerData class
    /// </summary>
    public class DiskspaceManagerData 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DiskspaceManagerData()
        {
            _orderedStudyList = new DMStudyItemList();
            _isProcessing = false;
            _checkingFrequency = 120000;
            _driveInfoList = new DMDriveInfoList();
            DriveInfo[] driveinfos = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in driveinfos)
            {
                if (!driveInfo.DriveType.Equals(DriveType.Fixed))
                    continue;
                DMDriveInfo dmDriveInfo = new DMDriveInfo();
                dmDriveInfo.DriveName = driveInfo.RootDirectory.ToString().Replace("\\", "");
                _driveInfoList.Add(dmDriveInfo);
            }
        }

        #region Fields

        private DMStudyItemList _orderedStudyList;
        private DMDriveInfoList _driveInfoList;
        private int _checkingFrequency;
        private bool _isProcessing;

        private event EventHandler _orderedStudyListReadyEvent;
        private event EventHandler _orderedStudyListRequiredEvent;
        private event EventHandler _deleteStudyInDBCompletedEvent;
        private event EventHandler _deleteStudyInDBRequiredEvent;

        public DMStudyItemList OrderedStudyList
        {
            get { return _orderedStudyList; }
            set { _orderedStudyList = value; }
        }

        public DMDriveInfoList DriveInfoList
        {
            get { return _driveInfoList; }
            set { _driveInfoList = value; }
        }

        public bool IsProcessing
        {
            get { return _isProcessing; }
            set { _isProcessing = value; }
        }

        public int CheckingFrequency
        {
            get { return _checkingFrequency; }
            set { _checkingFrequency = value; }
        }

        #endregion 

        public bool EnoughDeletedFiles
        {
            get {
                foreach (DMDriveInfo dmDriveInfo in _driveInfoList)
                {
                    if (!dmDriveInfo.EnoughDeletedFiles)
                        return false;
                }
                return true; 
            }
        }

        public bool ReachHighWaterMark
        {
            get {
                foreach (DMDriveInfo dmDriveInfo in _driveInfoList)
                {
                    if (dmDriveInfo.ReachHighWaterMark)
                        return true;
                }
                return false; 
            }
        }

        public event EventHandler OrderedStudyListReadyEvent
        {
            add { _orderedStudyListReadyEvent += value; }
            remove { _orderedStudyListReadyEvent -= value; }
        }

        public event EventHandler OrderedStudyListRequiredEvent
        {
            add { _orderedStudyListRequiredEvent += value; }
            remove { _orderedStudyListRequiredEvent -= value; }
        }

        public event EventHandler DeleteStudyInDBCompletedEvent
        {
            add { _deleteStudyInDBCompletedEvent += value; }
            remove { _deleteStudyInDBCompletedEvent -= value; }
        }

        public event EventHandler DeleteStudyInDBRequiredEvent
        {
            add { _deleteStudyInDBRequiredEvent += value; }
            remove { _deleteStudyInDBRequiredEvent -= value; }
        }

        public void FireOrderedStudyListReady()
        {
            EventsHelper.Fire(_orderedStudyListReadyEvent, this, EventArgs.Empty);
        }

        public void FireOrderedStudyListRequired()
        {
            EventsHelper.Fire(_orderedStudyListRequiredEvent, this, EventArgs.Empty);
        }

        public void FireDeleteStudyInDBCompleted()
        {
            EventsHelper.Fire(_deleteStudyInDBCompletedEvent, this, EventArgs.Empty);
        }

        public void FireDeleteStudyInDBRequired()
        {
            EventsHelper.Fire(_deleteStudyInDBRequiredEvent, this, EventArgs.Empty);
        }

    }

    public class DMStudyItemList : List<DMStudyItem>
    {
        public DMStudyItemList()
        {
        }
    }

    public class DMStudyItem
    {
        public DMStudyItem()
        {
        }

        public string StudyInstanceUID
        {
            get { return _studyInstanceUID; }
            set { _studyInstanceUID = value; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public DateTime StoreTime
        {
            get { return _storeTime; }
            set { _storeTime = value; }
        }

        public int DriveID
        {
            get { return _driveID; }
            set { _driveID = value; }
        }

        public long UsedSpace
        {
            get { return _usedSpace; }
            set { _usedSpace = value; }
        }

        public DiskspaceManagerStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public List<DMSopItem> SopItemList
        {
            get { return _sopItemList; }
            set { _sopItemList = value; }
        }

        #region Private Members

        private string _studyInstanceUID;
        private string _accessionNumber;
        private DateTime _storeTime;
        private int _driveID;
        private long _usedSpace;
        private DiskspaceManagerStatus _status;
        private List<DMSopItem> _sopItemList;

        #endregion

    }

    public class DMSopItem
    {
        public DMSopItem()
        {
        }

        public string SopInstanceUID
        {
            get { return _sopInstanceUID; }
            set { _sopInstanceUID = value; }
        }

        public string LocationUri
        {
            get { return _locationUri; }
            set { _locationUri = value; }
        }

        #region Private Members
        private string _sopInstanceUID;
        private string _locationUri;
        #endregion

    }

    public class DMDriveInfoList : List<DMDriveInfo>
    {
        public DMDriveInfoList()
        {
        }
    }

    public class DMDriveInfo
    {
        public DMDriveInfo()
        {
            _highWatermark = 60;
            _lowWatermark = 40;
            _watermarkMinDifference = 5;
            init();
        }

        public void init()
        {
            _usedSpace = 0;
            _driveSize = 1;
            _deletedFileSpace = 0;
            _deletedStudyNumber = 0;
        }

        public string DriveName
        {
            get { return _driveName; }
            set { _driveName = value; }
        }

        public float HighWatermark
        {
            get { return _highWatermark; }
            set { _highWatermark = value >= _lowWatermark + _watermarkMinDifference ? value : _lowWatermark + _watermarkMinDifference; }
        }

        public float LowWatermark
        {
            get { return _lowWatermark; }
            set { _lowWatermark = value <= _highWatermark - _watermarkMinDifference ? value : _highWatermark - _watermarkMinDifference; }
        }

        public float WatermarkMinDifference
        {
            get { return _watermarkMinDifference; }
            set { _watermarkMinDifference = value; }
        }

        public long UsedSpace
        {
            get { return _usedSpace; }
            set { _usedSpace = value; }
        }

        public long DriveSize
        {
            get { return _driveSize; }
            set { _driveSize = value; }
        }

        public long DeletedFileSpace
        {
            get { return _deletedFileSpace; }
            set { _deletedFileSpace = value; }
        }

        public int DeletedStudyNumber
        {
            get { return _deletedStudyNumber; }
            set { _deletedStudyNumber = value; }
        }

        public bool EnoughDeletedFiles
        {
            get { return (100 * (_usedSpace - _deletedFileSpace) / _driveSize) <= _lowWatermark ? true : false; }
        }

        public float UsedSpacePercentage
        {
            get { return ((int)(10000.0F * _usedSpace / _driveSize))/100.0F; }
        }

        public bool ReachHighWaterMark
        {
            get { return UsedSpacePercentage >= _highWatermark ? true : false; }
        }

        public bool ReachLowWaterMark
        {
            get { return UsedSpacePercentage <= _lowWatermark ? true : false; }
        }

        #region Private Members

        private string _driveName;
        private float _highWatermark;
        private float _lowWatermark;
        private float _watermarkMinDifference;
        private long _usedSpace;
        private long _driveSize;
        private long _deletedFileSpace;
        private int _deletedStudyNumber;

        #endregion
    }

    public enum DiskspaceManagerStatus
    {
        ExistsInDatabase,
        ExistsOnDrive,
        DeletedFromDatabase,
        DeletedFromDrive
    }
}
