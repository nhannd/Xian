using System;
using System.Drawing;
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Layers
{
	/// <summary>
	/// A group of layers that are subject to a particular <see cref="SpatialTransform"/>.
	/// </summary>
	public sealed class LayerGroup : Layer
	{
		private event EventHandler<RectangleChangedEventArgs> m_DestinationRectangleChangedEvent;

		/// <summary>
		/// Initializes a new instance of the <see cref="LayerGroup"/> class.
		/// </summary>
		public LayerGroup()
		{
			base.SpatialTransform = new SpatialTransform();
			this.Layers.ItemAdded += new EventHandler<LayerEventArgs>(OnLayerAdded);
			this.RedrawRequired = true;
		}

		/// <summary>
		/// Gets a collection of this <see cref="LayerGroup"/>'s child layers.
		/// </summary>
		public LayerCollection Layers
		{
			get { return base.BaseLayers as LayerCollection; }
		}


		/// <summary>
		/// Occurs when <see cref="DestinationRectangle"/> has changed.
		/// </summary>
		public event EventHandler<RectangleChangedEventArgs> DestinationRectangleChanged
		{
			add { m_DestinationRectangleChangedEvent += value; }
			remove { m_DestinationRectangleChangedEvent -= value; }
		}

		/// <summary>
		/// Gets or sets the destination rectangle.
		/// </summary>
		public Rectangle DestinationRectangle
		{
			get
			{
				return base.SpatialTransform.DestinationRectangle;
			}
			set
			{
				base.SpatialTransform.DestinationRectangle = value;
				base.SpatialTransform.Calculate();

				if (this.Layers == null)
					return;

				foreach (Layer layer in this.Layers)
				{
					LayerGroup layerGroup = layer as LayerGroup;

					if (layerGroup != null)
						layerGroup.DestinationRectangle = value;
				}

				EventsHelper.Fire(m_DestinationRectangleChangedEvent, this, new RectangleChangedEventArgs(value));
			}
		}

		public override bool Selected
		{
			set
			{
				Platform.CheckMemberIsSet(base.ParentLayerManager, "Layer.ParentLayerManager");

				if (base.Selected != value)
				{
					base.Selected = value;

					if (base.Selected)
					{
						if (base.ParentLayerManager != null)
							base.ParentLayerManager.SelectedLayerGroup = this;
					}
				}
			}
		}

		protected override BaseLayerCollection CreateChildLayers()
		{
			return new LayerCollection();
		}

		private void OnLayerAdded(object sender, LayerEventArgs e)
		{
			e.Layer.ParentLayer = this;
			e.Layer.ParentLayerManager = base.ParentLayerManager;

			// If we're adding a new LayerGroup, retain its
			// own SpatialTransform (that's the whole point
			// of a LayerGroup--it should have its own transform)
			if (e.Layer is LayerGroup)
				e.Layer.SpatialTransform.DestinationRectangle = base.SpatialTransform.DestinationRectangle;
			else
				e.Layer.SpatialTransform = base.SpatialTransform;

		}
	}
}
