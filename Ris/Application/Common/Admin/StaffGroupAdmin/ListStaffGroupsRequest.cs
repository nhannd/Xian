using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
    [DataContract]
	public class ListStaffGroupsRequest : ListRequestBase
    {
        public ListStaffGroupsRequest()
        {
        }

        public ListStaffGroupsRequest(SearchResultPage page)
            :base(page)
        {
        }

		/// <summary>
		/// If true, only elective staff groups will be returned.
		/// </summary>
		[DataMember]
		public bool ElectiveGroupsOnly;
	}
}
