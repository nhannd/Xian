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
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines the final image that is presented to the user in an <see cref="ITile"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// An <see cref="IPresentationImage"/> can be thought of as a “scene” 
	/// composed of many parts, be they images, lines, text, etc.  It is the
	/// image that is presented to the user in a <see cref="Tile"/>.
	/// </para>
	/// <para>
	/// You should not implement <see cref="IPresentationImage"/> directly.
	/// Instead, subclass <see cref="PresentationImage"/>.
	/// </para>
	/// </remarks>
	/// <seealso cref="IDisplaySet">for more information on disposal of <see cref="IDisplaySet"/>s and <see cref="IPresentationImage"/>s.</seealso>
	public interface IPresentationImage : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="IPresentationImage"/> is not part of the 
		/// logical workspace yet.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the parent <see cref="IDisplaySet"/>.
		/// </summary>
		/// <value>The parent <see cref="IDisplaySet"/> or <b>null</b> if the 
		/// <see cref="IPresentationImage"/> has not
		/// been added to the <see cref="IDisplaySet"/> yet.</value>
		IDisplaySet ParentDisplaySet { get; }

		/// <summary>
		/// Gets the associated <see cref="ITile"/>.
		/// </summary>
		/// <value>The <see cref="ITile"/> that currently contains the
		/// <see cref="IPresentationImage"/> or <b>null</b> if the 
		/// <see cref="IPresentationImage"/> is not currently visible.</value>
		ITile Tile { get; }

		/// <summary>
		/// Gets the client rectangle of the surface on which the
		/// presentation image will be rendered.
		/// </summary>
		Rectangle ClientRectangle { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="IPresentationImage"/>
		/// is linked.
		/// </summary>
		bool Linked { get; set; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="IPresentationImage"/>
		/// is selected.
		/// </summary>
		bool Selected { get; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="IPresentationImage"/>
		/// is visible.
		/// </summary>
		bool Visible { get; }

		/// <summary>
		/// Gets the currently selected <see cref="IGraphic"/>.
		/// </summary>
		/// <value>The currently selected <see cref="IGraphic"/> or <b>null</b>
		/// if no <see cref="IGraphic"/> is currently selected.</value>
		/// <remarks>
		/// It is possible for an <see cref="IGraphic"/> to be selected,
		/// focussed or selected and focussed.
		/// </remarks>
		ISelectableGraphic SelectedGraphic { get; set; }

		/// <summary>
		/// Gets the currently focussed <see cref="IGraphic"/>.
		/// </summary>
		/// <value>The currently selected <see cref="IGraphic"/> or <b>null</b>
		/// if no <see cref="IGraphic"/> is currently focussed.</value>
		/// <remarks>
		/// It is possible for an <see cref="IGraphic"/> to be selected,
		/// focussed or selected and focussed.
		/// </remarks>
		IFocussableGraphic FocussedGraphic { get; set; }

		/// <summary>
		/// Gets or sets unique identifier for this <see cref="IPresentationImage"/>.
		/// </summary>
		string Uid { get; set; }

		/// <summary>
		/// Creates a fresh copy of the <see cref="IPresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="IPresentationImage"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		/// <returns></returns>
		IPresentationImage CreateFreshCopy();

		/// <summary>
		/// Creates a deep copy of the <see cref="IPresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="IPresentationImage"/>s should never return null from this method.
		/// </remarks>
		IPresentationImage Clone();

		/// <summary>
		/// Renders the <see cref="PresentationImage"/> to an offscreen <see cref="Bitmap"/>.
		/// </summary>
		/// <param name="width">Bitmap width.</param>
		/// <param name="height">Bitmap height.</param>
		/// <returns></returns>
		/// <remarks>
		/// This method can be used anywhere an offscreen bitmap is required, such as 
		/// paper/DICOM printing, thumbnail generation, creation of new DICOM images, etc.
		/// </remarks>
		Bitmap DrawToBitmap(int width, int height);
	}
}
