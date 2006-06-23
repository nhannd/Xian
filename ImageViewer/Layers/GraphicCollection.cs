using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layers
{
	/// <summary>
	/// 
	/// </summary>
	public class GraphicCollection : BaseLayerCollection
	{
		public GraphicCollection()
		{

		}

		public void Insert(int index, Graphic graphic)
		{
			Platform.CheckArgumentRange(index, 0, this.Count - 1, "index");
			Platform.CheckForNullReference(graphic, "graphic");
			base.Insert(index, graphic);
		}

		public void Add(Graphic graphic)
		{
			Platform.CheckForNullReference(graphic, "graphic");
			base.Add(graphic);
		}
	}
}
