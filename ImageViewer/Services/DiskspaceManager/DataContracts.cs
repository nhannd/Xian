#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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