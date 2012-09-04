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

namespace ClearCanvas.Ris.Application.Common.Jsml
{
    [DataContract]
    public class GetOperationNamesRequest : DataContractBase
    {
        public GetOperationNamesRequest(string serviceContractName)
        {
            this.ServiceContractName = serviceContractName;
        }

        /// <summary>
        /// The name of the service contract to query.  This must be an assembly-qualified name.
        /// </summary>
        [DataMember]
        public string ServiceContractName;
    }
}
