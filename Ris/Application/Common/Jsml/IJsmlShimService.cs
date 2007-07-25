using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Jsml
{
    /// <summary>
    /// This is a pass-through service that provides a mechanism for invoking an operation
    /// on another service using JSML-encoded request/response objects rather than native
    /// .NET request/response objects.
    /// </summary>
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
        InvokeOperationResponse InvokeOperation(InvokeOperationRequest request); 
    }
}
