using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Jsml;
using System.Reflection;
using System.ServiceModel;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using System.IO;

namespace ClearCanvas.Ris.Application.Services.Jsml
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IJsmlShimService))]
    public class JsmlShimService : ApplicationServiceBase, IJsmlShimService
    {
        #region IJsmlShimService Members

        public GetOperationNamesResponse GetOperationNames(GetOperationNamesRequest request)
        {
            Type contract = Type.GetType(request.ServiceContractName);

            List<string> names = new List<string>();
            foreach (MethodInfo method in contract.GetMethods())
            {
                if (IsServiceOperation(method))
                {
                    names.Add(method.Name);
                }
            }

            GetOperationNamesResponse response = new GetOperationNamesResponse();
            response.OperationNames = names.ToArray();
            return response;
        }

        public InvokeOperationResponse InvokeOperation(InvokeOperationRequest request)
        {
            Type contract = Type.GetType(request.ServiceContractName);
            MethodInfo operation = contract.GetMethod(request.OperationName);
            ParameterInfo[] parameters = operation.GetParameters();
            if (parameters.Length != 1)
                throw new Exception();  // todo fix this

            object service = null;
            try
            {
                service = Platform.GetService(contract);

                object innerRequest = JsmlSerializer.Deserialize(parameters[0].ParameterType, request.RequestJsml.Value);
                object innerResponse = operation.Invoke(service, new object[] { innerRequest });

                InvokeOperationResponse response = new InvokeOperationResponse();
                string responseJsml = JsmlSerializer.Serialize((DataContractBase)innerResponse, "responseData", false);

                response.ResponseJsml = new JsmlBlob(responseJsml);
                return response;
            }
            finally
            {
                if (service != null && service is IDisposable)
                    (service as IDisposable).Dispose();
            }
        }

        #endregion

        private bool IsServiceOperation(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(OperationContractAttribute), false).Length > 0;
        }
    }
}
