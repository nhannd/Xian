using System;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.Automation;

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

		public GetActiveViewersResult GetActiveViewers()
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
			{
				return new ViewerAutomation().GetActiveViewers();
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					return client.GetActiveViewers();
				}
			} 
		}

		public GetViewerInfoResult GetViewerInfo(GetViewerInfoRequest request)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
			{
				return new ViewerAutomation().GetViewerInfo(request);
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					return client.GetViewerInfo(request);
				}
			} 
		}

		public OpenStudiesResult OpenStudies(OpenStudiesRequest request)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
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

		public void ActivateViewer(ActivateViewerRequest request)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
			{
				new ViewerAutomation().ActivateViewer(request);
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					client.ActivateViewer(request);
				}
			} 
		}

		public void CloseViewer(CloseViewerRequest request)
		{
			// Done for reasons of speed, as well as the fact that a call to the service from the same thread
			// that the service is hosted on (the main UI thread) will cause a deadlock.
			if (SynchronizationContext.Current == ViewerAutomationServiceHostTool.HostSynchronizationContext)
			{
				new ViewerAutomation().CloseViewer(request);
			}
			else
			{
				using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
				{
					client.CloseViewer(request);
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
}