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
using System.Drawing;
using System.Threading;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	internal static class InteractiveShutterGraphicBuilders
	{
		private static readonly Color _normalColor = Color.LightSteelBlue;
		private static readonly Color _invalidColor = Color.Red;
		private static readonly int _flashDelay = 100;

		public static InteractiveCircleGraphicBuilder CreateCircularShutterBuilder(IBoundableGraphic graphic)
		{
			return new InteractiveCircularShutterBuilder(graphic);
		}

		public static InteractiveBoundableGraphicBuilder CreateRectangularShutterBuilder(IBoundableGraphic graphic)
		{
			return new InteractiveRectangularShutterBuilder(graphic);
		}

		public static InteractivePolygonGraphicBuilder CreatePolygonalShutterBuilder(IPointsGraphic graphic)
		{
			return new InteractivePolygonalShutterBuilder(graphic);
		}

		private static bool IsShutterTooSmall(IGraphic shutterGraphic)
		{
			shutterGraphic.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				RectangleF boundingBox = shutterGraphic.BoundingBox;
				return (boundingBox.Width < 50 || boundingBox.Height < 50);
			}
			finally
			{
				shutterGraphic.ResetCoordinateSystem();
			}
		}

		private delegate void DoFlashDelegate(IVectorGraphic graphic, SynchronizationContext context);

		private static void DoFlash(IVectorGraphic graphic, SynchronizationContext context)
		{
			context.Send(delegate
			             	{
			             		if (graphic.ImageViewer != null && graphic.ImageViewer.DesktopWindow != null)
			             		{
			             			graphic.Color = _invalidColor;
			             			graphic.Draw();
			             		}
			             	}, null);
			Thread.Sleep(_flashDelay);
			context.Send(delegate
			             	{
			             		if (graphic.ImageViewer != null && graphic.ImageViewer.DesktopWindow != null)
			             		{
			             			graphic.Color = _normalColor;
			             			graphic.Draw();
			             		}
			             	}, null);
		}

		private static void Flash(IGraphic graphic)
		{
			if (graphic is IVectorGraphic)
			{
				DoFlashDelegate doFlashDelegate = DoFlash;
				doFlashDelegate.BeginInvoke((IVectorGraphic) graphic, SynchronizationContext.Current, delegate(IAsyncResult result) { doFlashDelegate.EndInvoke(result); }, null);
			}
		}

		private class InteractiveCircularShutterBuilder : InteractiveCircleGraphicBuilder
		{
			public InteractiveCircularShutterBuilder(IBoundableGraphic graphic)
				: base(graphic)
			{
				graphic.Color = _normalColor;
			}

			protected override void OnGraphicComplete()
			{
				if (IsShutterTooSmall(base.Graphic))
				{
					Flash(base.Graphic);
					base.Rollback();
					return;
				}
				base.OnGraphicComplete();
			}
		}

		private class InteractiveRectangularShutterBuilder : InteractiveBoundableGraphicBuilder
		{
			public InteractiveRectangularShutterBuilder(IBoundableGraphic graphic)
				: base(graphic)
			{
				graphic.Color = _normalColor;
			}

			protected override void OnGraphicComplete()
			{
				if (IsShutterTooSmall(base.Graphic))
				{
					Flash(base.Graphic);
					base.Rollback();
					return;
				}
				base.OnGraphicComplete();
			}
		}

		private class InteractivePolygonalShutterBuilder : InteractivePolygonGraphicBuilder
		{
			public InteractivePolygonalShutterBuilder(IPointsGraphic graphic)
				: base(graphic)
			{
				graphic.Color = _normalColor;
			}

			protected override void OnGraphicComplete()
			{
				if (IsShutterTooSmall(base.Graphic))
				{
					Flash(base.Graphic);
					base.Rollback();
					return;
				}
				base.OnGraphicComplete();
			}
		}
	}
}