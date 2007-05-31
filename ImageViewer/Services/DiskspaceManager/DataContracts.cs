using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Services.DiskspaceManager
{

	[DataContract]
	public class DiskspaceManagerServiceConfiguration
	{
        private float _lowWatermark;
        private float _highWatermark;
        private int _checkFrequency;

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
    }
}