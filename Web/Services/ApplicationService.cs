#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Web.Common;

namespace ClearCanvas.Web.Services
{
    public abstract class ApplicationService : IApplicationService
    {
        protected static Application FindApplication(Guid applicationId)
		{
			Application application = Application.Find(applicationId);
			if (application == null)
			{
                string reason = String.Format("Could not find the specified app id {0}", applicationId);
                throw new FaultException(reason);
			}

			return application;
		}

        /// <summary>
        /// Ensure number of applicatin
        /// </summary>
        protected static void CheckNumberOfApplications()
        {
            var settings = new ApplicationServiceSettings();
            if (settings.MaximumSimultaneousApplications <= 0)
                return;

            Cache cache = Cache.Instance;
            lock (cache.SyncLock)
            {
                // Note: because app is added into the cache ONLY when it has successfully started, there's a small chance that 
                // this check will fail and # of app actually will exceed the max allowed. Decided to live with it for now.
                if (cache.Count >= settings.MaximumSimultaneousApplications)
                {
                    Platform.Log(LogLevel.Warn, "Refuse to start: Max # of Simultaneous Applications ({0}) has been reached.", settings.MaximumSimultaneousApplications);
                    throw new FaultException<OutOfResourceFault>(new OutOfResourceFault { ErrorMessage = SR.MessageMaxApplicationsAllowedExceeded });
                }
            }
        }

        protected static void CheckMemoryAvailable()
        {
            var settings = new ApplicationServiceSettings();

            if (settings.MinimumFreeMemoryMB <= 0)
                return;

            bool memoryAvailable = SystemResources.GetAvailableMemory(SizeUnits.Megabytes) > settings.MinimumFreeMemoryMB;

            if (!memoryAvailable)
            {
                string error = String.Format(
                    "Application server out of resources.  Minimum free memory not available ({0}MB required, {1}MB available).",
                    settings.MinimumFreeMemoryMB,
                    SystemResources.GetAvailableMemory(SizeUnits.Megabytes));

                Platform.Log(LogLevel.Warn, error);
                throw new FaultException<OutOfResourceFault>(new OutOfResourceFault { ErrorMessage = error });
            }
        }

        protected static string GetClientAddress()
        {
            OperationContext context = OperationContext.Current;
            if (context != null)
            {
                MessageProperties prop = context.IncomingMessageProperties;
                var endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

                return endpoint != null ? endpoint.Address : "Unknown";
            }

            return null;
        }

        public StartApplicationResult StartApplication(StartApplicationRequest request)
        {
            CheckNumberOfApplications();
            // TODO (Phoenix5): Restore
            //CheckMemoryAvailable();

            try
            {
                OperationContext operationContext = OperationContext.Current;
                if (operationContext != null)
                {
                    // 5 minute timeout, mostly for debugging.
                    operationContext.Channel.OperationTimeout = TimeSpan.FromMinutes(5);
                }

                Application application = Application.Start(request, new EventQueue());

                //TODO: when we start allowing application recovery, remove these lines.
                // NOTE: These events are fired only if the underlying connection is permanent (eg, duplex http or net tcp).

                // Commented out per CR 3/22/2011, don't want the contenxt to reference the application
                //operationContext.Channel.Closed += delegate { application.Stop(); };
                //operationContext.Channel.Faulted += delegate { application.Stop(); };

                return new StartApplicationResult { ApplicationId = application.Identifier };
            }
            catch (InvalidUserSessionException ex)
            {
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
            }
            catch (PasswordExpiredException ex)
            {
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
            }
            catch (UserAccessDeniedException ex)
            {
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
            }
            catch (RequestValidationException ex)
            {
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
            }
            catch (Exception ex)
            {
                throw new FaultException(ExceptionTranslator.Translate(ex));
            }
        }

        public ProcessMessagesResult ProcessMessages(MessageSet messageSet)
        {
            Application application = FindApplication(messageSet.ApplicationId);
            if (application == null)
                return null;

            try
            {
                var eventQueue = (EventQueue) application.EventDeliveryStrategy;
                application.ProcessMessages(messageSet);
                // TODO (Phoenix5): This Pending thing used for anything? Restore.
                return new ProcessMessagesResult {EventSet = eventQueue.GetPendingEvents(5), Pending = false };
            }
            catch (InvalidUserSessionException ex)
            {
                Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages");
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
            }
            catch (PasswordExpiredException ex)
            {
                Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages");
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
            }
            catch (UserAccessDeniedException ex)
            {
                Platform.Log(LogLevel.Error, ex, "Error has occurred in ProcessMessages");
                throw new FaultException<SessionValidationFault>(new SessionValidationFault { ErrorMessage = ExceptionTranslator.Translate(ex) });
            }
            catch (RequestValidationException ex)
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
            try
            {
                IApplication application = FindApplication(request.ApplicationId);

                Platform.Log(LogLevel.Info, "Received application shutdown request from {0}", GetClientAddress());

                application.Stop();
            }
            catch (Exception ex)
            {
                throw new FaultException(ExceptionTranslator.Translate(ex));
            }
        }
    }
}
