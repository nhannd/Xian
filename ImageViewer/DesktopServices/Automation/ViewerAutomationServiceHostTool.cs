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
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Services.Automation;

namespace ClearCanvas.ImageViewer.DesktopServices.Automation
{
	/// <summary>
	/// For internal use only.
	/// </summary>
	/// <remarks>
	/// This class is implemented as a desktop tool rather than an application tool in order
	/// to take advantage of the 'UseSynchronizationContext' WCF service behaviour, which
	/// automatically marshals all service request over to the thread on which the service host was
	/// started.
	/// </remarks>

	//[ButtonAction("test", "global-menus/Test/Test Automation Client", "TestClient")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	[DesktopServiceHostPermission(new string[] { Common.AuthorityTokens.Workflow.Study.View })]
	public class ViewerAutomationServiceHostTool : DesktopServiceHostTool
	{
		public ViewerAutomationServiceHostTool()
		{
		}

		protected override ServiceHost CreateServiceHost()
		{
			ServiceHost host = new ServiceHost(typeof(ViewerAutomation));
			foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
				endpoint.Binding.Namespace = AutomationNamespace.Value;

			return host;
		}

		private void TestClient()
		{
			SynchronizationContext context = SynchronizationContext.Current;

			//Have to test client on another thread, otherwise there is a deadlock b/c the service is hosted on the main thread.
			ThreadPool.QueueUserWorkItem(delegate
											{
												try
												{
													using (ViewerAutomationServiceClient client = new ViewerAutomationServiceClient())
													{
														client.GetActiveViewers();
													}

													context.Post(delegate
																	{
																		base.Context.DesktopWindow.ShowMessageBox("Success!", MessageBoxActions.Ok);
																	}, null);
												}
												catch (Exception e)
												{
													context.Post(delegate
													{
														base.Context.DesktopWindow.ShowMessageBox(e.Message, MessageBoxActions.Ok);
													}, null);

												}
											});

		}
	}
}
