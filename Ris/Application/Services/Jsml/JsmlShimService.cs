#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Jsml;

namespace ClearCanvas.Ris.Application.Services.Jsml
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IJsmlShimService))]
    public class JsmlShimService : ApplicationServiceBase, IJsmlShimService
    {
        #region IJsmlShimService Members

        public GetOperationNamesResponse GetOperationNames(GetOperationNamesRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ServiceContractName, "ServiceContractName");

            GetOperationNamesResponse response = new GetOperationNamesResponse();
        	response.OperationNames = ShimUtil.GetOperationNames(request.ServiceContractName);
            return response;
        }

        public InvokeOperationResponse InvokeOperation(InvokeOperationRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.ServiceContractName, "ServiceContractName");
			Platform.CheckMemberIsSet(request.OperationName, "OperationName");
			Platform.CheckMemberIsSet(request.RequestJsml, "RequestJsml");

        	string responseJsml = ShimUtil.InvokeOperation(
				request.ServiceContractName, request.OperationName, request.RequestJsml.Value);

			InvokeOperationResponse response = new InvokeOperationResponse();
			response.ResponseJsml = new JsmlBlob(responseJsml);
			return response;
        }

        #endregion
    }
}
