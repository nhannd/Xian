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
