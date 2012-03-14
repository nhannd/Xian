#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.Common.LocalDataStore;

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
			_localDataStoreEventBroker.SendProgressUpdate += OnSendProgressUpdate;
			_localDataStoreEventBroker.InstanceDeleted += OnInstanceDeleted;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_localDataStoreEventBroker != null)
				{
					_localDataStoreEventBroker.InstanceDeleted -= OnInstanceDeleted;
					_localDataStoreEventBroker.SendProgressUpdate -= OnSendProgressUpdate;
					_localDataStoreEventBroker.Dispose();
					_localDataStoreEventBroker = null;
				}
			}

			_synchronizationContext = null;

			base.Dispose(disposing);
		}

		private static void OnSendProgressUpdate(object sender, ItemEventArgs<SendProgressItem> e)
		{
			if (e.Item.HasErrors && e.Item.MessageType == MessageType.Current)
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

		private static bool _isDeleteFailureMessageShowing = false;

		private static void OnInstanceDeleted(object sender, ItemEventArgs<DeletedInstanceInformation> e)
		{
			if (e.Item.Failed)
			{
				// don't show error message if it's already showing!
				if (_isDeleteFailureMessageShowing)
					return;

				var desktopWindow = Application.ActiveDesktopWindow;
				if (desktopWindow == null)
					return;

				_isDeleteFailureMessageShowing = true;
				try
				{
					desktopWindow.ShowMessageBox(SR.MessageFailedToDeleteOneOrMoreStudies, MessageBoxActions.Ok);
				}
				finally
				{
					_isDeleteFailureMessageShowing = false;
				}
			}
		}
	}
}