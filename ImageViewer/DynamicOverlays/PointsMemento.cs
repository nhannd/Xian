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
		List<PointF> _AnchorPoints = new List<PointF>();

		public PointsMemento()
		{
		}

		public void Add(PointF point)
		{
			_AnchorPoints.Add(point);
		}

		#region IEnumerable<PointF> Members

		public System.Collections.Generic.IEnumerator<PointF> GetEnumerator()
		{
			return _AnchorPoints.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _AnchorPoints.GetEnumerator();
		}

		#endregion
	}
}
