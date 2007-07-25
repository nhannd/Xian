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
