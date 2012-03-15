#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Serialization;
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
