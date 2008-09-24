using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	public class QueryNamespace
	{
		public const string Value = "http://www.clearcanvas.ca/dicom/contracts/query";
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class QueryFailedFault
	{
		public QueryFailedFault()
		{
		}

		[DataMember(IsRequired = true)]
		public string Description;
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class DataValidationFault
	{
		public DataValidationFault()
		{
		}

		[DataMember(IsRequired = false)]
		public string Description;
	}
}
