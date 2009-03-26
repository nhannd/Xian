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
using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An arc <see cref="InvariantPrimitive"/>.
	/// </summary>
	[Cloneable(true)]
	public class InvariantArcPrimitive : InvariantBoundablePrimitive, IArcGraphic
	{
		private float _startAngle;
		private float _sweepAngle;

		/// <summary>
		/// Initializes a new instance of <see cref="InvariantArcPrimitive"/>.
		/// </summary>
		public InvariantArcPrimitive()
		{

		}

		/// <summary>
		/// Gets or sets the angle in degrees at which the arc begins.
		/// </summary>
		/// <remarks>
		/// It is good practice to set the <see cref="IArcGraphic.StartAngle"/> before the <see cref="IArcGraphic.SweepAngle"/>
		/// because in the case where a graphic is scaled differently in x than in y, the conversion
		/// of the <see cref="IArcGraphic.SweepAngle"/> from <see cref="CoordinateSystem.Source"/> to
		/// <see cref="CoordinateSystem.Destination"/> coordinates is dependent upon the <see cref="IArcGraphic.StartAngle"/>.
		/// However, under normal circumstances, where the scale in x and y are the same, the <see cref="IArcGraphic.StartAngle"/>
		/// and <see cref="IArcGraphic.SweepAngle"/> can be set independently.
		/// </remarks>
		public float StartAngle
		{
			get
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					return _startAngle;
				}
				else
				{
					return ArcPrimitive.ConvertStartAngle(_startAngle, this.SpatialTransform, CoordinateSystem.Destination);
				}
			}
			set
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					_startAngle = value;
				}
				else
				{
					_startAngle = ArcPrimitive.ConvertStartAngle(value, this.SpatialTransform, CoordinateSystem.Source);
				}
			}
		}

		/// <summary>
		/// Gets or sets the angle in degrees that the arc sweeps out.
		/// </summary>
		/// <remarks>
		/// See <see cref="IArcGraphic.StartAngle"/> for information on setting the <see cref="IArcGraphic.SweepAngle"/>.
		/// </remarks>
		public float SweepAngle
		{
			get
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					return _sweepAngle;
				}
				else
				{
					return ArcPrimitive.ConvertSweepAngle(_sweepAngle, _startAngle, this.SpatialTransform, CoordinateSystem.Destination);
				}
			}
			set
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					_sweepAngle = value;
				}
				else
				{
					this.CoordinateSystem = CoordinateSystem.Destination;
					_sweepAngle = ArcPrimitive.ConvertSweepAngle(value, this.StartAngle, this.SpatialTransform, CoordinateSystem.Source);
					this.ResetCoordinateSystem();
				}
			}
		}

		/// <summary>
		/// Performs a hit test on the <see cref="InvariantArcPrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="InvariantArcPrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the arc.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Source;

			bool result = ArcPrimitive.HitTest(
				SpatialTransform.ConvertToSource(point), this.Rectangle,
				this.StartAngle, this.SweepAngle,
				this.SpatialTransform);

			this.ResetCoordinateSystem();

			return result;
		}

		public override PointF GetClosestPoint(PointF point)
		{
			return ArcPrimitive.GetClosestPoint(point, this.Rectangle, this.StartAngle, this.SweepAngle);
		}

		public override bool Contains(Point point)
		{
			return false;
		}
	}
}
