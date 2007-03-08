using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// A base class image viewer tool that can notify its associated application
	/// component when the active workspace changes.
	/// </summary>
	public abstract class ComponentImageViewerTool : ImageViewerTool
	{
		private ImageViewerToolComponent _imageViewerToolComponent;

		/// <summary>
		/// Gets or sets the associated <see cref="ImageViewerToolComponent"/>.
		/// </summary>
		public ImageViewerToolComponent ImageViewerToolComponent
		{
			get { return _imageViewerToolComponent; }
			set { _imageViewerToolComponent = value; }
		}

		/// <summary>
		/// Overrides <see cref="ToolBase.Initialize"/>.
		/// </summary>
		public override void Initialize()
		{
			this.Context.DesktopWindow.WorkspaceManager.ActiveWorkspaceChanged += OnWorkspaceActivated;
			base.Initialize();
		}

		private void OnWorkspaceActivated(object sender, WorkspaceActivationChangedEventArgs e)
		{
			if (_imageViewerToolComponent != null)
			{
				_imageViewerToolComponent.ImageViewer = GetSubjectImageViewer();
			}
		}

		/// <summary>
		/// Gets a reference to the <see cref="IImageViewer"/> hosted by the active workspace.
		/// </summary>
		/// <returns>The active <see cref="IImageViewer"/> or <b>null</b> if 
		/// the active workspace does not host an <see cref="IImageViewer"/>.</returns>
		protected IImageViewer GetSubjectImageViewer()
		{
			IWorkspace workspace = this.Context.DesktopWindow.ActiveWorkspace;
			if (workspace is ApplicationComponentHostWorkspace
				&& ((ApplicationComponentHostWorkspace)workspace).Component is IImageViewer)
			{
				return (IImageViewer)((ApplicationComponentHostWorkspace)workspace).Component;
			}
			return null;
		}
	}
}
