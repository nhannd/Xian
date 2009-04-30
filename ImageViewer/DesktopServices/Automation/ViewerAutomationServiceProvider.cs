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