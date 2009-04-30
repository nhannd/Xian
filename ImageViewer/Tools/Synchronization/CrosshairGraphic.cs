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

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	internal class CrosshairGraphic : CompositeGraphic
	{
		private static readonly int _lineLength = 10;

		private readonly LinePrimitive _line1;
		private readonly LinePrimitive _line2;
		private readonly LinePrimitive _line3;
		private readonly LinePrimitive _line4;

		public PointF _anchor;

		public CrosshairGraphic()
		{
			base.Graphics.Add(_line1 = new LinePrimitive());
			base.Graphics.Add(_line2 = new LinePrimitive());
			base.Graphics.Add(_line3 = new LinePrimitive());
			base.Graphics.Add(_line4 = new LinePrimitive());

			this.Color = Color.LimeGreen;
		}

		
		public Color Color
		{
			get { return _line1.Color; }
			set
			{
				_line1.Color = _line2.Color = _line3.Color = _line4.Color = value;
			}
		}

		public PointF Anchor
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return _anchor;
				}
				else
				{
					return base.SpatialTransform.ConvertToDestination(_anchor);
				}
			}
			set
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					_anchor = value;
				}
				else
				{
					_anchor = base.SpatialTransform.ConvertToSource(value);
				}
			}
		}

		public override void OnDrawing()
		{
			SetCrossHairLines();

			base.OnDrawing();
		}

		private void SetCrossHairLines()
		{
			base.CoordinateSystem = CoordinateSystem.Destination;

			SizeF offset1 = new SizeF(_lineLength + 5F, 0);
			SizeF offset2 = new SizeF(5F, 0);

			PointF anchor = Anchor;

			_line1.Pt1 = PointF.Subtract(anchor, offset1);
			_line1.Pt2 = PointF.Subtract(anchor, offset2);

			_line2.Pt1 = PointF.Add(anchor, offset1);
			_line2.Pt2 = PointF.Add(anchor, offset2);

			offset1 = new SizeF(0, _lineLength + 5F);
			offset2 = new SizeF(0, 5F);

			_line3.Pt1 = PointF.Subtract(anchor, offset1);
			_line3.Pt2 = PointF.Subtract(anchor, offset2);

			_line4.Pt1 = PointF.Add(anchor, offset1);
			_line4.Pt2 = PointF.Add(anchor, offset2);

			base.ResetCoordinateSystem();
		}
	}
}
