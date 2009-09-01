using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	[MenuAction("show", "imageviewer-contextmenu/MenuImageProperties", "Show")]
	[KeyboardAction("show", "imageviewer-keyboard/ImageProperties", "Show", KeyStroke = XKeys.P)]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ImagePropertiesTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _shelves = new Dictionary<IDesktopWindow, IShelf>();
		
		public ImagePropertiesTool()
		{
		}

		private IShelf ComponentShelf
		{
			get
			{
				if (_shelves.ContainsKey(Context.DesktopWindow))
					return _shelves[Context.DesktopWindow];

				return null;
			}
		}

		public void Show()
		{
			if (ComponentShelf == null)
			{
				try
				{
					IDesktopWindow desktopWindow = Context.DesktopWindow;
					
					ImagePropertiesApplicationComponent component =
						new ImagePropertiesApplicationComponent(Context.DesktopWindow);

					IShelf shelf = ApplicationComponent.LaunchAsShelf(Context.DesktopWindow, component,
						SR.TitleImageProperties, "ImageProperties", ShelfDisplayHint.DockLeft);

					_shelves.Add(Context.DesktopWindow, shelf);
					shelf.Closed += delegate { _shelves.Remove(desktopWindow); };
				}
				catch(Exception e)
				{
					ExceptionHandler.Report(e, Context.DesktopWindow);
				}
			}
			else
			{
				ComponentShelf.Show();
			}
		}
	}
}