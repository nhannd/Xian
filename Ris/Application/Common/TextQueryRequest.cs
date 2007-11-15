using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class TextQueryRequest : PagedDataContractBase
    {
        /// <summary>
        /// The query text.
        /// </summary>
        [DataMember]
        public string TextQuery;

        /// <summary>
        /// The maximum number of allowed matches for which results should be returned.  If the query results in more
        /// matches, it is considered to be not specific enough, and no results are returned. If this value is 0,
        /// it is ignored.
        /// </summary>
        [DataMember]
        public int SpecificityThreshold;

    }
}
