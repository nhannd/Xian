using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
	[DataContract]
	public class GetWorklistEditFormChoicesRequest : DataContractBase
	{
		public GetWorklistEditFormChoicesRequest(bool userDefinedWorklist)
		{
			UserDefinedWorklist = userDefinedWorklist;
		}

		/// <summary>
		/// Specifies whether to obtain form data specific to creating user-defined worklists.
		/// </summary>
		[DataMember]
		public bool UserDefinedWorklist;
	}
}
