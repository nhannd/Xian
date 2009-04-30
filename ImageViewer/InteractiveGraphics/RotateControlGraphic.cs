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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public sealed class RotateControlGraphic : ControlPointsGraphic
	{
		private PointF _centre;
		private PointF _reference;

		[CloneIgnore]
		private bool _bypassControlPointChangedEvent = false;

		public RotateControlGraphic(IGraphic subject)
			: base(subject)
		{
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF bounds = subject.BoundingBox;
				_centre = new PointF(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
				_reference = _centre + new SizeF(0, bounds.Height/5);
				base.ControlPoints.Add(_reference);
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			Initialize();
		}

		private RotateControlGraphic(RotateControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		private void Initialize()
		{
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override PointF ConstrainControlPointLocation(int controlPointIndex, PointF cursorLocation) {
			return base.ConstrainControlPointLocation(controlPointIndex, cursorLocation);
		}

		//protected override void OnMoved() {
		//    base.OnMoved();

		//    this.CoordinateSystem = CoordinateSystem.Source;
		//    try {
		//        RectangleF bounds = this.Subject.BoundingBox;
		//        PointF newCentre = new PointF(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
		//        SizeF offset = new SizeF(newCentre.X - _centre.X, newCentre.Y - _centre.Y);
		//        _centre += offset;
		//        _reference += offset;
		//        base.ControlPoints[0] += offset;
		//    } finally {
		//        this.ResetCoordinateSystem();
		//    }
		//}

		protected override void OnControlPointChanged(int index, PointF point)
		{
			if (!_bypassControlPointChangedEvent)
			{
				PointF centre = _centre;
				PointF reference = _reference;

				if(this.CoordinateSystem ==  CoordinateSystem.Destination)
				{
					_centre = this.SpatialTransform.ConvertToSource(_centre);
					_reference = this.SpatialTransform.ConvertToSource(_reference);
				}

				this.SpatialTransform.RotationXY = (int) Vector.SubtendedAngle(reference, centre, point);
			}
			base.OnControlPointChanged(index, point);
		}
	}
}