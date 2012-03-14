#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Common.DiskspaceManager
{

	[DataContract]
	public class DiskspaceManagerServiceConfiguration
	{
        private float _lowWatermark;
        private float _highWatermark;
        private int _checkFrequency;
		private bool _enforceStudyLimit;
		private int _studyLimit;

		public DiskspaceManagerServiceConfiguration()
		{
		}

		[DataMember(IsRequired = true)]
        public float LowWatermark
		{
			get { return _lowWatermark; }
            set { _lowWatermark = value; }
		}

        [DataMember(IsRequired = true)]
        public float HighWatermark
        {
            get { return _highWatermark; }
            set { _highWatermark = value; }
        }

        [DataMember(IsRequired = true)]
        public int CheckFrequency
        {
            get { return _checkFrequency; }
            set { _checkFrequency = value; }
        }

		[DataMember(IsRequired = false)]
		public bool EnforceStudyLimit
		{
			get { return _enforceStudyLimit; }
			set { _enforceStudyLimit = value; }
		}

		[DataMember(IsRequired = false)]
		public int StudyLimit
		{
			get { return _studyLimit; }
			set { _studyLimit = value; }
		}
	}

	[DataContract]
	public class DiskspaceManagerServiceInformation
	{
        private string _driveName;
		private long _driveSize;
		private long _usedSpace;
		private float _lowWatermark;
        private float _highWatermark;
        private int _checkFrequency;
		private int _studyCount;
		private bool _enforceStudyLimit;
		private int _studyLimit;
		private int _maxStudyLimit;
		private int _minStudyLimit;

		public DiskspaceManagerServiceInformation()
		{ 
		}

        [DataMember(IsRequired = true)]
        public string DriveName
        {
            get { return _driveName; }
            set { _driveName = value; }
        }

		[DataMember(IsRequired = true)]
		public long DriveSize
		{
			get { return _driveSize; }
			set { _driveSize = value; }
		}

		[DataMember(IsRequired = true)]
		public long UsedSpace
		{
			get { return _usedSpace; }
			set { _usedSpace = value; }
		}

		[DataMember(IsRequired = true)]
        public float LowWatermark
        {
            get { return _lowWatermark; }
            set { _lowWatermark = value; }
        }

        [DataMember(IsRequired = true)]
        public float HighWatermark
        {
            get { return _highWatermark; }
            set { _highWatermark = value; }
        }

        [DataMember(IsRequired = true)]
        public int CheckFrequency
        {
            get { return _checkFrequency; }
            set { _checkFrequency = value; }
        }

		[DataMember(IsRequired = false)]
		public int StudyCount
		{
			get { return _studyCount; }
			set { _studyCount = value; }
		}

		[DataMember(IsRequired = false)]
		public bool EnforceStudyLimit
		{
			get { return _enforceStudyLimit; }
			set { _enforceStudyLimit = value; }
		}

		[DataMember(IsRequired = false)]
		public int StudyLimit
		{
			get { return _studyLimit; }
			set { _studyLimit = value; }
		}

		[DataMember(IsRequired = false)]
		public int MinStudyLimit
		{
			get { return _minStudyLimit; }
			set { _minStudyLimit = value; }
		}

		[DataMember(IsRequired = false)]
		public int MaxStudyLimit
		{
			get { return _maxStudyLimit; }
			set { _maxStudyLimit = value; }
		}
	}
}
