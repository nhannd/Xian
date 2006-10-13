using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layers
{
	public sealed class LayerManager : IUIEventHandler
	{
		private PresentationImage _parentPresentationImage;
		private LayerGroup _rootLayerGroup;
		private LayerGroup _selectedLayerGroup;
		private ImageLayer _selectedImageLayer;
		private GraphicLayer _selectedGraphicLayer;
		private Graphic _selectedGraphic;

		private CaptureUIEventHandler _mouseCapture;

		public LayerManager(PresentationImage presentationImage)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");

			_parentPresentationImage = presentationImage;
			_rootLayerGroup = new LayerGroup();
			_rootLayerGroup.ParentLayerManager = this;

			_mouseCapture = new CaptureUIEventHandler();
		}

		public IImageViewer ImageViewer
		{
			get 
			{
				if (this.ParentPresentationImage == null)
					return null;

				return this.ParentPresentationImage.ImageViewer; 
			}
		}

		public PresentationImage ParentPresentationImage
		{
			get { return _parentPresentationImage; }
		}

		public LayerGroup RootLayerGroup
		{
			get { return _rootLayerGroup; }
		}

		public LayerGroup SelectedLayerGroup
		{
			get { return _selectedLayerGroup; }
			internal set
			{
				Platform.CheckForNullReference(value, "SelectedLayerGroup");

				if (_selectedLayerGroup == value)
					return;

				if (_selectedLayerGroup != null)
					_selectedLayerGroup.Selected = false;

				_selectedLayerGroup = value;

				if (this.ImageViewer != null)
					this.ImageViewer.EventBroker.OnLayerGroupSelected(
						new LayerGroupSelectedEventArgs(_selectedLayerGroup));
			}
		}

		public ImageLayer SelectedImageLayer
		{
			get { return _selectedImageLayer; }
			internal set
			{
				Platform.CheckForNullReference(value, "SelectedImageLayer");

				if (_selectedImageLayer == value)
					return;

				if (_selectedImageLayer != null)
					_selectedImageLayer.Selected = false;

				_selectedImageLayer = value;

				if (this.ImageViewer != null)
					this.ImageViewer.EventBroker.OnImageLayerSelected(
						new ImageLayerSelectedEventArgs(_selectedImageLayer));
			}
		}

		public GraphicLayer SelectedGraphicLayer
		{
			get { return _selectedGraphicLayer; }
			internal set
			{
				Platform.CheckForNullReference(value, "SelectedGraphicLayer");

				if (_selectedGraphicLayer == value)
					return;

				if (_selectedGraphicLayer != null)
					_selectedGraphicLayer.Selected = false;

				_selectedGraphicLayer = value;

				if (this.ImageViewer != null)
					this.ImageViewer.EventBroker.OnGraphicLayerSelected(
						new GraphicLayerSelectedEventArgs(_selectedGraphicLayer));
			}
		}

		public Graphic SelectedGraphic
		{
			get { return _selectedGraphic; }
			internal set
			{
				// It is allowable for the selected graphic to be set to null,
				// as that means that no graphic is currently selected.

				if (_selectedGraphic == value)
					return;

				if (_selectedGraphic != null)
					_selectedGraphic.Selected = false;

				_selectedGraphic = value;

				if (this.ImageViewer != null)
					if (_selectedGraphic != null)
						this.ImageViewer.EventBroker.OnGraphicSelected(
							new GraphicSelectedEventArgs(_selectedGraphic));
			}
		}

		#region Public Events
		
		public event EventHandler<MouseCaptureChangingEventArgs> NotifyCaptureChanging
		{
			add { _mouseCapture.NotifyCaptureChanging += value; }
			remove { _mouseCapture.NotifyCaptureChanging -= value; }
		}

		#endregion
		
		#region IUIEventHandler Members

		public bool OnMouseMove(XMouseEventArgs e)
		{
			bool handled = _mouseCapture.OnMouseMove(e);
			if (!handled)
				handled = _rootLayerGroup.OnMouseMove(e);

			return handled;
		}

		public bool OnMouseDown(XMouseEventArgs e)
		{
			bool handled = _mouseCapture.OnMouseDown(e);
			if (!handled)
				handled = _rootLayerGroup.OnMouseDown(e);

			return handled;
		}

		public bool OnMouseUp(XMouseEventArgs e)
		{
			bool handled = _mouseCapture.OnMouseUp(e);
			if (!handled)
				handled = _rootLayerGroup.OnMouseUp(e);

			return handled;
		}

		public bool OnMouseWheel(XMouseEventArgs e)
		{
			bool handled = _mouseCapture.OnMouseWheel(e);
			if (!handled)
				handled = _rootLayerGroup.OnMouseWheel(e);

			return handled;
		}

		public bool OnKeyDown(XKeyEventArgs e)
		{
			bool handled = _mouseCapture.OnKeyDown(e);
			if (!handled)
				handled = _rootLayerGroup.OnKeyDown(e);

			return handled;
		}

		public bool OnKeyUp(XKeyEventArgs e)
		{
			bool handled = _mouseCapture.OnKeyUp(e);
			if (!handled)
				handled = _rootLayerGroup.OnKeyUp(e);

			return handled;
		}

		#endregion
	}
}
