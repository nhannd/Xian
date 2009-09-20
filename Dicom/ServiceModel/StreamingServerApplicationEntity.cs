using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel
{
	[DataContract]
	public class StreamingServerApplicationEntity : DicomServerApplicationEntity
	{
		public StreamingServerApplicationEntity()
		{}

		public StreamingServerApplicationEntity(string aeTitle, string hostName, int port, int headerServicePort, int wadoServicePort)
			: this(aeTitle, hostName, port, headerServicePort, wadoServicePort, "", "")
		{
		}

		public StreamingServerApplicationEntity(string aeTitle, string hostName, int port, int headerServicePort, int wadoServicePort, string description)
			: this(aeTitle, hostName, port, headerServicePort, wadoServicePort, description, "")
		{
		}

		public StreamingServerApplicationEntity(string aeTitle, string hostName, int port, int headerServicePort, int wadoServicePort, string description, string location)
			: base(aeTitle, hostName, port, description, location)
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
