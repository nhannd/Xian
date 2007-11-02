using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
    public class StaffTextQueryRequest : TextQueryRequest
    {
        /// <summary>
        /// If populated, limits the query to the specified staff types. Specified in terms of StaffType codes.
        /// </summary>
        [DataMember]
        public string[] StaffTypesFilter;
    }
}
