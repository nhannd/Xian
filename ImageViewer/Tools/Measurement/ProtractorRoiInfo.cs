#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public class ProtractorRoiInfo : Roi
	{
		private List<PointF> _points;

		internal ProtractorRoiInfo(ProtractorGraphic protractor) : base(protractor.ParentPresentationImage)
		{
			_points = new List<PointF>();

			protractor.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				for (int i = 0; i < protractor.Points.Count; ++i)
					_points.Add(protractor.Points[i]);
			}
			finally
			{
				protractor.ResetCoordinateSystem();
			}
		}

		internal ProtractorRoiInfo(PointF point1, PointF vertex, PointF point2, IPresentationImage presentationImage)
			: base(presentationImage)
		{
			_points = new List<PointF>();
			_points.Add(point1);
			_points.Add(vertex);
			_points.Add(point2);
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

		public override Roi CopyTo(IPresentationImage presentationImage)
		{
			return new ProtractorRoiInfo(_points[0], _points[1], _points[2], presentationImage);
		}

		public override bool Contains(PointF point)
		{
			return false;
		}
	}
}