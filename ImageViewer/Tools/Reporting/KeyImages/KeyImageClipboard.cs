using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	public class KeyImageClipboard
	{
		public const string MenuSite = "keyimageclipboard-contextmenu";
		public const string ToolbarSite = "keyimageclipboard-toolbar";

		private static readonly Dictionary<IImageViewer, KeyImageInformation> _keyImageInformation;
		private static readonly Dictionary<IDesktopWindow, IShelf> _clipboardShelves;

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

		#endregion

		#region Internal Methods

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
		}

		internal static void OnViewerClosed(IImageViewer viewer)
		{
			KeyImageInformation info = GetKeyImageInformation(viewer);
			_keyImageInformation.Remove(viewer);

			if (info != null)
			{				
				using(info)
				{
					info.Publish();
				}
			}
		}
		
		#endregion

		#region Public Methods

		public static void Add(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");
			Platform.CheckForNullReference(image.ImageViewer, "image.ImageViewer");

			KeyImageInformation info = GetKeyImageInformation(image.ImageViewer);
			if (info == null)
				throw new ArgumentException("The specified image's viewer is not valid.", "image");

			IImageSopProvider sopProvider = image as IImageSopProvider;
			if (sopProvider == null)
				throw new ArgumentException("The image must be an IImageSopProvider.", "image");

			info.ClipboardItems.Add(ClipboardComponent.CreatePresentationImageItem(image));
		}

		//TODO: make these internal?
		public static KeyImageInformation GetKeyImageInformation(Workspace workspace)
		{
			IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
			return GetKeyImageInformation(viewer);
		}

		public static KeyImageInformation GetKeyImageInformation(IImageViewer viewer)
		{
			if (viewer != null)
				return _keyImageInformation[viewer];
			else
				return null;
		}

		public static KeyImageInformation GetKeyImageInformation(IDesktopWindow desktopWindow)
		{
			IShelf shelf = GetClipboardShelf(desktopWindow);
			if (shelf == null)
				return null;

			KeyImageClipboardComponent component = shelf.Component as KeyImageClipboardComponent;
			if (component != null)
				return component.KeyImageInformation;
			else
				return null;
		}

		public static void Show(IDesktopWindow desktopWindow)
		{
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
				shelf = ApplicationComponent.LaunchAsShelf(desktopWindow, component, SR.TitleKeyImages, ShelfDisplayHint.DockLeft);
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