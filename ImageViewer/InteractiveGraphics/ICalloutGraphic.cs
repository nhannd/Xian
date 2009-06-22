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
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Defines an <see cref="IVectorGraphic"/> that consists of some text content associated with a particular anchor point.
	/// </summary>
	public interface ICalloutGraphic : IVectorGraphic
	{
		/// <summary>
		/// Gets the callout text.
		/// </summary>
		string Text { get; }

		/// <summary>
		/// Gets or sets the font size in points used to display the callout text.
		/// </summary>
		/// <remarks>
		/// The default font size is 10 points.
		/// </remarks>
		float FontSize { get; set; }

		/// <summary>
		/// Gets or sets the font name used to display the callout text.
		/// </summary>
		/// <remarks>
		/// The default font is Arial.
		/// </remarks>
		string FontName { get; set; }

		/// <summary>
		/// Gets the bounding rectangle of the text portion of the callout.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		RectangleF TextBoundingBox { get; }

		/// <summary>
		/// Gets or sets the location of the center of the text.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		PointF TextLocation { get; set; }

		/// <summary>
		/// Occurs when the value of the <see cref="TextLocation"/> property changes.
		/// </summary>
		event EventHandler TextLocationChanged;

		/// <summary>
		/// Gets or sets the point where the callout is anchored.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		PointF AnchorPoint { get; set; }

		/// <summary>
		/// Occurs when the value of the <see cref="AnchorPoint"/> property changes.
		/// </summary>
		event EventHandler AnchorPointChanged;
	}
}