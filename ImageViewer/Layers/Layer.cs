using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Layers
{
	/// <summary>
	/// The base layer object.
	/// </summary>
	public abstract class Layer : IUIEventHandler
	{
		private BaseLayerCollection _baseLayers;
		private string _name;
		private double _transparency = 1.0f;
		private bool _visible = true;
		private bool _redrawRequired = true;
		private bool _autoHandleMouse = true;
		private bool _selected = false;
		private bool _focused = false;
		private bool _isLeaf = false;
		private SpatialTransform _spatialTransform;
		private Layer _parentLayer;
		private LayerManager _parentLayerManager;
		private Stack<CoordinateSystem> _coordinateSystemStack = new Stack<CoordinateSystem>();

		protected Layer()
		{
			_baseLayers = CreateChildLayers();
			_coordinateSystemStack.Push(CoordinateSystem.Source);
		}

		protected Layer(bool isLeaf)
		{
			_isLeaf = isLeaf;
		}

		/// <summary>
		/// Gets a collection of child layers.
		/// </summary>
		protected BaseLayerCollection BaseLayers
		{
			get { return _baseLayers; }
		}

		/// <summary>
		/// Gets or sets the name of this layer.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this layer is visible.
		/// </summary>
		/// <remarks>The effect of this property will be seen when 
		/// <see cref="Draw"/> is called. This property is recursive.  That is, 
		/// when set, the new value is applied to all child layers, right 
		/// down to the leaves.
		/// </remarks>
		public bool Visible
		{
			get { return _visible; }
			set	
			{
				_visible = value;

				if (this.IsLeaf)
					return;

				foreach (Layer layer in this.BaseLayers)
					layer.Visible = value;
			}
		}

		/// <summary>
		/// Currently unused.
		/// </summary>
		public double Transparency
		{
			get { return _transparency; }
			set 
			{
				_transparency = value;

				if (this.IsLeaf)
					return;

				foreach (Layer layer in this.BaseLayers)
					layer.Transparency = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this layer needs to be redrawn.
		/// </summary>
		/// <remarks>
		/// Clients should never have to set this property, as it is automatically
		/// set when <see cref="Draw"/> is called.  The effect of this property depends 
		/// entirely on the renderer.  For example, the renderer may choose to set 
		/// this to <b>false</b> after rendering the layer
		/// in an attempt to optimize the rendering during the next redraw.  Or it
		/// may ignore this property entirely.
		/// This property is recursive.  That is, when set, the
		/// new value is applied to all child layers, right down to the leaves.		
		/// </remarks>
		public bool RedrawRequired
		{
			get { return _redrawRequired; }
			set 
			{
				_redrawRequired = value;

				if (this.IsLeaf)
					return;

				foreach (Layer layer in this.BaseLayers)
					layer.RedrawRequired = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this layer should automatically
		/// handle mouse messages.
		/// </summary>
		/// <remarks>
		/// By default this property is <b>true</b>, meaning that as a mouse message
		/// is passed down the chain of layers, each layer has an opportunity to handle it.
		/// However, there are situation in which this is not desirable.  A good example
		/// is when a parent layer wants full control over which of its child layers should
		/// handle the messages.  In such a scenario, a child layer should never handle 
		/// the messages automatically, only when its parent tells it to do so, and thus
		/// this property should be set to <b>false</b>.
		/// This property is recursive.  That is, when set, the
		/// new value is applied to all child layers, right down to the leaves.		
		/// </remarks>
		public bool AutoHandleMouse
		{
			get { return _autoHandleMouse; }
			set 
			{
				_autoHandleMouse = value; 

				if (this.IsLeaf)
					return;

				foreach (Layer layer in this.BaseLayers)
					layer.AutoHandleMouse = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the layer is selected.
		/// </summary>
		public virtual bool Selected
		{
			get { return _selected; }
			set { _selected = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the layer is focused.
		/// </summary>
		public virtual bool Focused
		{
			get { return _focused; }
			set { _focused = value; }
		}

		/// <summary>
		/// Gets a value indicating whether the layer is a leaf layer.
		/// </summary>
		public bool IsLeaf
		{
			get { return _isLeaf; }
		}

		/// <summary>
		/// Gets or sets the coordinate system the layer is using.
		/// </summary>
		/// <remarks>
		/// It is often desirable for a client to be able to work in both original
		/// image (source) coordinates, or in screen (destination) coordinates.
		/// By setting this property, all coordinates will be interpreted in that
		/// coordinate system for this layer and all its child layers as well.
		/// The proper practice is to call <see cref="ResetCoordinateSystem"/> after
		/// having set this property.
		/// This property is recursive.  That is, when set, the
		/// new value is applied to all child layers, right down to the leaves.		
		/// </remarks>
		public CoordinateSystem CoordinateSystem
		{
			get { return _coordinateSystemStack.Peek(); }
			set
			{
				Platform.CheckForNullReference(value, "CoordinateSystem");

				_coordinateSystemStack.Push(value);

				if (this.IsLeaf)
					return;

				foreach (Layer layer in this.BaseLayers)
					layer.CoordinateSystem = value;
			}
		}

		/// <summary>
		/// Gets the <see cref="SpatialTransform"/> associated with this
		/// layer's parent <see cref="LayerGroup"/>
		/// </summary>
		public SpatialTransform SpatialTransform
		{
			get { return _spatialTransform; }
			internal set
			{
				_spatialTransform = value;

				if (this.IsLeaf)
					return;

				foreach (Layer layer in this.BaseLayers)
					layer.SpatialTransform = value;
			}
		}

		/// <summary>
		/// Gets this layer's parent <see cref="Layer"/>.
		/// </summary>
		public Layer ParentLayer
		{
			get { return _parentLayer; }
			internal set { _parentLayer = value; }
		}

		/// <summary>
		/// Gets this layer's parent <see cref="PresentationImage"/>.
		/// </summary>
		/// <value>Can be <b>null</b> if the layer has not been added
		/// to the layer tree  (For example, right after construction.)</value>
		public PresentationImage ParentPresentationImage
		{
			get 
			{
				if (this.ParentLayerManager == null)
					return null;

				return this.ParentLayerManager.ParentPresentationImage; 
			}
		}

		/// <summary>
		/// Gets this layer's parent <see cref="LayerManager"/>.
		/// </summary>
		/// <value>Can be <b>null</b> if the layer has not been added
		/// to the layer tree  (For example, right after construction.)</value>
		public LayerManager ParentLayerManager
		{
			get { return _parentLayerManager; }
			internal set
			{
				_parentLayerManager = value;

				if (this.IsLeaf)
					return;

				foreach (Layer layer in this.BaseLayers)
					layer.ParentLayerManager = value;
			}
		}

		/// <summary>
		/// Gets this layer's parent <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>Can be <b>null</b> if the layer has not been added
		/// to the layer tree  (For example, right after construction.)</value>
		public IImageViewer ImageViewer
		{
			get 
			{
				if (this.ParentPresentationImage == null)
					return null;

				return this.ParentPresentationImage.ImageViewer; 
			}
		}

		#region IUIEventHandler Members

		/// <summary>
		/// Processes a MouseDown message.
		/// </summary>
		/// <param name="e"></param>
		/// <returns><b>true</b> if a child layer has handled the message.
		/// <b>false</b> otherwise.</returns>
		public virtual bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (this.IsLeaf)
				return false;

			foreach (Layer layer in this.BaseLayers)
			{
				if (layer.Visible && layer.AutoHandleMouse)
				{
					bool handled = layer.OnMouseDown(e);

					if (handled)
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Processes a MouseMove message.
		/// </summary>
		/// <param name="e"></param>
		/// <returns><b>true</b> if a child layer has handled the message.
		/// <b>false</b> otherwise.</returns>
		public virtual bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (this.IsLeaf)
				return false;

			foreach (Layer layer in this.BaseLayers)
			{
				if (layer.Visible && layer.AutoHandleMouse)
				{
					bool handled = layer.OnMouseMove(e);

					if (handled)
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Processes a MouseUp message.
		/// </summary>
		/// <param name="e"></param>
		/// <returns><b>true</b> if a child layer has handled the message.
		/// <b>false</b> otherwise.</returns>
		public virtual bool OnMouseUp(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			if (this.IsLeaf)
				return false;

			foreach (Layer layer in this.BaseLayers)
			{
				if (layer.Visible && layer.AutoHandleMouse)
				{
					bool handled = layer.OnMouseUp(e);

					if (handled)
						return true;
				}
			}

			return false;
		}

		public virtual bool OnMouseWheel(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return false;
		}

		public virtual bool OnKeyDown(XKeyEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return false;
		}

		public virtual bool OnKeyUp(XKeyEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			return false;
		}

		#endregion

		/// <summary>
		/// Resets the <see cref="CoordinateSystem"/> to its old value.
		/// </summary>
		public void ResetCoordinateSystem()
		{
			if (_coordinateSystemStack.Count == 1)
				return;

			_coordinateSystemStack.Pop();

			if (this.IsLeaf)
				return;

			foreach (Layer layer in this.BaseLayers)
				layer.ResetCoordinateSystem();
		}

		/// <summary>
		/// Draw the layer.
		/// </summary>
		public virtual void Draw()
		{
			Platform.CheckMemberIsSet(this.ParentPresentationImage, "PresentationImage");
			this.RedrawRequired = true;
			this.ParentPresentationImage.Draw();
		}

		protected abstract BaseLayerCollection CreateChildLayers();
	}
}
