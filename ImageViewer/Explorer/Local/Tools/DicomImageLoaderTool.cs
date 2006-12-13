using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Explorer.Local.Tools
{
	[MenuAction("Open", "explorerlocal-contextmenu/OpenFiles")]
	[Tooltip("Open", "OpenDicomFilesVerbose")]
	[IconSet("Open", IconScheme.Colour, "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png")]
	[ClickHandler("Open", "Open")]
	[EnabledStateObserver("Open", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class DicomImageLoaderTool : Tool<ILocalImageExplorerToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public DicomImageLoaderTool()
		{
			_enabled = true;
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			this.Context.DefaultActionHandler = Open;
		}

		/// <summary>
		/// Called to determine whether this tool is enabled/disabled in the UI.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Notifies that the Enabled state of this tool has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public void Open()
		{
			LocalImageLoader imageLoader = new LocalImageLoader();
			DiagnosticImageViewerComponent viewer = new DiagnosticImageViewerComponent();
			int totalImages = 0;
			int totalFailedImages = 0;

			foreach (string path in this.Context.SelectedPaths)
			{
				int failedImages;
				int numImages;

				List<LocalImageSop> images = imageLoader.Load(path, out numImages, out failedImages);

				foreach (LocalImageSop image in images)
				{
					ImageViewerComponent.StudyManager.StudyTree.AddImage(image);
					viewer.AddImage(image.SopInstanceUID);
				}

				totalImages += numImages;
				totalFailedImages += failedImages;
			}

			if (totalFailedImages > 0)
			{
				string str = String.Format("{0} images failed to load", totalFailedImages);
				Platform.ShowMessageBox(str);
			}

			// If the number of images that failed to load equals the total number
			// of images that should have loaded, then don't even bother
			// opening the workspace; just return.
			if (totalFailedImages == totalImages)
				return;

			ApplicationComponent.LaunchAsWorkspace(
				this.Context.DesktopWindow,
				viewer,
				"Image",
				delegate
				{
					viewer.Dispose();
				});

			viewer.Layout();
		}
	}
}
