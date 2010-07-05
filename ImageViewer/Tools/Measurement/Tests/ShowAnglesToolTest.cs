#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Tools.Measurement.Tests
{
	[TestFixture]
	public class ShowAnglesToolTest
	{
		[Test]
		public void TestCloningShowAnglesToolCompositeGraphicLineSelection()
		{
			MockShowAnglesTool mockOwnerTool = new MockShowAnglesTool();
			using (MockPresentationImage image = new MockPresentationImage(100, 100))
			{
				ShowAnglesTool.ShowAnglesToolCompositeGraphic composite = new ShowAnglesTool.ShowAnglesToolCompositeGraphic(mockOwnerTool);
				image.OverlayGraphics.Add(composite);

				using (IPresentationImage clone = image.Clone())
				{
					var cloneComposite = FindShowAnglesToolComposite(clone);
					Assert.IsNotNull(cloneComposite, "ShowAnglesToolCompositeGraphic should be cloneable.");
				}

				PolylineGraphic line1 = new PolylineGraphic();
				line1.Points.Add(new PointF(0, 0));
				line1.Points.Add(new PointF(10, 10));
				VerticesControlGraphic control1 = new VerticesControlGraphic(line1);
				image.OverlayGraphics.Add(control1);
				composite.Select(control1);
				composite.OnDrawing();

				using (IPresentationImage clone = image.Clone())
				{
					var cloneComposite = FindShowAnglesToolComposite(clone);

					cloneComposite.CoordinateSystem = line1.CoordinateSystem;
					try
					{
						Assert.IsNotNull(cloneComposite.SelectedLine, "Cloned ShowAnglesToolCompositeGraphic should retain line graphic selection.");
						Assert.AreEqual(line1.Points[0], cloneComposite.SelectedLine.Points[0], "Cloned ShowAnglesToolCompositeGraphic should retain line graphic selection (X).");
						Assert.AreEqual(line1.Points[1], cloneComposite.SelectedLine.Points[1], "Cloned ShowAnglesToolCompositeGraphic should retain line graphic selection (Y).");
					}
					finally
					{
						cloneComposite.ResetCoordinateSystem();
					}
				}

				PolylineGraphic line2 = new PolylineGraphic();
				line2.Points.Add(new PointF(0, 10));
				line2.Points.Add(new PointF(10, 0));
				VerticesControlGraphic control2 = new VerticesControlGraphic(line2);
				image.OverlayGraphics.Add(control2);
				composite.Select(control2);
				composite.OnDrawing();

				using (IPresentationImage clone = image.Clone())
				{
					var cloneComposite = FindShowAnglesToolComposite(clone);

					cloneComposite.CoordinateSystem = line2.CoordinateSystem;
					try
					{
						Assert.IsNotNull(cloneComposite.SelectedLine, "Cloned ShowAnglesToolCompositeGraphic should retain line graphic selection (2).");
						Assert.AreEqual(line2.Points[0], cloneComposite.SelectedLine.Points[0], "Cloned ShowAnglesToolCompositeGraphic should retain line graphic selection (X2).");
						Assert.AreEqual(line2.Points[1], cloneComposite.SelectedLine.Points[1], "Cloned ShowAnglesToolCompositeGraphic should retain line graphic selection (Y2).");
					}
					finally
					{
						cloneComposite.ResetCoordinateSystem();
					}
				}
			}
		}

		[Test]
		public void TestCloningShowAnglesToolCompositeGraphicVisibility()
		{
			MockShowAnglesTool mockOwnerTool = new MockShowAnglesTool();
			using (MockPresentationImage image = new MockPresentationImage(100, 100))
			{
				ShowAnglesTool.ShowAnglesToolCompositeGraphic composite = new ShowAnglesTool.ShowAnglesToolCompositeGraphic(mockOwnerTool);
				image.OverlayGraphics.Add(composite);

				PolylineGraphic line1 = new PolylineGraphic();
				line1.Points.Add(new PointF(0, 0));
				line1.Points.Add(new PointF(10, 10));
				VerticesControlGraphic control1 = new VerticesControlGraphic(line1);
				image.OverlayGraphics.Add(control1);
				composite.Select(control1);
				composite.OnDrawing();

				PolylineGraphic line2 = new PolylineGraphic();
				line2.Points.Add(new PointF(0, 10));
				line2.Points.Add(new PointF(10, 0));
				VerticesControlGraphic control2 = new VerticesControlGraphic(line2);
				image.OverlayGraphics.Add(control2);
				composite.Select(control2);
				composite.OnDrawing();

				mockOwnerTool.ShowAngles = true;
				composite.OnDrawing();

				using (IPresentationImage clone = image.Clone())
				{
					var cloneComposite = FindShowAnglesToolComposite(clone);
					Assert.IsTrue(cloneComposite.Visible, "Cloned ShowAnglesToolCompositeGraphic should retain visibility state when captured (true).");
					mockOwnerTool.ShowAngles = false;
					composite.OnDrawing();
					Assert.IsTrue(cloneComposite.Visible, "Cloned ShowAnglesToolCompositeGraphic should retain visibility state when captured (true) even when the original changes.");
				}

				mockOwnerTool.ShowAngles = false;
				composite.OnDrawing();

				using (IPresentationImage clone = image.Clone())
				{
					var cloneComposite = FindShowAnglesToolComposite(clone);
					Assert.IsFalse(cloneComposite.Visible, "Cloned ShowAnglesToolCompositeGraphic should retain visibility state when captured (false).");
					mockOwnerTool.ShowAngles = true;
					composite.OnDrawing();
					Assert.IsFalse(cloneComposite.Visible, "Cloned ShowAnglesToolCompositeGraphic should retain visibility state when captured (false) even when the original changes.");
				}
			}
		}

		[Test]
		public void TestBug6614()
		{
			MockShowAnglesTool mockOwnerTool = new MockShowAnglesTool();
			using (MockPresentationImage image = new MockPresentationImage(1000, 1000))
			{
				ShowAnglesTool.ShowAnglesToolCompositeGraphic composite = new ShowAnglesTool.ShowAnglesToolCompositeGraphic(mockOwnerTool);
				image.OverlayGraphics.Add(composite);

				PolylineGraphic line1 = new PolylineGraphic();
				line1.Points.Add(new PointF(274.983246f, 483.976f));
				line1.Points.Add(new PointF(674.3086f, 490.196f));
				VerticesControlGraphic control1 = new VerticesControlGraphic(line1);
				image.OverlayGraphics.Add(control1);
				composite.Select(control1);
				composite.OnDrawing();

				using (IPresentationImage clone = image.Clone())
				{
					var cloneComposite = FindShowAnglesToolComposite(clone);
					var cloneAngles = FindShowAnglesTool(cloneComposite.Graphics);

					if (cloneAngles != null)
					{
						foreach (ICalloutGraphic calloutGraphic in CollectionUtils.Select(cloneAngles.Graphics, IsOfType<ICalloutGraphic>))
						{
							if (calloutGraphic.Visible)
							{
								Assert.AreNotEqual(string.Format(SR.ToolsMeasurementFormatDegrees, 0), calloutGraphic.Text, "ShowAnglesToolGraphic should not have spurious 0.0 degree callout with only one line.");
							}
						}
					}
				}
			}
		}

		private static ShowAnglesTool.ShowAnglesToolCompositeGraphic FindShowAnglesToolComposite(IPresentationImage image)
		{
			IOverlayGraphicsProvider imageOverlayGraphics = (IOverlayGraphicsProvider) image;
			IGraphic graphic = CollectionUtils.SelectFirst(imageOverlayGraphics.OverlayGraphics, IsOfType<ShowAnglesTool.ShowAnglesToolCompositeGraphic>);
			if (graphic is CompositeGraphic)
				((CompositeGraphic) graphic).OnDrawing();
			return (ShowAnglesTool.ShowAnglesToolCompositeGraphic) graphic;
		}

		private static ShowAnglesTool.ShowAnglesToolGraphic FindShowAnglesTool(IEnumerable<IGraphic> graphicCollection)
		{
			return (ShowAnglesTool.ShowAnglesToolGraphic) CollectionUtils.SelectFirst(graphicCollection, IsOfType<ShowAnglesTool.ShowAnglesToolGraphic>);
		}

		private static bool IsOfType<T>(object @object)
		{
			return @object is T;
		}

		[Cloneable]
		private class MockPresentationImage : GrayscalePresentationImage
		{
			private static readonly FieldInfo _clientRectangleField = typeof (PresentationImage).GetField("_clientRectangle", BindingFlags.NonPublic | BindingFlags.Instance);

			public MockPresentationImage(int rows, int columns) : base(rows, columns)
			{
				_clientRectangleField.SetValue(this, new Rectangle(0, 0, columns, rows));
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected MockPresentationImage(MockPresentationImage source, ICloningContext context) : base(source, context)
			{
				context.CloneFields(source, this);

				_clientRectangleField.SetValue(this, source.ClientRectangle);
			}
		}

		private class MockShowAnglesTool : ShowAnglesTool
		{
			public MockShowAnglesTool()
			{
				this.ShowAngles = true;
			}

			protected override void OnShowAnglesChanged()
			{
				// prevent actual drawing to the non-existent physical workspace
			}
		}
	}
}

#endif