using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Shreds
{
    /// <summary>
    /// Extension point for views onto <see cref="DiskspaceManagementComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DiskspaceManagementComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public interface IDiskspaceManagement
    {
        void SetComponent(DiskspaceManagementComponent component);
        DMStudyItemList OrderedStudyList { get; set;}
        bool IsProcessing { get; set;}
        event EventHandler DeleteStudyInDBRequiredEvent;
        event EventHandler OrderedStudyListRequiredEvent;

    }

    /// <summary>
    /// DiskspaceManagementComponent class
    /// </summary>
    [AssociateView(typeof(DiskspaceManagementComponentViewExtensionPoint))]
    public class DiskspaceManagementComponent : ApplicationComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DiskspaceManagementComponent()
        {
            _orderedStudyList = new DMStudyItemList();
            _isProcessing = false;
            _highWatermark = 40;
            _lowWatermark = 30;
            _watermarkMinDifference = 5;
            _dataStoreDrives = new string[] { "C:" };
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Fields

        private DMStudyItemList _orderedStudyList;
        private string [] _dataStoreDrives;
        private bool _isProcessing;
        private readonly long _watermarkMinDifference;
        private decimal _highWatermark;
        private decimal _lowWatermark;
        private long _usedSpace;
        private long _driveSize;

        private event EventHandler _orderedStudyListReadyEvent;
        private event EventHandler _orderedStudyListRequiredEvent;
        private event EventHandler _deleteStudyInDBCompletedEvent;
        private event EventHandler _deleteStudyInDBRequiredEvent;

        public DMStudyItemList OrderedStudyList
        {
            get { return _orderedStudyList; }
            set { _orderedStudyList = value; }
        }

        public string[] DataStoreDrives
        {
            get { return _dataStoreDrives; }
            set { _dataStoreDrives = value; }
        }

        public bool IsProcessing
        {
            get { return _isProcessing; }
            set { _isProcessing = value; }
        }

        public decimal HighWatermark
        {
            get { return _highWatermark; }
            set { _highWatermark = value >= _lowWatermark + _watermarkMinDifference ? value : _lowWatermark + _watermarkMinDifference; }
        }

        public decimal LowWatermark
        {
            get { return _lowWatermark; }
            set { _lowWatermark = value <= _highWatermark - _watermarkMinDifference ? value : _highWatermark - _watermarkMinDifference; }
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

        #endregion

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

        public long UsedSpace
        {
            get { return _usedSpace; }
            set { _usedSpace = value; }
        }

        public DiskspaceManagementStatus Status
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
        private long _usedSpace;
        private DiskspaceManagementStatus _status;
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

    public enum DiskspaceManagementStatus
    {
        ExistsInDatabase,
        ExistsOnLocalDrive,
        DeletedFromDatabase,
        DeletedFromLocalDrive
    }
}
