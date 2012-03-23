#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel
{
	[DataContract(Namespace = DicomNamespace.Value)]
    public class StreamingServerApplicationEntity : DicomServerApplicationEntity, IStreamingServerApplicationEntity, IEquatable<StreamingServerApplicationEntity>
	{
		public StreamingServerApplicationEntity()
		{}

        public StreamingServerApplicationEntity(IStreamingServerApplicationEntity other)
            : base(other)
        {
            this.HeaderServicePort = other.HeaderServicePort;
            this.WadoServicePort = other.WadoServicePort;
        }

		public StreamingServerApplicationEntity(string aeTitle, string hostName, int port, int headerServicePort, int wadoServicePort)
			: this(aeTitle, hostName, port, headerServicePort, wadoServicePort, "", "", "")
		{
		}

		public StreamingServerApplicationEntity(string aeTitle, string hostName, int port, int headerServicePort, int wadoServicePort, string name, string description, string location)
			: base(aeTitle, hostName, port, name, description, location)
		{
			HeaderServicePort = headerServicePort;
			WadoServicePort = wadoServicePort;
		}

		[DataMember(IsRequired = true)]
		public int HeaderServicePort { get; set; }

		[DataMember(IsRequired = true)]
		public int WadoServicePort { get; set; }

        public override bool IsStreaming { get { return true; } }

        public override string ToString()
        {
            return String.Format("{0}\nHeaderServicePort: {1}\nWadoServicePort: {2}",
                                 base.ToString(), HeaderServicePort, WadoServicePort);
        }

        public override bool Equals(object obj)
        {
            if (obj is StreamingServerApplicationEntity)
                return Equals((StreamingServerApplicationEntity)obj);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 0x9872357;
            hash ^= HeaderServicePort.GetHashCode();
            hash ^= WadoServicePort.GetHashCode();
            hash ^= base.GetHashCode();
            return hash;
        }

        #region IEquatable<StreamingServerApplicationEntity> Members

        public bool Equals(StreamingServerApplicationEntity other)
        {
            return HeaderServicePort == other.HeaderServicePort
                   && WadoServicePort == other.WadoServicePort
                   && base.Equals(other);
        }

        #endregion
    }
}
