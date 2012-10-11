#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using ClearCanvas.Common;
using ClearCanvas.Web.Common;

namespace ClearCanvas.Web.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true,
        InstanceContextMode = InstanceContextMode.PerSession,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        AddressFilterMode = AddressFilterMode.Prefix)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PollingApplicationService : ApplicationService, IPollingApplicationService
    {
        public GetPendingEventsResult GetPendingEvents(GetPendingEventsRequest request)
        {
            Application application = Application.Find(request.ApplicationId);
            if (application != null)
            {
                var eventQueue = (EventQueue)application.EventDeliveryStrategy;
                try
                {
                    var response = new GetPendingEventsResult
                    {
                        ApplicationId = application.Identifier,
                        EventSet = eventQueue.GetPendingEvents(Math.Max(0, request.MaxWaitTime))
                    };

                    return response;
                }
                catch (Exception)
                {
                    // TODO (CR Sep 2012): What if it happened not on shutdown?

                    // This happens on shutdown, just return an empty response.
                    return new GetPendingEventsResult
                    {
                        ApplicationId = application.Identifier,
                    };
                }
            }

            // Without a permanent connection, there's a chance the client is polling even when the application has stopped on the server.
            // Throw fault exception to tell the client to stop.
            string reason = String.Format("Could not find the specified ApplicationId: {0}", request.ApplicationId);
            Platform.Log(LogLevel.Error, reason);
            throw new FaultException<InvalidOperationFault>(new InvalidOperationFault(), reason);
        }
    }
}
