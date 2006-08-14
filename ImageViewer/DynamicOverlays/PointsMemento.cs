using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for AnchorPoints.
	/// </summary>
	public class PointsMemento : IMemento, IEnumerable<PointF>
	{
		List<PointF> _anchorPoints = new List<PointF>();

		public PointsMemento()
		{
		}

		public void Add(PointF point)
		{
			_anchorPoints.Add(point);
		}

		#region IEnumerable<PointF> Members

		public System.Collections.Generic.IEnumerator<PointF> GetEnumerator()
		{
			return _anchorPoints.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _anchorPoints.GetEnumerator();
		}

		#endregion
	}
}
