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

using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	partial class DefineSlicePlaneTool
	{
		private class SliceLineGraphic : CompositeGraphic
		{
			private SliceLineGraphic(IGraphic graphic)
			{
				base.Graphics.Add(graphic);
			}

			public IPointsGraphic LineGraphic
			{
				get { return ((IControlGraphic) base.Graphics[0]).Subject as IPointsGraphic; }
			}

			private SliceLineControlGraphic GetSliceLineControlGraphic(IControlGraphic graphic)
			{
				if (graphic is SliceLineControlGraphic)
					return (SliceLineControlGraphic) graphic;

				if (graphic.DecoratedGraphic is IControlGraphic)
					return GetSliceLineControlGraphic((IControlGraphic) graphic.DecoratedGraphic);

				return null;
			}

			public string Text
			{
				get { return GetSliceLineControlGraphic((IControlGraphic) base.Graphics[0]).Text; }
				set { GetSliceLineControlGraphic((IControlGraphic) base.Graphics[0]).Text = value; }
			}

			public void SetLine(IPresentationImage referenceImage, IPresentationImage targetImage)
			{
				DicomImagePlane thisImagePlane = DicomImagePlane.FromImage(referenceImage);
				DicomImagePlane imagePlane = DicomImagePlane.FromImage(targetImage);
				Vector3D patientPt1, patientPt2;
				if (thisImagePlane.GetIntersectionPoints(imagePlane, out patientPt1, out patientPt2))
				{
					Vector3D imagePt1 = imagePlane.ConvertToImagePlane(patientPt1);
					Vector3D imagePt2 = imagePlane.ConvertToImagePlane(patientPt2);

					this.LineGraphic.Points.Clear();
					this.LineGraphic.Points.Add(imagePlane.ConvertToImage(new PointF(imagePt1.X, imagePt1.Y)));
					this.LineGraphic.Points.Add(imagePlane.ConvertToImage(new PointF(imagePt2.X, imagePt2.Y)));
				}
			}

			public static SliceLineGraphic CreateSliceLineGraphic(MprViewerComponent mprViewer, IMprSliceSet sliceSet, Color hotColor, Color normalColor)
			{
				PolylineGraphic polylineGraphic = new PolylineGraphic();
				MoveControlGraphic moveControlGraphic = new MoveControlGraphic(polylineGraphic);
				VerticesControlGraphic verticesControlGraphic = new VerticesControlGraphic(moveControlGraphic);
				verticesControlGraphic.CanAddRemoveVertices = false;
				SliceLineControlGraphic sliceLineControlGraphic = new SliceLineControlGraphic(verticesControlGraphic);
				StandardStatefulGraphic statefulGraphic = new StandardStatefulGraphic(sliceLineControlGraphic);
				statefulGraphic.InactiveColor = statefulGraphic.SelectedColor = normalColor;
				statefulGraphic.FocusColor = statefulGraphic.FocusSelectedColor = hotColor;
				statefulGraphic.State = statefulGraphic.CreateInactiveState();
				return new SliceLineGraphic(statefulGraphic);
			}

			private class SliceLineControlGraphic : ControlGraphic
			{
				private ITextGraphic _textGraphic;

				public SliceLineControlGraphic(IGraphic subject) : base(subject)
				{
					base.Graphics.Add(_textGraphic = new InvariantTextPrimitive());
					base.DecoratedGraphic.VisualStateChanged += DecoratedGraphic_VisualStateChanged;
				}

				protected override void Dispose(bool disposing)
				{
					base.DecoratedGraphic.VisualStateChanged -= DecoratedGraphic_VisualStateChanged;
					_textGraphic = null;

					base.Dispose(disposing);
				}

				public string Text
				{
					get { return _textGraphic.Text; }
					set { _textGraphic.Text = value; }
				}

				private void DecoratedGraphic_VisualStateChanged(object sender, VisualStateChangedEventArgs e)
				{
					base.DecoratedGraphic.CoordinateSystem = CoordinateSystem.Destination;
					_textGraphic.CoordinateSystem = CoordinateSystem.Destination;
					try
					{
						RectangleF rectangle = base.DecoratedGraphic.BoundingBox;
						_textGraphic.Location = new PointF(rectangle.Right, rectangle.Top);
					}
					finally
					{
						_textGraphic.ResetCoordinateSystem();
						base.DecoratedGraphic.ResetCoordinateSystem();
					}
				}
			}
		}
	}
}