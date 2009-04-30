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

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A strategy for automatically calculating the location of a <see cref="XAnnotationGraphic"/>'s callout.
	/// </summary>
	public interface IAnnotationCalloutLocationStrategy
	{
		/// <summary>
		/// Sets the <see cref="XAnnotationGraphic"/> that owns this strategy.
		/// </summary>
		void SetAnnotationGraphic(AnnotationGraphic annotationGraphic);

		/// <summary>
		/// Called when the <see cref="XAnnotationGraphic"/>'s callout location has been changed externally; for example, by the user.
		/// </summary>
		void OnCalloutLocationChangedExternally();

		/// <summary>
		/// Called by the owning <see cref="XAnnotationGraphic"/> to get the callout's new location.
		/// </summary>
		/// <param name="location">The new location of the callout.</param>
		/// <param name="coordinateSystem">The <see cref="CoordinateSystem"/> of <paramref name="location"/>.</param>
		/// <returns>True if the callout location needs to change, false otherwise.</returns>
		bool CalculateCalloutLocation(out PointF location, out CoordinateSystem coordinateSystem);

		/// <summary>
		/// Called by the owning <see cref="XAnnotationGraphic"/> to get the callout's end point.
		/// </summary>
		/// <param name="endPoint">The callout end point.</param>
		/// <param name="coordinateSystem">The <see cref="CoordinateSystem"/> of <paramref name="endPoint"/>.</param>
		void CalculateCalloutEndPoint(out PointF endPoint, out CoordinateSystem coordinateSystem);

		/// <summary>
		/// Creates a deep copy of this strategy object.
		/// </summary>
		/// <remarks>
		/// <see cref="IXAnnotationCalloutLocationStrategy"/>s should not return null from this method.
		/// </remarks>
		IAnnotationCalloutLocationStrategy Clone();
	}
}
