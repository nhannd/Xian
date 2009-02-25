#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public class ProtractorRoiInfo : RoiInfo
	{
		private List<PointF> _points;

		public ProtractorRoiInfo()
		{
			_points = new List<PointF>();
		}

		/// <summary>
		/// Three points in destination coordinates that define the angle.
		/// </summary>
		public List<PointF> Points
		{
			get { return _points; }
		}

		protected internal override void Initialize(InteractiveGraphic graphic)
		{
			ProtractorInteractiveGraphic protractor = graphic as ProtractorInteractiveGraphic;
			Platform.CheckForInvalidCast(protractor, "protractor", typeof(ProtractorInteractiveGraphic).FullName);

			base.Initialize(graphic);

			_points.Clear();

			graphic.CoordinateSystem = CoordinateSystem.Source;

			PolyLineGraphic line = protractor.PolyLine;
			for (int i = 0; i < line.Count; ++i)
				_points.Add(line[i]);

			graphic.ResetCoordinateSystem();
		}
	}

	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuProtractor", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarProtractor", "Select", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.ProtractorToolSmall.png", "Icons.ProtractorToolMedium.png", "Icons.ProtractorToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Measurement.Angle")]

	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ProtractorTool : MeasurementTool<ProtractorRoiInfo>
	{
		public ProtractorTool()
			: base(SR.TooltipProtractor)
		{
		}

		protected override string CreationCommandName
		{
			get { return SR.CommandCreateProtractor; }
		}

		protected override InteractiveGraphic CreateInteractiveGraphic()
		{
			return new ProtractorInteractiveGraphic();
		}

		protected override GraphicState CreateCreateState(RoiGraphic roiGraphic)
		{
			return new CreatePolyLineGraphicState(roiGraphic);
		}

		protected override IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return new ProtractorRoiCalloutLocationStrategy();
		}
	}
}
