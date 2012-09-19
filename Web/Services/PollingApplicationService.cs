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
        public void ReportPerformance(PerformanceData data)
        {
            PerformanceMonitor.Report(data);
        }

        public GetPendingEventResult GetPendingEvent(GetPendingEventRequest request)
        {
            IApplication application = Application.Find(request.ApplicationId);

            if (application != null)
            {
                try
                {
                    var response = new GetPendingEventResult
                    {
                        ApplicationId = application.Identifier,
                        EventSet = application.GetPendingOutboundEvent(Math.Max(0, request.MaxWaitTime))
                    };

                    return response;
                }
                catch (Exception)
                {
                    // TODO (CR Sep 2012): What if it happened not on shutdown?

                    // This happens on shutdown, just return an empty response.
                    return new GetPendingEventResult
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

        public void SetProperty(SetPropertyRequest request)
        {
            IApplication application = Application.Find(request.ApplicationId);
            if (application != null)
            {
                application.SetProperty(request.Key, request.Value);
            }
        }
    }
}
