#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Web.Common;

namespace ClearCanvas.Web.Services
{
    [ServiceBehavior( IncludeExceptionDetailInFaults = true, 
        InstanceContextMode = InstanceContextMode.PerSession,
        ConcurrencyMode= ConcurrencyMode.Multiple,
        AddressFilterMode = AddressFilterMode.Prefix)]
    [AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Allowed)]
    class ApplicationService : IApplicationService
    {
        static ApplicationService()
        {
            PerformanceMonitor.Initialize();
        }



        private static string GetClientAddress()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint =
                prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            return endpoint!=null? endpoint.Address : "Unknown";
        }


        private static Application FindApplication(Guid applicationId)
		{
			Application application = Application.Find(applicationId);
			if (application == null)
			{
                string reason = string.Format("Could not find the specified app id {0}", applicationId);
                throw new FaultException(reason);
			}

			return application;
		}

        public StartApplicationRequestResponse StartApplication(StartApplicationRequest request)
        {
        	//TODO (CR May 2010): should we be checking the max# of applications?
            bool memoryAvailable = ApplicationServiceSettings.Default.MinimumFreeMemoryMB <= 0
                                   ||
                                   SystemResources.GetAvailableMemory(SizeUnits.Megabytes) >
                                   ApplicationServiceSettings.Default.MinimumFreeMemoryMB;

            if (memoryAvailable)
            {
                try
                {
					OperationContext operationContext = OperationContext.Current;
					// 5 minute timeout, mostly for debugging.
					operationContext.Channel.OperationTimeout = TimeSpan.FromMinutes(5);

					Application application = Application.Start(request);
                    
					//TODO: when we start allowing application recovery, remove these lines.
                    // NOTE: These events are fired only if the underlying connection is permanent (eg, duplex http or net tcp).
					operationContext.Channel.Closed += delegate { application.Stop(); };
					operationContext.Channel.Faulted += delegate { application.Stop(); };

                    return new StartApplicationRequestResponse { AppIdentifier = application.Identifier };
                }
                catch(Enterprise.Common.InvalidUserSessionException ex)
                {
                    throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
                }
                catch (Enterprise.Common.PasswordExpiredException ex)
                {
                    throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
                }
                catch (Enterprise.Common.UserAccessDeniedException ex)
                {
                    throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
                }
                catch (Enterprise.Common.RequestValidationException ex)
                {
                    throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
                }
                catch (Exception ex)
                {
                    throw new FaultException(ExceptionTranslator.Translate(ex));
                } 
            }
            else
            {
                string error =String.Format(
                        "Application server out of resources.  Minimum free memory not available ({0}MB expected, {1}MB available).",
                        ApplicationServiceSettings.Default.MinimumFreeMemoryMB,
                        SystemResources.GetAvailableMemory(SizeUnits.Megabytes));
                
                Platform.Log(LogLevel.Warn, error);

                throw new FaultException(error);
            }
        }

        public ProcessMessagesResult ProcessMessages(MessageSet messageSet)
		{
            IApplication application = FindApplication(messageSet.ApplicationId);
			if (application == null)
				return null;

            try
			{
				return application.ProcessMessages(messageSet);
			}
			catch (Enterprise.Common.InvalidUserSessionException ex)
			{
                Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages"); 
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
			}
			catch (Enterprise.Common.PasswordExpiredException ex)
			{
                Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages"); 
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
			}
			catch (Enterprise.Common.UserAccessDeniedException ex)
			{
                Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages"); 
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
			}
			catch (Enterprise.Common.RequestValidationException ex)
			{
                Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages"); 
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
			}
			catch (Exception ex)
			{
			    Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages");
				throw new FaultException(ExceptionTranslator.Translate(ex));
			}
		}

		public void StopApplication(StopApplicationRequest request)
		{
            IApplication application = FindApplication(request.ApplicationId);
			
			try
			{
			    Platform.Log(LogLevel.Info, "Received application shutdown request from {0}", GetClientAddress());
                   
                application.Shutdown();
			}
			catch (Exception ex)
			{
				throw new FaultException(ExceptionTranslator.Translate(ex)); 
			}
		}

        public void ReportPerformance(PerformanceData data)
        {
            PerformanceMonitor.Initialize();
            PerformanceMonitor.Report(data);
        }

        public EventSet GetPendingEvent(GetPendingEventRequest request)
        {

            IApplication application = Application.Find(request.ApplicationId);

            if (application!=null)
                return application.GetPendingOutboundEvent(Math.Max(0, request.MaxWaitTime));


            // Without a permanent connection, there's a chance the client is polling even when the application has stopped on the server.
            // Throw fault exception to tell the client to stop.
            string reason = string.Format("Could not find the specified app id {0}", request.ApplicationId);
            Platform.Log(LogLevel.Error, reason);
            throw new FaultException<InvalidOperationFault>(new InvalidOperationFault(), reason);

            // TODO:
            // When the app is stopped, it is also removed from the cache. 
            // The client will not get any events fired by the app prior to stopping (eg, when study is not found)
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
