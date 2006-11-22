using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	public class ImageViewerDesktopTool : Tool<IDesktopToolContext>
	{
		private ImageViewerToolComponent _imageViewerToolComponent;

		public ImageViewerDesktopTool()
		{

		}

		public ImageViewerToolComponent ImageViewerToolComponent
		{
			get { return _imageViewerToolComponent; }
			set { _imageViewerToolComponent = value; }
		}

		public override void Initialize()
		{
			this.Context.DesktopWindow.WorkspaceManager.ActiveWorkspaceChanged += OnWorkspaceActivated;
			base.Initialize();
		}

		/// <summary>
		/// Associate the layout component with the active workspace
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnWorkspaceActivated(object sender, WorkspaceActivationChangedEventArgs e)
		{
			if (_imageViewerToolComponent != null)
			{
				_imageViewerToolComponent.Subject = GetSubjectImageViewer();
			}
		}

		/// <summary>
		/// Gets a reference to the <see cref="IImageViewer"/> hosted by the active workspace,
		/// if it exists, otherwise null.
		/// </summary>
		/// <returns></returns>
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
