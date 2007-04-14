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
using System.IO;

namespace ClearCanvas.ImageViewer.Explorer.Local.Tools
{
	[MenuAction("Open", "explorerlocal-contextmenu/MenuOpenFiles")]
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
			DiagnosticImageViewerComponent viewer = new DiagnosticImageViewerComponent();

			int successfulLoadAttempts = 0;
			int successfulImagesInLoadFailure = 0;

			string[] files = BuildFileList();

			if (files.Length == 0)
				return;

			bool cancelled = false;

			try
			{
				viewer.LoadImages(files, this.Context.DesktopWindow, out cancelled);
				successfulLoadAttempts++;
			}
			catch (OpenStudyException e)
			{
				successfulImagesInLoadFailure += e.SuccessfulImages;
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}

			if (cancelled || 
				(successfulLoadAttempts == 0 && successfulImagesInLoadFailure == 0))
				return;

			ApplicationComponent.LaunchAsWorkspace(
				this.Context.DesktopWindow,
				viewer,
				viewer.PatientsLoadedLabel,
				delegate
				{
					viewer.Dispose();
				});

			viewer.Layout();
			viewer.PhysicalWorkspace.SelectDefaultImageBox();
		}

		private string[] BuildFileList()
		{
			List<string> fileList = new List<string>();

			foreach (string path in this.Context.SelectedPaths)
			{
				if (File.Exists(path))
					fileList.Add(path);
				else if (Directory.Exists(path))
					fileList.AddRange(Directory.GetFiles(path, "*.dcm", SearchOption.AllDirectories));
			}

			return fileList.ToArray();
		}
	}
}
