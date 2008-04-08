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
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// The default strategy for automatically calculating the location of a <see cref="RoiGraphic"/>'s callout.
	/// </summary>
	[Cloneable(true)]
	public class RoiCalloutLocationStrategy : IRoiCalloutLocationStrategy
	{
		[CloneIgnore]
		private RoiGraphic _roiGraphic;
		private bool _initialLocationSet;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal protected RoiCalloutLocationStrategy()
		{
			_initialLocationSet = false;
		}

		/// <summary>
		/// Gets the owning <see cref="RoiGraphic"/>.
		/// </summary>
		protected RoiGraphic RoiGraphic
		{
			get { return _roiGraphic; }
		}

		/// <summary>
		/// Gets the <see cref="RoiGraphic"/>'s Roi.
		/// </summary>
		protected InteractiveGraphic Roi
		{
			get { return _roiGraphic.Roi; }
		}

		/// <summary>
		/// Gets the <see cref="RoiGraphic"/>'s Callout.
		/// </summary>
		protected CalloutGraphic Callout
		{
			get { return _roiGraphic.Callout; }
		}

		#region IRoiCalloutLocationStrategy Members

		/// <summary>
		/// Sets the <see cref="InteractiveGraphics.RoiGraphic"/> that owns this strategy.
		/// </summary>
		public virtual void SetRoiGraphic(RoiGraphic roiGraphic)
		{
			_roiGraphic = roiGraphic;
		}

		/// <summary>
		/// Does nothing, unless overridden.
		/// </summary>
		public virtual void OnCalloutLocationChangedExternally()
		{
		}

		/// <summary>
		/// Calculates the initial callout location only; returns false thereafter.
		/// </summary>
		public virtual bool CalculateCalloutLocation(out PointF location, out CoordinateSystem coordinateSystem)
		{
			location = PointF.Empty;
			coordinateSystem = CoordinateSystem.Destination;

			if (!_initialLocationSet)
			{
				_initialLocationSet = true;

				//TODO: make the offset less hard-coded (use case Roi analyzers with many results).
				SizeF offset = new SizeF(0, 30);

				// Setup the callout
				this.Roi.CoordinateSystem = CoordinateSystem.Destination;
				location = Roi.ControlPoints[0] - offset;
				this.Roi.ResetCoordinateSystem();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Calculates the callout endpoint using the <see cref="InteractiveGraphic.GetClosestPoint"/> method.
		/// </summary>
		public virtual void CalculateCalloutEndPoint(out PointF endPoint, out CoordinateSystem coordinateSystem)
		{
			coordinateSystem = this.RoiGraphic.CoordinateSystem;
			endPoint = RoiGraphic.Roi.GetClosestPoint(RoiGraphic.Callout.StartPoint);
		}

		/// <summary>
		/// Creates a deep copy of this strategy object.
		/// </summary>
		/// <remarks>
		/// <see cref="IRoiCalloutLocationStrategy"/>s should not return null from this method.
		/// </remarks>
		public IRoiCalloutLocationStrategy Clone()
		{
			return CloneBuilder.Clone(this) as IRoiCalloutLocationStrategy;
		}

		#endregion
	}
}
