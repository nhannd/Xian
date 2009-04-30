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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	public class LocalDataStoreActivityMonitorComponentManager
	{
		private static IShelf _importComponentShelf;
		private static IShelf _reindexComponentShelf;
		private static IShelf _dicomSendReceiveActivityComponentShelf;

		private LocalDataStoreActivityMonitorComponentManager()
		{
		}

		public static void ShowSendReceiveActivityComponent(IDesktopWindow desktopWindow)
		{
			if (_dicomSendReceiveActivityComponentShelf != null)
			{
				_dicomSendReceiveActivityComponentShelf.Activate();
			}
			else
			{
				try
				{
					ReceiveQueueApplicationComponent receiveComponent = new ReceiveQueueApplicationComponent();
					SendQueueApplicationComponent sendComponent = new SendQueueApplicationComponent();

					SplitPane topPane = new SplitPane(SR.TitleReceive, receiveComponent, 0.5F);
					SplitPane bottomPane = new SplitPane(SR.TitleSend, sendComponent, 0.5F);

					SplitComponentContainer container = new SplitComponentContainer(topPane, bottomPane, SplitOrientation.Horizontal);

				    _dicomSendReceiveActivityComponentShelf = ApplicationComponent.LaunchAsShelf(
				        desktopWindow, container,
				        SR.MenuDicomSendReceiveActivity,
				        "Send/Receive Activity Monitor",
				        ShelfDisplayHint.DockLeft | ShelfDisplayHint.DockAutoHide);
				    _dicomSendReceiveActivityComponentShelf.Closed +=
				        delegate
				        {
				            _dicomSendReceiveActivityComponentShelf = null;
				        };
				}
				catch
				{
					_dicomSendReceiveActivityComponentShelf = null;
					throw;
				}
			}
		}

		public static void ShowImportComponent(IDesktopWindow desktopWindow)
		{
			if (_importComponentShelf != null)
			{
				_importComponentShelf.Activate();
			}
			else
			{
				try
				{
					DicomFileImportApplicationComponent component = new DicomFileImportApplicationComponent();
				    _importComponentShelf = ApplicationComponent.LaunchAsShelf(
				        desktopWindow, component,
				        component.Title,
				        "Import",
				        ShelfDisplayHint.DockBottom | ShelfDisplayHint.DockAutoHide);
				    _importComponentShelf.Closed +=
				        delegate
				        {
				            _importComponentShelf = null;
				        };
				}
				catch
				{
					_importComponentShelf = null;
					throw;
				}
			}
		}

		public static void ShowReindexComponent(IDesktopWindow desktopWindow)
		{
			if (_reindexComponentShelf != null)
			{
				_reindexComponentShelf.Activate();
			}
			else
			{
				try
				{
					LocalDataStoreReindexApplicationComponent component = new LocalDataStoreReindexApplicationComponent();
				    _reindexComponentShelf = ApplicationComponent.LaunchAsShelf(
				        desktopWindow, component,
				        component.Title,
				        "Reindex",
				        ShelfDisplayHint.DockBottom | ShelfDisplayHint.DockAutoHide);
				    _reindexComponentShelf.Closed +=
				        delegate
				        {
				            _reindexComponentShelf = null;
				        };
				}
				catch
				{
					_reindexComponentShelf = null;
					throw;
				}
			}
		}
	}
}
