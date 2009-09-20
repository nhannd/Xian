using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel
{
	[DataContract]
	public class UnknownDestinationAEFault
	{
		public UnknownDestinationAEFault()
		{}
	}

	[DataContract]
	public class UnknownCalledAEFault
	{
		public UnknownCalledAEFault()
		{ }
	}

	[DataContract]
	public class UnknownCallingAEFault
	{
		public UnknownCallingAEFault()
		{ }
	}

	[DataContract]
	public class UnknownSourceAEFault
	{
		public UnknownSourceAEFault()
		{ }
	}

	[DataContract]
	public class StudyOfflineFault
	{
		public StudyOfflineFault()
		{ }
	}

	[DataContract]
	public class StudyInUseFault
	{
		public StudyInUseFault()
		{ }
	}

	[DataContract]
	public class StudyNearlineFault
	{
		public StudyNearlineFault()
		{ }
	}

	[DataContract]
	public class StudyNotFoundFault
	{
		public StudyNotFoundFault()
		{ }
	}
}
