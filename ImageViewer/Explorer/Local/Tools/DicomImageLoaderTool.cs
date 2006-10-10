using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Local.Tools
{
	[MenuAction("Open", "explorerlocal-contextmenu/OpenFiles")]
	[Tooltip("Open", "OpenDicomFilesVerbose")]
	[IconSet("Open", IconScheme.Colour, "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png")]
	[ClickHandler("Open", "Open")]
	[EnabledStateObserver("Open", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class DicomImageLoaderTool : Tool<ILocalImageViewerToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;
		
		private LocalImageLoader _imageLoader; 

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

			SetDoubleClickHandler();
		}

		private void SetDoubleClickHandler()
		{
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
			if (_imageLoader == null)
				_imageLoader = new LocalImageLoader();

			try
			{

				IEnumerable<string> studyInstanceUIDs = _imageLoader.Load(this.Context.SelectedPaths);

				foreach (string studyInstanceUID in studyInstanceUIDs)
				{
					ImageViewerComponent viewer = new ImageViewerComponent(studyInstanceUID);
					ApplicationComponent.LaunchAsWorkspace(
						this.Context.DesktopWindow, 
						viewer, 
						"Study", 
						delegate
						{
							viewer.Dispose();
						});
				}
			}
			catch (OpenStudyException ex)
			{
				if (ex.StudyCouldNotBeLoaded)
				{
					Platform.ShowMessageBox(ClearCanvas.ImageViewer.SR.ErrorUnableToLoadStudy);
					return;
				}

				if (ex.AtLeastOneImageFailedToLoad)
				{
					Platform.ShowMessageBox(ClearCanvas.ImageViewer.SR.ErrorAtLeastOneImageFailedToLoad);
					return;
				}
			}
			catch (Exception ex)
			{
				// Just in case.  It's unlikely, but we could also catch:
				//    - DirectoryNotFoundException
				//    - ArgumentNullException
				//    - ArgumentException
				Platform.ShowMessageBox(ex.Message);
			}
		}
	}
}
