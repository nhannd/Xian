#region License

// Copyright (c) 2010, ClearCanvas Inc.
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