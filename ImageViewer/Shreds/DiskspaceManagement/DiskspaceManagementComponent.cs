using System;
using System.Collections.Generic;
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
        event EventHandler OrderedStudyListReadyEvent;
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
            _dataStoreDrives = new string[] { "C:" };
            _highWatermark = 40;
            _lowWatermark = 30;
            _markDifference = 5;
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
        private readonly long _markDifference;
        private decimal _highWatermark;
        private decimal _lowWatermark;
        private long _usedSpace;
        private long _driveSize;

        private event EventHandler _orderedStudyListReadyEvent;
        private event EventHandler _orderedStudyListRequiredEvent;

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
            set { _highWatermark = value >= _lowWatermark + _markDifference ? value : _lowWatermark + _markDifference; }
        }

        public decimal LowWatermark
        {
            get { return _lowWatermark; }
            set { _lowWatermark = value <= _highWatermark - _markDifference ? value : _highWatermark - _markDifference; }
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

        public void FireOrderedStudyListReady()
        {
            EventsHelper.Fire(_orderedStudyListReadyEvent, this, EventArgs.Empty);
        }

        public void FireOrderedStudyListRequired()
        {
            EventsHelper.Fire(_orderedStudyListRequiredEvent, this, EventArgs.Empty);
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

        public DateTime CreatedTimeStamp
        {
            get { return _createdTimeStamp; }
            set { _createdTimeStamp = value; }
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
        private DateTime _createdTimeStamp;
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
