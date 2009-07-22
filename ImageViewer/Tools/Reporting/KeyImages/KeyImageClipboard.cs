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
using System.ServiceModel.Security;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Threading;
using System.Security.Policy;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	public class KeyImageClipboard
	{
		public const string MenuSite = "keyimageclipboard-contextmenu";
		public const string ToolbarSite = "keyimageclipboard-toolbar";

		private static readonly Dictionary<IImageViewer, KeyImageInformation> _keyImageInformation;
		private static readonly Dictionary<IDesktopWindow, IShelf> _clipboardShelves;

		private static ILocalDataStoreEventBroker _localDataStoreEventBroker;

		static KeyImageClipboard()
		{
			_keyImageInformation = new Dictionary<IImageViewer, KeyImageInformation>();
			_clipboardShelves = new Dictionary<IDesktopWindow, IShelf>();
		}

		#region Event Handling

		private static void OnWorkspaceActivated(object sender, ItemEventArgs<Workspace> e)
		{
			IShelf shelf = GetClipboardShelf(e.Item.DesktopWindow);
			if (shelf == null)
				return;

			KeyImageClipboardComponent clipboardComponent = shelf.Component as KeyImageClipboardComponent;
			if (clipboardComponent == null)
				return;

			clipboardComponent.KeyImageInformation = GetKeyImageInformation(e.Item) ?? new KeyImageInformation();
		}

		private static void OnClipboardShelfClosed(object sender, ClosedEventArgs e)
		{
			IShelf clipboardShelf = (IShelf) sender;
			clipboardShelf.Closed -= OnClipboardShelfClosed;

			foreach (KeyValuePair<IDesktopWindow, IShelf> pair in _clipboardShelves)
			{
				if (pair.Value == clipboardShelf)
				{
					_clipboardShelves.Remove(pair.Key);
					break;
				}
			}
		}

		#endregion

		#region Private Methods

		private static IShelf GetClipboardShelf(IDesktopWindow desktopWindow)
		{
			if (_clipboardShelves.ContainsKey(desktopWindow))
				return _clipboardShelves[desktopWindow];
			else 
				return null;
		}

		private static void ManageLocalDataStoreConnection()
		{
			if (_keyImageInformation.Count == 0 && _localDataStoreEventBroker != null)
			{
				try
				{
					_localDataStoreEventBroker.LostConnection -= DummyEventHandler;
					_localDataStoreEventBroker.Dispose();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "Failed to unsubscribe to local data store events.");
				}
				finally
				{
					_localDataStoreEventBroker = null;
				}
			}
			else if (_keyImageInformation.Count > 0 && _localDataStoreEventBroker == null)
			{
				try
				{
					_localDataStoreEventBroker = LocalDataStoreActivityMonitor.CreatEventBroker(true);
					//we subscribe to something to keep the local data store connection open.
					_localDataStoreEventBroker.LostConnection += DummyEventHandler;
				}
				catch (Exception e)
				{
					_localDataStoreEventBroker = null;
					Platform.Log(LogLevel.Warn, e, "Failed to subscribe to local data store events.");
				}
			}
		}

		private static void DummyEventHandler(object sender, EventArgs e)
		{
		}

		#endregion

		#region Internal Methods


		internal static KeyImageInformation GetKeyImageInformation(Workspace workspace)
		{
			IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
			return GetKeyImageInformation(viewer);
		}

		internal static KeyImageInformation GetKeyImageInformation(IImageViewer viewer)
		{
			if (!PermissionsHelper.IsInRole(AuthorityTokens.KeyImages))
				throw new PolicyException(SR.ExceptionViewKeyImagePermissionDenied);

			if (viewer != null)
				return _keyImageInformation[viewer];
			else
				return null;
		}

		internal static KeyImageInformation GetKeyImageInformation(IDesktopWindow desktopWindow)
		{
			IShelf shelf = GetClipboardShelf(desktopWindow);
			if (shelf == null)
				return null;

			if (!PermissionsHelper.IsInRole(AuthorityTokens.KeyImages))
				throw new PolicyException(SR.ExceptionViewKeyImagePermissionDenied);

			KeyImageClipboardComponent component = shelf.Component as KeyImageClipboardComponent;
			if (component != null)
				return component.KeyImageInformation;
			else
				return null;
		}

		#region Event Publishing

		internal static void OnDesktopWindowOpened(IDesktopWindow desktopWindow)
		{
			desktopWindow.Workspaces.ItemActivationChanged += OnWorkspaceActivated;
			_clipboardShelves[desktopWindow] = null;
		}

		internal static void OnDesktopWindowClosed(IDesktopWindow desktopWindow)
		{
			desktopWindow.Workspaces.ItemActivationChanged -= OnWorkspaceActivated;
			_clipboardShelves.Remove(desktopWindow);
		}

		internal static void OnViewerOpened(IImageViewer viewer)
		{
			if (!_keyImageInformation.ContainsKey(viewer))
				_keyImageInformation[viewer] = new KeyImageInformation();

			ManageLocalDataStoreConnection();
		}

		internal static void OnViewerClosed(IImageViewer viewer)
		{
			KeyImageInformation info = GetKeyImageInformation(viewer);
			_keyImageInformation.Remove(viewer);

			if (info != null)
			{				
				using(info)
				{
					try
					{
						new KeyImagePublisher(info, true).Publish();
					}
					catch (Exception e)
					{
						//TODO: show a message box or something?
						Platform.Log(LogLevel.Error, e, "An error occurred while attempting to publish key images.");
					}
				}
			}

			ManageLocalDataStoreConnection();
		}

		#endregion
		#endregion

		#region Public Methods

		public static void Add(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");
			Platform.CheckForNullReference(image.ImageViewer, "image.ImageViewer");

			if (!PermissionsHelper.IsInRole(AuthorityTokens.KeyImages))
				throw new PolicyException(SR.ExceptionCreateKeyImagePermissionDenied);

			KeyImageInformation info = GetKeyImageInformation(image.ImageViewer);
			if (info == null)
				throw new ArgumentException("The specified image's viewer is not valid.", "image");

			IImageSopProvider sopProvider = image as IImageSopProvider;
			if (sopProvider == null)
				throw new ArgumentException("The image must be an IImageSopProvider.", "image");

			info.ClipboardItems.Add(ClipboardComponent.CreatePresentationImageItem(image));
		}

		public static void Show(IDesktopWindow desktopWindow)
		{
			Show(desktopWindow, ShelfDisplayHint.DockLeft);
		}

		public static void Show(ShelfDisplayHint displayHint)
		{
			Show(Application.ActiveDesktopWindow, displayHint);
		}

		public static void Show(IDesktopWindow desktopWindow, ShelfDisplayHint displayHint)
		{
			if (!PermissionsHelper.IsInRole(AuthorityTokens.KeyImages))
				throw new PolicyException(SR.ExceptionViewKeyImagePermissionDenied);

			IShelf shelf = GetClipboardShelf(desktopWindow);
			if (shelf != null)
			{
				shelf.Activate();
			}
			else
			{
				Workspace activeWorkspace = desktopWindow.ActiveWorkspace;
				KeyImageInformation info = GetKeyImageInformation(activeWorkspace) ?? new KeyImageInformation();
				ClipboardComponent component = new KeyImageClipboardComponent(info);
				shelf = ApplicationComponent.LaunchAsShelf(desktopWindow, component, SR.TitleKeyImages, displayHint);
				shelf.Closed += OnClipboardShelfClosed;

				_clipboardShelves[desktopWindow] = shelf;
			}
		}

		public static void Show()
		{
			Show(Application.ActiveDesktopWindow);
		}

		#endregion
	}
}