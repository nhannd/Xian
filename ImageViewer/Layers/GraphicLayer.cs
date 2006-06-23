using System;
using System.Drawing;
using System.Collections;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Layers
{
	/// <summary>
	/// A container for <see cref="Graphic"/> objects.
	/// </summary>
	public class GraphicLayer : Layer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GraphicLayer"/> class.
		/// </summary>
		public GraphicLayer()
		{
			this.Graphics.ItemAdded += new EventHandler<LayerEventArgs>(OnGraphicAdded);
		}

		/// <summary>
		/// Gets a collection of <see cref="Graphic"/> objects.
		/// </summary>
		public GraphicCollection Graphics
		{
			get
			{
				return base.BaseLayers as GraphicCollection;
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
							base.ParentLayerManager.SelectedGraphicLayer = this;
					}
				}
			}
		}

		protected override BaseLayerCollection CreateChildLayers()
		{
			return new GraphicCollection();
		}

		private void OnGraphicAdded(object sender, LayerEventArgs e)
		{
			e.Layer.ParentLayer = this;
			e.Layer.ParentLayerManager = base.ParentLayerManager;
			e.Layer.SpatialTransform = base.SpatialTransform;
			e.Layer.CoordinateSystem = base.CoordinateSystem;
		}
	}
}
