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
