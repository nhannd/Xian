using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Describes a presentation image
	/// </summary>
	public abstract class PresentationImage : IDrawable, IUIEventHandler
	{
		private DisplaySet _parentDisplaySet;
		private LayerManager _layerManager;
		private bool _selected = false;
		private bool _linked = true;

		private event EventHandler<ImageDrawingEventArgs> _imageDrawingEvent;
		private event EventHandler<LinkageChangedEventArgs> _linkageChangedEvent;

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentationImage"/> class.
		/// </summary>
		protected PresentationImage()
		{
		}

		/// <summary>
		/// Gets the parent <see cref="DisplaySet"/>.
		/// </summary>
		/// <value>Can be <b>null</b> if <see cref="PresentationImage"/> has not been
		/// added to a <see cref="DisplaySet"/>.</value>
		public DisplaySet ParentDisplaySet
		{
			get { return _parentDisplaySet; }
			set
			{
				Platform.CheckForNullReference(value, "ParentDisplaySet");
				_parentDisplaySet = value;
			}
		}

		/// <summary>
		/// Gets the parent <see cref="ImageWorkspace"/>.
		/// </summary>
		/// <value>Can be <b>null</b> if <see cref="PresentationImage"/> has not been
		/// added to a <see cref="DisplaySet"/>.</value>
		public IImageViewer ParentViewer
		{
			get 
			{
				if (this.ParentDisplaySet == null)
					return null;

				return this.ParentDisplaySet.ParentViewer; 
			}
		}

		/// <summary>
		/// Gets the <see cref="LayerManager"/>.
		/// </summary>
		public LayerManager LayerManager
		{
			get 
			{
				if (_layerManager == null)
					_layerManager = new LayerManager(this);
				
				return _layerManager; 
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="PresentationImage"/> is visible.
		/// </summary>
		public bool Visible
		{
			get
			{
				// The image is visible as long as one or more clients subscribe
				// to the ImageDrawing event.  When no one is subscribing, that means
				// the image is not visible

				if (_imageDrawingEvent == null)
					return false;

				return _imageDrawingEvent.GetInvocationList().Length > 0;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="PresentationImage"/> is selected.
		/// </summary>
		public bool Selected
		{
			get { return _selected; }
			set 
			{
				if (_selected != value)
				{
					_selected = value;

					if (_selected)
					{
						this.ParentViewer.EventBroker.OnPresentationImageSelected(
							new PresentationImageSelectedEventArgs(this));
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="PresentationImage"/> is linked.
		/// </summary>
		public bool Linked
		{
			get { return _linked; }
			set
			{
				if (_linked != value)
				{
					_linked = value;
					EventsHelper.Fire(_linkageChangedEvent, this, new LinkageChangedEventArgs(value));
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="PresentationImage"/> is about to be drawn.
		/// </summary>
		/// <remarks>The event handler receives an argument of type <see cref="ImageDrawingEventArgs"/>.</remarks>
		public event EventHandler<ImageDrawingEventArgs> ImageDrawing
		{
			add { _imageDrawingEvent += value; }
			remove { _imageDrawingEvent -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="IsLinked"/> property in this <see cref="PresentationImage"/>
		/// has changed.
		/// </summary>
		/// <remarks>The event handler receives an argument of type <see cref="LinkageChangedEventArgs"/>.</remarks>
		public event EventHandler<LinkageChangedEventArgs> LinkageChanged
		{
			add { _linkageChangedEvent += value; }
			remove { _linkageChangedEvent -= value; }
		}

		#region IDrawable Members

		public virtual void Draw(bool paintNow)
		{
			if (!this.Visible)
				return;

			this.LayerManager.RootLayerGroup.RedrawRequired = true;
			DrawLayers(paintNow);
		}

		#endregion

		internal void DrawLayers(bool paintNow)
		{
			if (!this.Visible)
				return;

			ImageDrawingEventArgs args = new ImageDrawingEventArgs(this, paintNow);
			this.ParentViewer.EventBroker.OnImageDrawing(args);
			OnImageDrawing(this, args);
		}

		#region IUIEventHandler Members

		public bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			e.SelectedPresentationImage = this;
			e.SelectedDisplaySet = this.ParentDisplaySet;

			bool handled = this.LayerManager.RootLayerGroup.OnMouseDown(e);

            if (!handled)
            {
				MouseTool tool = this.ParentViewer.MouseToolMap[e.Button];
                if (tool != null)
                {
                    tool.OnMouseDown(e);
                }
            }

			return true;
		}

		public bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			e.SelectedPresentationImage = this;
			e.SelectedDisplaySet = this.ParentDisplaySet;

			bool handled = this.LayerManager.RootLayerGroup.OnMouseMove(e);

            if (!handled)
            {
				MouseTool tool = this.ParentViewer.MouseToolMap[e.Button];
                if (tool != null)
                {
                    tool.OnMouseMove(e);
                }
            }

			return true;
		}

		public bool OnMouseUp(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			e.SelectedPresentationImage = this;
			e.SelectedDisplaySet = this.ParentDisplaySet;
			
			bool handled = this.LayerManager.RootLayerGroup.OnMouseUp(e);

            if (!handled)
            {
				MouseTool tool = this.ParentViewer.MouseToolMap[e.Button];
                if (tool != null)
                {
                    tool.OnMouseUp(e);
                }
            }

			return true;
		}

		public bool OnMouseWheel(XMouseEventArgs e)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool OnKeyDown(XKeyEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return true;
		}

		public bool OnKeyUp(XKeyEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return true;
		}

		#endregion

		protected virtual void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			ImageLayer selectedImageLayer = this.LayerManager.SelectedImageLayer;

			if (selectedImageLayer != null)
			{
				if (selectedImageLayer.IsGrayscale && selectedImageLayer.RedrawRequired)
					this.LayerManager.SelectedImageLayer.GrayscaleLUTPipeline.Execute();
			}

			EventsHelper.Fire(_imageDrawingEvent, this, e);
		}
	}
}
