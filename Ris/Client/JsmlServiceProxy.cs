#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using System.ServiceModel;
using ClearCanvas.Ris.Application.Common.Jsml;
using System.IO;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Service proxy for use by javascript code.
    /// </summary>
    /// <remarks>
    /// This is a COM-visible class that allows javascript code running in a browser to effectively
    /// make use of <see cref="Platform.GetService"/> to obtain an abritrary service and invoke 
    /// operations on it.  For ease of use, a wrapper (outer proxy) may be created in javascript
    /// around this object (innerproxy) that allows service methods to be invoked directly.
    /// An example is shown below.  In this example, the call to window.external.GetServiceProxy
    /// returns an instance of this class.
    /// <code>
    /// getService: function(serviceContractName)
    ///	{
    ///	    var innerProxy = window.external.GetServiceProxy(serviceContractName);
    ///	    var operations = JSML.parse(innerProxy.GetOperationNames());
    ///	    
    ///	    var proxy = { _innerProxy: innerProxy };
    ///	    operations.each(
    ///	        function(operation)
    ///	        {
    ///	            proxy[operation] = 
    ///	                function(request)
    ///	                {
    ///	                    return JSML.parse( this._innerProxy.InvokeOperation(operation, JSML.create(request, "requestData")) );
    ///	                };
    ///	        });
    ///	    return proxy;
    ///	}
    /// </code>
    /// </remarks>
    [ComVisible(true)]
    public class JsmlServiceProxy
    {
        private string _serviceContractName;

        /// <summary>
        /// Constructs a proxy instance.
        /// </summary>
        /// <param name="serviceContractInterfaceName">An assembly-qualified service contract name.</param>
        public JsmlServiceProxy(string serviceContractInterfaceName)
        {
            _serviceContractName = serviceContractInterfaceName;
        }

        /// <summary>
        /// Returns the names of the operations provided by the service.
        /// </summary>
        /// <returns>A JSML-encoded array of operation names.</returns>
        public string GetOperationNames()
        {
            string[] names = null;
            Platform.GetService<IJsmlShimService>(
                delegate(IJsmlShimService service)
                {
                    names = service.GetOperationNames(new GetOperationNamesRequest(_serviceContractName)).OperationNames;
                });
            return JsmlSerializer.Serialize(names, "operationNames");
        }

        /// <summary>
        /// Invokes the specified operation, passing the specified request, and returns the result of the operation.
        /// </summary>
        /// <param name="operationName">The name of the operation to invoke.</param>
        /// <param name="requestJsml">The request object, as JSML.</param>
        /// <returns>The response object, as JSML.</returns>
        public string InvokeOperation(string operationName, string requestJsml)
        {
            string responseJsml = null;
            Platform.GetService<IJsmlShimService>(
                delegate(IJsmlShimService service)
                {
                    InvokeOperationRequest request = new InvokeOperationRequest(_serviceContractName, operationName, new JsmlBlob(requestJsml));
                    responseJsml = service.InvokeOperation(request).ResponseJsml.Value;
                });
            return responseJsml;
        }
    }
}
