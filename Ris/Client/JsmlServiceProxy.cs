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

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Service proxy for use by javascript code.
    /// <code>
    ///     getService: function(serviceContractName)
    ///     {
    ///         // get the service proxy COM object
    ///         var proxy = window.external.GetServiceProxy(serviceContractName);
    /// 
    ///         // add a function to the proxy for each operation
    ///         for(var operationName in proxy.OperationNames)
    ///         {
    ///             proxy[operationName] = function(request)
    ///                 {
    ///                     var response = proxy.Invoke(operationName, JSML.create(request, "data"));
    ///                     return JSML.parse(response);
    ///                 }
    ///         }
    ///         return proxy;
    ///     }
    /// </code>
    /// </summary>
    [ComVisible(true)]
    public class JsmlServiceProxy
    {
        #region ServiceRegistry class

        class ServiceRegistry
        {
            private static ServiceRegistry _instance;
            private static object _syncLock = new object();

            public static Type Lookup(string serviceContractName)
            {
                if (_instance == null)
                {
                    lock (_syncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ServiceRegistry();
                        }
                    }
                }

                lock (_syncLock)
                {
                    Type contract = null;
                    _instance._serviceContractMap.TryGetValue(serviceContractName, out contract);
                    return contract;
                }
            }

            private Dictionary<string, Type> _serviceContractMap;

            private ServiceRegistry()
            {
                _serviceContractMap = new Dictionary<string, Type>();
                foreach (PluginInfo plugin in Platform.PluginManager.Plugins)
                {
                    foreach (Type t in plugin.Assembly.GetTypes())
                    {
                        if (IsServiceContract(t))
                            _serviceContractMap.Add(t.FullName, t);
                    }
                }
            }

            private bool IsServiceContract(Type t)
            {
                return t.GetCustomAttributes(typeof(ServiceContractAttribute), false).Length > 0;
            }
        }

        #endregion

        #region Operation class

        class Operation
        {
            private Type _serviceContract;
            private MethodInfo _operationMethod;
            private Type _requestContract;

            public Operation(Type serviceContract, MethodInfo operationMethod)
            {
                _serviceContract = serviceContract;
                _operationMethod = operationMethod;
                ParameterInfo[] parameters = operationMethod.GetParameters();
                if (parameters.Length > 0)
                {
                    _requestContract = parameters[0].ParameterType;
                }
            }

            public string Invoke(string requestJsml)
            {
                object request = JsmlSerializer.Deserialize(_requestContract, requestJsml);
                object service = null;
                try
                {
                    service = Platform.GetService(_serviceContract);
                    object response = _operationMethod.Invoke(service, new object[] { request });
                    return JsmlSerializer.Serialize((DataContractBase)response, false);
                }
                finally
                {
                    if (service != null && service is IDisposable)
                        (service as IDisposable).Dispose();
                }
            }
        }

        #endregion

        private Type _serviceContract;
        private Dictionary<string, Operation> _operationMap;

        public JsmlServiceProxy(string serviceContractInterfaceName)
        {
            // resolve service contract type
            _serviceContract = ServiceRegistry.Lookup(serviceContractInterfaceName);

            // build the operation map
            _operationMap = new Dictionary<string, Operation>();
            foreach (MethodInfo method in _serviceContract.GetMethods())
            {
                if (IsServiceOperation(method))
                {
                    _operationMap.Add(method.Name, new Operation(_serviceContract, method));
                }
            }
        }

        public string[] OperationNames
        {
            get
            {
                string[] names = new string[_operationMap.Count];
                _operationMap.Keys.CopyTo(names, 0);
                return names;
            }
        }

        public string Invoke(string operationName, string requestJsml)
        {
            return _operationMap[operationName].Invoke(requestJsml);
        }

        private bool IsServiceOperation(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(OperationContractAttribute), false).Length > 0;
        }
    }
}
