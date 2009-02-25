#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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

using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public class PolygonRoiInfo : RoiInfo
	{
		private PolygonF _polygon;

		public PolygonRoiInfo() {}

		/// <summary>
		/// Gets an object representing the polygon.
		/// </summary>
		public PolygonF Polygon
		{
			get { return _polygon; }
		}

		/// <summary>
		/// Initializes a <see cref="PolygonRoiInfo"/> object from a <see cref="PolygonInteractiveGraphic"/>.
		/// </summary>
		protected internal override void Initialize(InteractiveGraphic graphic)
		{
			PolygonInteractiveGraphic polygon = (PolygonInteractiveGraphic) graphic;

			base.Initialize(graphic);

			graphic.CoordinateSystem = CoordinateSystem.Source;

			if (polygon.VertexCount >= 3)
			{
				List<PointF> vertices = new List<PointF>(polygon.VertexCount);
				for (int n = 0; n < polygon.VertexCount; n++)
				{
					vertices.Add(polygon.PolyLine[n]);
				}
				_polygon = new PolygonF(vertices);
			} 
			else
			{
				_polygon = null;
			}

			graphic.ResetCoordinateSystem();
		}
	}

	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuPolygonalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarPolygonalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.PolygonalRoiToolSmall.png", "Icons.PolygonalRoiToolMedium.png", "Icons.PolygonalRoiToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Measurement.Roi.Polygonal")]
	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class PolygonalRoiTool : MeasurementTool<PolygonRoiInfo>
	{
		public PolygonalRoiTool() : base(SR.TooltipPolygonalRoi) {}

		protected override string CreationCommandName
		{
			get { return SR.CommandCreatePolygonalRoi; }
		}

		protected override InteractiveGraphic CreateInteractiveGraphic()
		{
			return new PolygonInteractiveGraphic();
		}

		protected override GraphicState CreateCreateState(RoiGraphic roiGraphic)
		{
			return new CreatePolygonGraphicState(roiGraphic);
		}

		protected override IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return new PolygonRoiCalloutLocationStrategy();
		}
	}
}