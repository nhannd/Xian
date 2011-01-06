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
using System.ServiceModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Jsml
{
    /// <summary>
    /// This is a pass-through service that provides a mechanism for invoking an operation
    /// on another service using JSML-encoded request/response objects rather than native
    /// .NET request/response objects.
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IJsmlShimService
    {
        /// <summary>
        /// Returns the names of the service operations provided by the specified service.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetOperationNamesResponse GetOperationNames(GetOperationNamesRequest request);

        /// <summary>
        /// Invokes the specified operation on the specified service.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		InvokeOperationResponse InvokeOperation(InvokeOperationRequest request); 
    }
}
