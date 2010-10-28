#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Events;

namespace ClearCanvas.Web.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.PerSession,AddressFilterMode = AddressFilterMode.Prefix)]
    class ApplicationService : IApplicationService
    {
        static ApplicationService()
        {
            PerformanceMonitor.Initialize();
        }

        private static Application FindApplication(Guid applicationId)
		{
			Application application = Application.Find(applicationId);
			if (application == null)
			{
				var notFoundEvent = new ApplicationNotFoundEvent { ApplicationId = applicationId, Identifier = Guid.NewGuid()};
				var callback = OperationContext.Current.GetCallbackChannel<IApplicationServiceCallback>();
				ThreadPool.QueueUserWorkItem(
					delegate
					{
						try
						{
							callback.EventNotification(new EventSet { Events = new Event[] { notFoundEvent } });
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e, "Error sending application not found exception.");
						}
					});

				return null;
			}

			return application;
		}

        public void StartApplication(StartApplicationRequest request)
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

					Application application = Application.Start(request, operationContext.GetCallbackChannel<IApplicationServiceCallback>());

					//TODO: when we start allowing application recovery, remove these lines.
					operationContext.Channel.Closed += delegate { application.Stop(); };
					operationContext.Channel.Faulted += delegate { application.Stop(); };
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

		public void ProcessMessages(MessageSet messageSet)
		{
			IApplication application = FindApplication(messageSet.ApplicationId);
			//FindApplication has already dealt with it (ApplicationNotFoundEvent).
			if (application == null)
				return;

			try
			{
				application.ProcessMessages(messageSet);
			}
			catch (Enterprise.Common.InvalidUserSessionException ex)
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

		public void StopApplication(StopApplicationRequest request)
		{
			IApplication application = FindApplication(request.ApplicationId);
			//FindApplication has already dealt with it (ApplicationNotFoundEvent).
			if (application == null)
				return;

			try
			{
				application.Stop();
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
    }
}
