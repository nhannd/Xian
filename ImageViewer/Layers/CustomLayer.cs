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
		private event EventHandler<DrawCustomLayerEventArgs> _drawCustomLayerEvent;
		private bool _doubleBuffer;

		public CustomLayer()
		{
		}

		public event EventHandler<DrawCustomLayerEventArgs> DrawCustomLayerEvent
		{
			add
			{
				_drawCustomLayerEvent += value;
			}
			remove
			{
				_drawCustomLayerEvent -= value;
			}
		}

		public void Draw(DrawCustomLayerEventArgs args)
		{
			Platform.CheckForNullReference(args, "args");
			EventsHelper.Fire(_drawCustomLayerEvent, this, args);
		}

		public bool DoubleBuffer
		{
			get { return _doubleBuffer; }
			set {_doubleBuffer = value; }
		}

		protected override BaseLayerCollection  CreateChildLayers()
		{
			return null;
		}
	}
}
