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
	/// listen for changes in <see cref="ITile"/> selection and
	/// <see cref="ImageViewerComponent"/>activation.  For example, the shelf that
	/// appears when the image layout tool activated has UI elements which should
	/// be enabled/disabled depending on whether a <see cref="Tile"/> has been selected
	/// or whether the active workspace contains an <see cref="ImageViewerComponent"/>.
	/// This base class, in conjunction with <see cref="DesktopImageViewerTool"/>,
	/// encapsulates that functionality.  If you are developing an 
	/// <see cref="ApplicationComponent"/> that requires that kind of functionality,
	/// use this as your base class.
	/// </remarks>
	public abstract class ImageViewerToolComponent : ApplicationComponent
	{
		private IImageViewer _imageViewer;
		private event EventHandler _subjectChanged;

		protected ImageViewerToolComponent()
		{

		}

		/// <summary>
		/// Gets/sets the subject <see cref="IImageViewer"/> that this component is associated with.
		/// </summary>
		/// <remarks>
		/// Note that <b>null</b> is a valid value.  Setting this property to null dissociates 
		/// it from any <see cref="IImageViewer"/>.
		/// </remarks>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			set
			{
				if (value != _imageViewer)
				{
					// stop listening to the old image viewer, if one was set
					if (_imageViewer != null)
					{
						_imageViewer.EventBroker.DisplaySetSelected -= OnDisplaySetSelected;
					}

					_imageViewer = value;

					// start listening to the new image viewer, if one has been set
					if (_imageViewer != null)
					{
						_imageViewer.EventBroker.DisplaySetSelected += OnDisplaySetSelected;
					}
					OnSubjectChanged();
				}
			}
		}

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
			add { _subjectChanged += value; }
			remove { _subjectChanged -= value; }
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

		/// <summary>
		/// Raises the <see cref="SubjectChanged"/> event.
		/// </summary>
		protected virtual void OnSubjectChanged()
		{
			EventsHelper.Fire(_subjectChanged, this, EventArgs.Empty);
		}

		private void OnDisplaySetSelected(object sender, DisplaySetSelectedEventArgs e)
		{
			OnSubjectChanged();
		}
	}
}
