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
using System.IO;

namespace ClearCanvas.Ris.Application.Common.Jsml
{
    [DataContract]
    public class InvokeOperationRequest : DataContractBase
    {
        public InvokeOperationRequest(string serviceContractName, string operationName, JsmlBlob requestJsml)
        {
            this.ServiceContractName = serviceContractName;
            this.OperationName = operationName;
            this.RequestJsml = requestJsml;
        }

        /// <summary>
        /// The assembly-qualified name of the service contract.
        /// </summary>
        [DataMember]
        public string ServiceContractName;

        /// <summary>
        /// The service operation to invoke.
        /// </summary>
        [DataMember]
        public string OperationName;

        /// <summary>
        /// The request argument to be passed to the service operation.
        /// </summary>
        [DataMember]
        public JsmlBlob RequestJsml;
    }
}
