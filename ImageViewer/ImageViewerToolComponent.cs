using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public abstract class ImageViewerToolComponent : ApplicationComponent
	{
		private IImageViewer _imageViewer;
		private event EventHandler _subjectChanged;

		public ImageViewerToolComponent()
		{

		}

		/// <summary>
		/// Gets/sets the subject <see cref="IImageViewer"/> that this component is associated with.  Note that
		/// null is a valid value.  Setting this property to null dissociates it from any image viewer.
		/// </summary>
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
						_imageViewer.EventBroker.DisplaySetSelected -= OnTileSelected;
					}

					_imageViewer = value;

					// start listening to the new image viewer, if one has been set
					if (_imageViewer != null)
					{
						_imageViewer.EventBroker.DisplaySetSelected += OnTileSelected;
					}
					UpdateFromImageViewer();
				}
			}
		}

		/// <summary>
		/// Notifies the view that the layout subject has changed.  The view should
		/// refresh itself entirely to reflect the state of this component.
		/// </summary>
		public event EventHandler SubjectChanged
		{
			add { _subjectChanged += value; }
			remove { _subjectChanged -= value; }
		}

		#region ApplicationComponent overrides

		/// <summary>
		/// Override of <see cref="ApplicationComponent.Start"/>
		/// </summary>
		public override void Start()
		{
			base.Start();

			UpdateFromImageViewer();
		}

		/// <summary>
		/// Override of <see cref="ApplicationComponent.Stop"/>
		/// </summary>
		public override void Stop()
		{
			base.Stop();
		}

		#endregion

		/// <summary>
		/// Updates this component to reflect the state of the currently selected
		/// image box in the subject image viewer.
		/// </summary>
		protected virtual void UpdateFromImageViewer()
		{
			EventsHelper.Fire(_subjectChanged, this, new EventArgs());
		}

		/// <summary>
		/// Updates the component in response to a change in the selected image box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTileSelected(object sender, DisplaySetSelectedEventArgs e)
		{
			UpdateFromImageViewer();
		}
	}
}
