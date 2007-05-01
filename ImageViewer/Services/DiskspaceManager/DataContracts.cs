using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Services.DiskspaceManager
{

	[DataContract]
	public class ServiceConfiguration
	{
        private float _lowWatermark;
        private float _highWatermark;
        private int _checkFrequency;

		public ServiceConfiguration()
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
	public class ServiceInformation
	{
        private string _driveName;
		private long _driveSize;
		private long _usedSpace;
		private float _lowWatermark;
        private float _highWatermark;
        private int _checkFrequency;

		public ServiceInformation()
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