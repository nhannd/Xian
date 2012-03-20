#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod;
using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel
{
	[DataContract(Namespace = DicomNamespace.Value)]
	public class DicomServerApplicationEntity : ApplicationEntity, IDicomServerApplicationEntity
	{
		public DicomServerApplicationEntity()
		{}

        public DicomServerApplicationEntity(IDicomServerApplicationEntity other)
            : this(other.AETitle, other.HostName, other.Port, other.Name, other.Description, other.Location)
        { }
        
        public DicomServerApplicationEntity(string aeTitle, string hostName, int port)
			: this(aeTitle, hostName, port, "", "", "")
		{
		}

		public DicomServerApplicationEntity(string aeTitle, string hostName, int port, string name, string description, string location)
			: base(aeTitle, name, description, location)
		{
			HostName = hostName;
			Port = port;
		}

		#region IDicomServerApplicationEntity Members

		[DataMember(IsRequired = true)]
		public string HostName { get; set; }

		[DataMember(IsRequired = true)]
		public int Port { get; set; }

		#endregion

        public override bool IsStreaming { get { return true; } }
    }
}
