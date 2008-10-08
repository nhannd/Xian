using System;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.Automation;
using System.Threading;

namespace ClearCanvas.ImageViewer.DesktopServices.Automation
{
	[ExtensionOf(typeof(ServiceProviderExtensionPoint))]
	public class ViewerAutomationServiceProvider : IServiceProvider
	{
		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IViewerAutomation))
				return new ViewerAutomationProxy();

			return null;
		}

		#endregion
	}

	internal class ViewerAutomationProxy : IViewerAutomation, IDisposable
	{
		#region IViewerAutomation Members

		public GetActiveViewerSessionsResult GetActiveViewerSessions()
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationTool.HostSynchronizationContext)
			{
				return new ViewerAutomation().GetActiveViewerSessions();
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					return client.GetActiveViewerSessions();
				}
			} 
		}

		public GetViewerSessionInfoResult GetViewerSessionInfo(GetViewerSessionInfoRequest request)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationTool.HostSynchronizationContext)
			{
				return new ViewerAutomation().GetViewerSessionInfo(request);
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					return client.GetViewerSessionInfo(request);
				}
			} 
		}

		public OpenStudiesResult OpenStudies(OpenStudiesRequest request)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationTool.HostSynchronizationContext)
			{
				return new ViewerAutomation().OpenStudies(request);
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					return client.OpenStudies(request);
				}
			} 
		}

		public void ActivateViewerSession(ActivateViewerSessionRequest request)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationTool.HostSynchronizationContext)
			{
				new ViewerAutomation().ActivateViewerSession(request);
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					client.ActivateViewerSession(request);
				}
			} 
		}

		public void CloseViewerSession(CloseViewerSessionRequest request)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationTool.HostSynchronizationContext)
			{
				new ViewerAutomation().CloseViewerSession(request);
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					client.CloseViewerSession(request);
				}
			} 
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
	
	internal class ViewerAutomationServiceClient : ClientBase<IViewerAutomation>, IViewerAutomation
	{
		public ViewerAutomationServiceClient()
		{
		}

		#region IViewerAutomation Members

		public GetActiveViewerSessionsResult GetActiveViewerSessions()
		{
			return base.Channel.GetActiveViewerSessions();
		}

		public GetViewerSessionInfoResult GetViewerSessionInfo(GetViewerSessionInfoRequest request)
		{
			return base.Channel.GetViewerSessionInfo(request);
		}

		public OpenStudiesResult OpenStudies(OpenStudiesRequest request)
		{
			return base.Channel.OpenStudies(request);
		}

		public void ActivateViewerSession(ActivateViewerSessionRequest request)
		{
			base.Channel.ActivateViewerSession(request);
		}

		public void CloseViewerSession(CloseViewerSessionRequest request)
		{
			base.Channel.CloseViewerSession(request);
		}

		#endregion
	}
}