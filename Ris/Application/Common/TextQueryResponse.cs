using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class TextQueryResponse<TSummary> : DataContractBase
        where TSummary : DataContractBase
    {
        public TextQueryResponse(bool tooManyMatches, List<TSummary> matches)
        {
            this.Matches = matches;
            this.TooManyMatches = tooManyMatches;
        }

        [DataMember]
        public List<TSummary> Matches;

        [DataMember]
        public bool TooManyMatches;
    }
}
