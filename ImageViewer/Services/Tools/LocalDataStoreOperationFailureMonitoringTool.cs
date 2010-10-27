#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionOf(typeof (ApplicationToolExtensionPoint))]
	public class LocalDataStoreOperationFailureMonitoringTool : Tool<IApplicationToolContext>
	{
		private SynchronizationContext _synchronizationContext;
		private ILocalDataStoreEventBroker _localDataStoreEventBroker;

		public override void Initialize()
		{
			base.Initialize();

			_synchronizationContext = SynchronizationContext.Current;

			_localDataStoreEventBroker = LocalDataStoreActivityMonitor.CreatEventBroker(_synchronizationContext);
			_localDataStoreEventBroker.SendProgressUpdate += OnProgressUpdate;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_localDataStoreEventBroker != null)
				{
					_localDataStoreEventBroker.SendProgressUpdate -= OnProgressUpdate;
					_localDataStoreEventBroker.Dispose();
					_localDataStoreEventBroker = null;
				}
			}

			_synchronizationContext = null;

			base.Dispose(disposing);
		}

		private static void OnProgressUpdate(object sender, ItemEventArgs<SendProgressItem> e)
		{
			if (e.Item.HasErrors)
			{
				var desktopWindow = Application.ActiveDesktopWindow;
				if (desktopWindow == null)
					return;

				try
				{
					LocalDataStoreActivityMonitorComponentManager.ShowSendReceiveActivityComponent(desktopWindow);

					var sendComponent = LocalDataStoreActivityMonitorComponentManager.SendActivityComponent;
					if (sendComponent != null)
						sendComponent.SetSelection(e.Item.SendOperationReference);
				}
				catch (Exception ex)
				{
					ExceptionHandler.Report(ex, SR.MessageFailedToLaunchSendReceiveActivityComponent, desktopWindow);
				}
			}
		}
	}
}