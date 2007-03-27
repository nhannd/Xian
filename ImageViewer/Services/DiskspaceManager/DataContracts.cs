using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Services.DiskspaceManager
{

	[DataContract]
	public class UpdateServerSettingRequest
	{
		private string _driveName;
        private string _status;
        private float _lowWatermark;
        private float _highWatermark;
        private float _usedSpace;
        private int _checkFrequency;

		public UpdateServerSettingRequest()
		{
		}

		[DataMember(IsRequired = true)]
        public string DriveName
		{
            get { return _driveName; }
            set { _driveName = value; }
		}

		[DataMember(IsRequired = true)]
        public string Status
		{
            get { return _status; }
            set { _status = value; }
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
        public float UsedSpace
        {
            get { return _usedSpace; }
            set { _usedSpace = value; }
        }

        [DataMember(IsRequired = true)]
        public int CheckFrequency
        {
            get { return _checkFrequency; }
            set { _checkFrequency = value; }
        }

    }

	[DataContract]
	public class GetServerSettingResponse
	{
        private string _driveName;
        private string _status;
        private float _lowWatermark;
        private float _highWatermark;
        private float _usedSpace;
        private int _checkFrequency;

        public GetServerSettingResponse(string driveName, string status, float lowWatermark, float highWatermark, float usedSpace, int checkFrequency)
		{
		    _driveName = driveName;
            _status = status;
            _lowWatermark = lowWatermark;
            _highWatermark = highWatermark;
            _usedSpace = usedSpace;
            _checkFrequency = checkFrequency;
        }

        [DataMember(IsRequired = true)]
        public string DriveName
        {
            get { return _driveName; }
            set { _driveName = value; }
        }

        [DataMember(IsRequired = true)]
        public string Status
        {
            get { return _status; }
            set { _status = value; }
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
        public float UsedSpace
        {
            get { return _usedSpace; }
            set { _usedSpace = value; }
        }

        [DataMember(IsRequired = true)]
        public int CheckFrequency
        {
            get { return _checkFrequency; }
            set { _checkFrequency = value; }
        }

    }
}