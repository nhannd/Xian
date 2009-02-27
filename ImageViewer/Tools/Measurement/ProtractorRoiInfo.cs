using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public class ProtractorRoiInfo : Roi
	{
		private List<PointF> _points;

		internal ProtractorRoiInfo(ProtractorInteractiveGraphic protractor) : base(protractor.ParentPresentationImage)
		{
			_points = new List<PointF>();

			protractor.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				PolyLineGraphic line = protractor.PolyLine;
				for (int i = 0; i < line.Count; ++i)
					_points.Add(line[i]);
			}
			finally
			{
				protractor.ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Three points in destination coordinates that define the angle.
		/// </summary>
		public List<PointF> Points
		{
			get { return _points; }
		}

		protected override RectangleF ComputeBounds()
		{
			return RectangleF.Empty;
		}

		public override bool Contains(PointF point)
		{
			return false;
		}
	}
}