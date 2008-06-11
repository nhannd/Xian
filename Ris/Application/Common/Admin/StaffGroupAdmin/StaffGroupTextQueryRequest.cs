using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
	[DataContract]
	public class StaffGroupTextQueryRequest : TextQueryRequest
	{
		public StaffGroupTextQueryRequest()
		{
		}

		public StaffGroupTextQueryRequest(string textQuery, int specificityThreshold, bool electiveGroupsOnly)
			: base(textQuery, specificityThreshold)
		{
			ElectiveGroupsOnly = electiveGroupsOnly;
		}

		/// <summary>
		/// If true, only elective staff groups will be returned.
		/// </summary>
		[DataMember]
		public bool ElectiveGroupsOnly;
	}
}
