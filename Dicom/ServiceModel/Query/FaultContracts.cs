using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// The namespace for all the query data and service contracts.
	/// </summary>
	public class QueryNamespace
	{
		/// <summary>
		/// The namespace for all the query data and service contracts.
		/// </summary>
		public const string Value = "http://www.clearcanvas.ca/dicom/query";
	}

	/// <summary>
	/// Data contract for 'query failed' faults.
	/// </summary>
	[DataContract(Namespace = QueryNamespace.Value)]
	public class QueryFailedFault
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public QueryFailedFault()
		{
		}

		/// <summary>
		/// A textual description of the query failure.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Description;
	}

	/// <summary>
	/// Data contract for data validation faults; when the data in the request is poorly formatted or incorrect.
	/// </summary>
	[DataContract(Namespace = QueryNamespace.Value)]
	public class DataValidationFault
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public DataValidationFault()
		{
		}

		/// <summary>
		/// A textual description of the fault.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string Description;
	}
}
