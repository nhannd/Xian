using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using System.Drawing.Drawing2D;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class SceneGraph : CompositeGraphic
	{
		private event EventHandler<RectangleChangedEventArgs> _clientRectangleChangedEvent;

		public SceneGraph()
		{
		}

		/// <summary>
		/// Occurs when <see cref="DestinationRectangle"/> has changed.
		/// </summary>
		public event EventHandler<RectangleChangedEventArgs> ClientRectangleChanged
		{
			add { _clientRectangleChangedEvent += value; }
			remove { _clientRectangleChangedEvent -= value; }
		}


		/// <summary>
		/// Gets or sets the destination rectangle.
		/// </summary>
		internal Rectangle ClientRectangle
		{
			get { return base.SpatialTransform.ClientRectangle; }
			set
			{
				SetClientRectangle(this, value);

				EventsHelper.Fire(_clientRectangleChangedEvent, this, new RectangleChangedEventArgs(value));
			}
		}

		private void SetClientRectangle(CompositeGraphic compositeGraphic, Rectangle clientRectangle)
		{
			compositeGraphic.SpatialTransform.ClientRectangle = clientRectangle;

			foreach (Graphic graphic in compositeGraphic.Graphics)
			{
				CompositeGraphic childGraphic = graphic as CompositeGraphic;

				if (childGraphic != null)
					SetClientRectangle(childGraphic, clientRectangle);
				else
					graphic.SpatialTransform.ClientRectangle = clientRectangle;
			}
		}

	}
}
