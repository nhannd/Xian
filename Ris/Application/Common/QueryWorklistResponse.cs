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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class QueryWorklistResponse<TSummary> : DataContractBase
    {
        public QueryWorklistResponse(List<TSummary> worklistItems, int itemCount)
        {
            this.WorklistItems = worklistItems;
            this.ItemCount = itemCount;
        }

        [DataMember]
        public List<TSummary> WorklistItems;

        [DataMember]
        public int ItemCount;
    }
}
