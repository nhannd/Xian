#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Services;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.Enterprise.Core.ServiceModel;

namespace ClearCanvas.Ris.Server
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class Application : Shred, IApplicationRoot
	{
        private ServiceMount _serviceMount;
		private bool _isStarted;

		#region IApplicationRoot Members

		void IApplicationRoot.RunApplication(string[] args)
		{
			StartUp();

			Console.WriteLine("PRESS ANY KEY TO EXIT");
			Console.Read();

			ShutDown();
		}

		#endregion

		#region Shred overrides

		public override void Start()
		{
			if(!_isStarted)
			{
				StartUp();
			}
		}

		public override void Stop()
		{
			if(_isStarted)
			{
				ShutDown();
			}
		}

		public override string GetDisplayName()
		{
			return SR.TitleRisServer;
		}

		public override string GetDescription()
		{
			return SR.MessageRisServerDescription;
		}

		#endregion

		#region Private Helpers

		private void StartUp()
		{
			Platform.Log(LogLevel.Info, "Starting application root {0}", this.GetType().FullName);

            _serviceMount = new ServiceMount(
                    new Uri(WebServicesSettings.Default.BaseUrl),
                    WebServicesSettings.Default.ConfigurationClass);

            _serviceMount.EnablePerformanceLogging = WebServicesSettings.Default.EnablePerformanceLogging;
            _serviceMount.MaxReceivedMessageSize = WebServicesSettings.Default.MaxReceivedMessageSize;
            _serviceMount.SendExceptionDetailToClient = WebServicesSettings.Default.SendExceptionDetailToClient;

            _serviceMount.AddServices(new CoreServiceExtensionPoint());
            _serviceMount.AddServices(new ApplicationServiceExtensionPoint());


            Platform.Log(LogLevel.Info, "Starting WCF services on {0}...", WebServicesSettings.Default.BaseUrl);

            _serviceMount.OpenServices();

            Platform.Log(LogLevel.Info, "WCF Services started on {0}", WebServicesSettings.Default.BaseUrl);

			// kick NHibernate, rather than waiting for it to load on demand
			PersistentStoreRegistry.GetDefaultStore();

			_isStarted = true;
		}

		private void ShutDown()
		{
			Platform.Log(LogLevel.Info, "Stopping WCF services...");

            _serviceMount.CloseServices();

			Platform.Log(LogLevel.Info, "WCF services stopped.");

			_isStarted = false;
		}

		#endregion
	}
}
