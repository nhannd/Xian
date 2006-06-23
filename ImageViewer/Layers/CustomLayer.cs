using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Layers
{
	/// <summary>
	/// Summary description for CustomLayer.
	/// </summary>
	public class CustomLayer : Layer
	{
		private event EventHandler<DrawCustomLayerEventArgs> m_DrawCustomLayerEvent;
		private bool m_DoubleBuffer;

		public CustomLayer()
		{
		}

		public event EventHandler<DrawCustomLayerEventArgs> DrawCustomLayerEvent
		{
			add
			{
				m_DrawCustomLayerEvent += value;
			}
			remove
			{
				m_DrawCustomLayerEvent -= value;
			}
		}

		public void Draw(DrawCustomLayerEventArgs args)
		{
			Platform.CheckForNullReference(args, "args");
			EventsHelper.Fire(m_DrawCustomLayerEvent, this, args);
		}

		public bool DoubleBuffer
		{
			get { return m_DoubleBuffer; }
			set { m_DoubleBuffer = value; }
		}

		protected override BaseLayerCollection  CreateChildLayers()
		{
			return null;
		}
	}
}
