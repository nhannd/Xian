using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layers
{
	/// <summary>
	/// Summary description for CustomLayer.
	/// </summary>
	public class CustomLayer : Layer
	{
		private event EventHandler<DrawCustomLayerEventArgs> _DrawCustomLayerEvent;
		private bool _DoubleBuffer;

		public CustomLayer()
		{
		}

		public event EventHandler<DrawCustomLayerEventArgs> DrawCustomLayerEvent
		{
			add
			{
				_DrawCustomLayerEvent += value;
			}
			remove
			{
				_DrawCustomLayerEvent -= value;
			}
		}

		public void Draw(DrawCustomLayerEventArgs args)
		{
			Platform.CheckForNullReference(args, "args");
			EventsHelper.Fire(_DrawCustomLayerEvent, this, args);
		}

		public bool DoubleBuffer
		{
			get { return _DoubleBuffer; }
			set {_DoubleBuffer = value; }
		}

		protected override BaseLayerCollection  CreateChildLayers()
		{
			return null;
		}
	}
}
