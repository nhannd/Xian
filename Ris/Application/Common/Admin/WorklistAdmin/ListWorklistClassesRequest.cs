using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class ListWorklistClassesRequest : DataContractBase
    {
        /// <summary>
        /// Filters the results by the specified class names.
        /// </summary>
        [DataMember]
        public List<string> ClassNames;

        /// <summary>
        /// Filters results by the specified categories.
        /// </summary>
        [DataMember]
        public List<string> Categories;

        /// <summary>
        /// Specifies whether static worklist classes should be included in the results.
        /// </summary>
        [DataMember]
        public bool IncludeStatic;
    }
}
