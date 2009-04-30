#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	/// <summary>
	/// Extension point for views onto <see cref="PathProfileComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PathProfileComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PathProfileComponent class
	/// </summary>
	[AssociateView(typeof(PathProfileComponentViewExtensionPoint))]
	public class PathProfileComponent : RoiAnalysisComponent
	{
		private int[] _pixelIndices;
		private int[] _pixelValues;

		/// <summary>
		/// Constructor
		/// </summary>
		public PathProfileComponent(IImageViewerToolContext imageViewerToolContext)
			: base(imageViewerToolContext)
		{
		}

		public int[] PixelIndices
		{
			get { return _pixelIndices; }
		}

		public int[] PixelValues
		{
			get { return _pixelValues; }
		}

		public override void Start()
		{
			// TODO prepare the component for its live phase
			base.Start();
		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		public bool ComputeProfile()
		{
			PolyLineInteractiveGraphic polyLine = GetSelectedPolyLine();

			// For now, make sure the ROI is a polyline
			if (polyLine == null || polyLine.PolyLine.Count != 2)
			{
				this.Enabled = false;
				return false;
			}

			IImageGraphicProvider imageGraphicProvider =
				polyLine.ParentPresentationImage as IImageGraphicProvider;

			if (imageGraphicProvider == null)
			{
				this.Enabled = false;
				return false;
			}

			// For now, only allow ROIs of grayscale images
			GrayscaleImageGraphic image = imageGraphicProvider.ImageGraphic as GrayscaleImageGraphic;

			if (image == null)
			{
				this.Enabled = false;
				return false;
			}

			polyLine.CoordinateSystem = CoordinateSystem.Source;
			Point pt1 = new Point((int)polyLine.PolyLine[0].X, (int)polyLine.PolyLine[0].Y);
			Point pt2 = new Point((int)polyLine.PolyLine[1].X, (int)polyLine.PolyLine[1].Y);

			if (pt1.X < 0 || pt1.X > image.Columns - 1 ||
				pt2.X < 0 || pt2.X > image.Columns - 1 ||
				pt1.Y < 0 || pt1.Y > image.Rows - 1 ||
				pt2.Y < 0 || pt2.Y > image.Rows - 1)
			{
				this.Enabled = false;
				return false;
			}


			List<Point> pixels = BresenhamLine(pt1, pt2);

			_pixelIndices = new int[pixels.Count];
			_pixelValues = new int[pixels.Count];

			int i = 0;

			foreach (Point pixel in pixels)
			{
				int rawPixelValue = image.PixelData.GetPixel(pixel.X, pixel.Y);
				_pixelIndices[i] = i;
				_pixelValues[i] = image.ModalityLut[rawPixelValue];
				i++;
			}

			this.Enabled = true;
			return true;
		}

		protected override bool CanAnalyzeSelectedRoi()
		{
			return GetSelectedPolyLine() == null ? false : true;
		}

		private PolyLineInteractiveGraphic GetSelectedPolyLine()
		{
			RoiGraphic graphic = GetSelectedRoi();

			if (graphic == null)
				return null;

			PolyLineInteractiveGraphic polyLine = graphic.Roi as PolyLineInteractiveGraphic;

			if (polyLine == null)
				return null;

			return polyLine;
		}


		// Swap the values of A and B
		private void Swap<T>(ref T a, ref T b)
		{
			T c = a;
			a = b;
			b = c;
		}

		// Returns the list of points from p0 to p1 
		private List<Point> BresenhamLine(Point p0, Point p1)
		{
			return BresenhamLine(p0.X, p0.Y, p1.X, p1.Y);
		}

		// Returns the list of points from (x0, y0) to (x1, y1)
		private List<Point> BresenhamLine(int x0, int y0, int x1, int y1)
		{
			// Optimization: it would be preferable to calculate in
			// advance the size of "result" and to use a fixed-size array
			// instead of a list.
			List<Point> result = new List<Point>();

			bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
			if (steep)
			{
				Swap(ref x0, ref y0);
				Swap(ref x1, ref y1);
			}
			if (x0 > x1)
			{
				Swap(ref x0, ref x1);
				Swap(ref y0, ref y1);
			}

			int deltax = x1 - x0;
			int deltay = Math.Abs(y1 - y0);
			int error = 0;
			int ystep;
			int y = y0;
			if (y0 < y1) ystep = 1; else ystep = -1;
			for (int x = x0; x <= x1; x++)
			{
				if (steep) result.Add(new Point(y, x));
				else result.Add(new Point(x, y));
				error += deltay;
				if (2 * error >= deltax)
				{
					y += ystep;
					error -= deltax;
				}
			}

			return result;
		}
	}
}
