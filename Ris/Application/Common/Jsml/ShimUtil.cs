#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Reflection;
using System.ServiceModel;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Jsml
{
	/// <summary>
	/// Provides dynamic-dispatch functionality for web-services.
	/// </summary>
	public static class ShimUtil
	{
		/// <summary>
		/// Returns the set of operations defined by the service contract.
		/// </summary>
		/// <param name="serviceContractTypeName"></param>
		/// <returns></returns>
		public static string[] GetOperationNames(string serviceContractTypeName)
		{
			Type contract = Type.GetType(serviceContractTypeName, true);

			return CollectionUtils.Map<MethodInfo, string>(
				CollectionUtils.Select(contract.GetMethods(), IsServiceOperation),
				delegate(MethodInfo m) { return m.Name; }).ToArray(); 
		}

		/// <summary>
		/// Invokes the specified operation on the specified service contract, passing the specified JSML-encoded request object.
		/// </summary>
		/// <param name="serviceContractTypeName"></param>
		/// <param name="operationName"></param>
		/// <param name="jsmlRequest"></param>
		/// <returns></returns>
		public static string InvokeOperation(string serviceContractTypeName, string operationName, string jsmlRequest)
		{
			Type contract = Type.GetType(serviceContractTypeName);
			MethodInfo operation = contract.GetMethod(operationName);
			ParameterInfo[] parameters = operation.GetParameters();
			if (parameters.Length != 1)
				throw new InvalidOperationException("Can only invoke methods with exactly one input parameter.");

			object service = null;
			try
			{
				service = Platform.GetService(contract);

				object innerRequest = JsmlSerializer.Deserialize(parameters[0].ParameterType, jsmlRequest);
				object innerResponse = operation.Invoke(service, new object[] { innerRequest });

				return JsmlSerializer.Serialize(innerResponse, "responseData", false);
			}
			finally
			{
				if (service != null && service is IDisposable)
					(service as IDisposable).Dispose();
			}
		}

		private static bool IsServiceOperation(MethodInfo method)
		{
			return AttributeUtils.HasAttribute<OperationContractAttribute>(method, false);
		}
	}
}
