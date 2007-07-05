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

					_dicomSendReceiveActivityComponentShelf = ApplicationComponent.LaunchAsShelf
						(
							desktopWindow, container, SR.MenuDicomSendReceiveActivity, ShelfDisplayHint.DockLeft | ShelfDisplayHint.DockAutoHide,
							delegate(IApplicationComponent closingComponent)
							{
								_dicomSendReceiveActivityComponentShelf = null;
							});
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
					_importComponentShelf = ApplicationComponent.LaunchAsShelf(desktopWindow, component, component.Title, ShelfDisplayHint.DockBottom | ShelfDisplayHint.DockAutoHide,
						delegate(IApplicationComponent closingComponent)
						{
							_importComponentShelf = null;
						});
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
					_reindexComponentShelf = ApplicationComponent.LaunchAsShelf(desktopWindow, component, component.Title, ShelfDisplayHint.DockBottom | ShelfDisplayHint.DockAutoHide,
						delegate(IApplicationComponent closingComponent)
						{
							_reindexComponentShelf = null;
						});
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
