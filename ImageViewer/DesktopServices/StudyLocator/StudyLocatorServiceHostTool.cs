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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.DesktopServices.StudyLocator
{
	/// <summary>
	/// For internal use only.
	/// </summary>
	//[ButtonAction("test", "global-menus/Test/Test Study Locator Client", "TestClient")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class StudyLocatorServiceHostTool : DesktopServiceHostTool
	{
		public StudyLocatorServiceHostTool()
		{
		}

		protected override ServiceHost CreateServiceHost()
		{
			ServiceHost host = new ServiceHost(typeof(Configuration.StudyLocator));
			foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
				endpoint.Binding.Namespace = QueryNamespace.Value;

			return host;
		}

		private void TestClient()
		{
			try
			{
				using (StudyRootQueryBridge bridge = new StudyRootQueryBridge(Platform.GetService<IStudyRootQuery>()))
				{
					bridge.QueryByAccessionNumber("test");
				}

				base.Context.DesktopWindow.ShowMessageBox("Success!", MessageBoxActions.Ok);
			}
			catch (Exception e)
			{
				base.Context.DesktopWindow.ShowMessageBox(e.Message, MessageBoxActions.Ok);
			}
		}
	}
}
