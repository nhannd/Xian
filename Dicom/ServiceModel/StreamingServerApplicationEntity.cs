using System.Runtime.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel
{
	[DataContract]
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
