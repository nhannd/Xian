using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A base class for image viewer related application components.
	/// </summary>
	/// <remarks>
	/// There is often a need for an <see cref="ApplicationComponent"/> that will
	/// listen for changes in <see cref="ImageViewerComponent"/>activation.  
	/// For example, the shelf that appears when the image layout tool activated 
	/// has UI elements which should be enabled/disabled depending on whether the 
	/// active workspace contains an <see cref="ImageViewerComponent"/>.
	/// This base class encapsulates that functionality.  If you are developing an 
	/// <see cref="ApplicationComponent"/> that requires that kind of functionality,
	/// use this as your base class.
	/// </remarks>
	public abstract class ImageViewerToolComponent : ApplicationComponent
	{
		private IImageViewerToolContext _imageViewerToolContext;
		private IImageViewer _imageViewer;
		private event EventHandler _subjectChangedEvent;

		protected ImageViewerToolComponent(IImageViewerToolContext imageViewerToolContext)
		{
			_imageViewerToolContext = imageViewerToolContext;
			this.ImageViewer = imageViewerToolContext.Viewer;
			ImageViewerToolContext.DesktopWindow.Workspaces.ItemActivationChanged += OnWorkspaceActivated;
		}

		/// <summary>
		/// Gets the subject <see cref="IImageViewer"/> that this component is associated with.
		/// </summary>
		/// <remarks>
		/// <value>
		/// The currently active <see cref="IImageViewer"/> or <b>null</b> if the
		/// there is no currently active <see cref="IImageViewer"/>.
		/// </value>
		/// </remarks>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			private set
			{
				if (value != _imageViewer)
				{
					IImageViewer oldImageViewer = _imageViewer;
					_imageViewer = value;

					OnActiveImageViewerChanged(
						new ActiveImageViewerChangedEventArgs(
						_imageViewer, 
						oldImageViewer));
				}
			}
		}

		public IImageViewerToolContext ImageViewerToolContext
		{
			get { return _imageViewerToolContext; }
		}

		protected abstract void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e);

		/// <summary>
		/// Occurs when either the selected <see cref="ITile"/> or active
		/// <see cref="ImageViewerComponent"/> has changed.
		/// </summary>
		/// <remarks>
		/// The view should subscribe to this event.  When this event is raised,
		/// the view should refresh itself entirely to reflect the state of the component.		
		/// </remarks>
		public event EventHandler SubjectChanged
		{
			add { _subjectChangedEvent += value; }
			remove { _subjectChangedEvent -= value; }
		}

		#region ApplicationComponent overrides

		/// <summary>
		/// Override of <see cref="ApplicationComponent.Start"/>
		/// </summary>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		public override void Start()
		{
			base.Start();

			OnSubjectChanged();
		}

		/// <summary>
		/// Override of <see cref="ApplicationComponent.Stop"/>
		/// </summary>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		public override void Stop()
		{
			base.Stop();
		}

		#endregion

		private void OnWorkspaceActivated(object sender, ItemEventArgs<Workspace> e)
		{
			Workspace activeWorkspace = ImageViewerToolContext.DesktopWindow.ActiveWorkspace;

			if (activeWorkspace == null)
			{
				this.ImageViewer = null;
			}
			else
			{
				IImageViewer imageViewer = ImageViewerComponent.GetAsImageViewer(activeWorkspace);
				this.ImageViewer = imageViewer;
			}
		}

		/// <summary>
		/// Raises the <see cref="SubjectChanged"/> event.
		/// </summary>
		protected virtual void OnSubjectChanged()
		{
			EventsHelper.Fire(_subjectChangedEvent, this, EventArgs.Empty);
		}
	}
}
