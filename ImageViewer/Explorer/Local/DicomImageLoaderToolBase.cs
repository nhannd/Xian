using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	// although there are currently no abstract members, this is marked 
	// as abstract because it cannot be used without being inherited.
	public abstract class DicomImageLoaderToolBase : Tool<ILocalImageExplorerToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		private LocalImageLoader _imageLoader;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public DicomImageLoaderToolBase()
		{
			_enabled = true;
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
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
