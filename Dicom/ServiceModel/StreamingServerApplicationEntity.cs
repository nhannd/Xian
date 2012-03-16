#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel
{
	[DataContract(Namespace = DicomNamespace.Value)]
	public class StreamingServerApplicationEntity : DicomServerApplicationEntity, IStreamingServerApplicationEntity
	{
		public StreamingServerApplicationEntity()
		{}

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
	}
}
