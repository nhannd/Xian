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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// The default strategy for automatically calculating the location of a <see cref="AnnotationGraphic"/>'s callout.
	/// </summary>
	[Cloneable(true)]
	public class AnnotationCalloutLocationStrategy : IAnnotationCalloutLocationStrategy
	{
		[CloneIgnore]
		private AnnotationGraphic _annotationGraphic;
		private bool _initialLocationSet;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal protected AnnotationCalloutLocationStrategy()
		{
			_initialLocationSet = false;
		}

		/// <summary>
		/// Gets the owning <see cref="AnnotationGraphic"/>.
		/// </summary>
		protected AnnotationGraphic AnnotationGraphic
		{
			get { return _annotationGraphic; }
		}

		/// <summary>
		/// Gets the <see cref="AnnotationGraphic"/>'s Subject.
		/// </summary>
		protected IGraphic Roi
		{
			get { return _annotationGraphic.Subject; }
		}

		/// <summary>
		/// Gets the <see cref="AnnotationGraphic"/>'s Callout.
		/// </summary>
		protected CalloutGraphic Callout
		{
			get { return _annotationGraphic.Callout; }
		}

		#region IRoiCalloutLocationStrategy Members

		/// <summary>
		/// Sets the <see cref="AnnotationGraphic"/> that owns this strategy.
		/// </summary>
		public virtual void SetAnnotationGraphic(AnnotationGraphic annotationGraphic)
		{
			_annotationGraphic = annotationGraphic;
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
				SizeF offset = new SizeF(0, 55);

				// Setup the callout
				this.Roi.CoordinateSystem = CoordinateSystem.Destination;
				location = RectangleUtilities.ConvertToPositiveRectangle(Roi.BoundingBox).Location - offset;
				this.Roi.ResetCoordinateSystem();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Calculates the callout endpoint using the <see cref="IGraphic.GetClosestPoint"/> method.
		/// </summary>
		public virtual void CalculateCalloutEndPoint(out PointF endPoint, out CoordinateSystem coordinateSystem)
		{
			coordinateSystem = this.AnnotationGraphic.CoordinateSystem;
			endPoint = AnnotationGraphic.Subject.GetClosestPoint(AnnotationGraphic.Callout.StartPoint);
		}

		/// <summary>
		/// Creates a deep copy of this strategy object.
		/// </summary>
		/// <remarks>
		/// <see cref="IAnnotationCalloutLocationStrategy"/>s should not return null from this method.
		/// </remarks>
		public IAnnotationCalloutLocationStrategy Clone()
		{
			return CloneBuilder.Clone(this) as IAnnotationCalloutLocationStrategy;
		}

		#endregion
	}
}
