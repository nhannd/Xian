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
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common.Automation;

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
	[DesktopServiceHostPermission(new string[] { AuthorityTokens.Study.Open })]
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
