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

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for a single <see cref="IPresentationImage"/>.
	/// </summary>
	public interface ITile : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="ITile"/> is not part of the 
		/// physical workspace yet.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the parent <see cref="IImageBox"/>
		/// </summary>
		/// <value>The parent <see cref="IImageBox"/> or <b>null</b> if the 
		/// <see cref="ITile"/> has not
		/// been added to the <see cref="IImageBox"/> yet.</value>
		IImageBox ParentImageBox { get; }

		/// <summary>
		/// Gets the <see cref="IPresentationImage"/> associated with this
		/// <see cref="ITile"/>.
		/// </summary>
		IPresentationImage PresentationImage { get; }

		/// <summary>
		/// Gets this <see cref="ITile"/>'s normalized rectangle.
		/// </summary>
		/// <remarks>
		/// Normalized coordinates specify the top-left corner,
		/// width and height of the <see cref="ITile"/> as a 
		/// fraction of the image box.  For example, if the
		/// <see cref="NormalizedRectangle"/> is (left=0.25, top=0.0, width=0.5, height=0.5) 
		/// and the image box has dimensions of (width=1000, height=800), the 
		/// <see cref="ITile"/> rectangle would be (left=250, top=0, width=500, height=400)
		/// </remarks>
		RectangleF NormalizedRectangle { get; }

		/// <summary>
		/// Gets this <see cref="ITile"/>'s client rectangle.
		/// </summary>
		Rectangle ClientRectangle { get; }

		/// <summary>
		/// Gets the presentation image index.
		/// </summary>
		int PresentationImageIndex { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ITile"/> is
		/// selected.
		/// </summary>
		/// <remarks>
		/// <see cref="ITile"/> selection is mutually exclusive.  That is,
		/// only one <see cref="ITile"/> is ever selected at a given time.  
		/// </remarks>
		bool Selected { get; }

		/// <summary>
		/// Gets or sets whether the tile is currently enabled.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets this <see cref="ITile"/>'s <see cref="ClearCanvas.ImageViewer.InformationBox">InformationBox</see>.
		/// </summary>
		InformationBox InformationBox { get; set; }

		/// <summary>
		/// Gets or sets this <see cref="ITile"/>'s <see cref="ClearCanvas.ImageViewer.EditBox">EditBox</see>.
		/// </summary>
		EditBox EditBox { get; set; }

		/// <summary>
		/// Selects the <see cref="ITile"/>.
		/// </summary>
		/// <remarks>
		/// Selecting a <see cref="ITile"/> also selects the containing <see cref="IImageBox"/>
		/// and deselects any other currently seleccted <see cref="ITile"/> 
		/// and <see cref="IImageBox"/>.
		/// </remarks>
		void Select();

		/// <summary>
		/// Occurs when the <see cref="InformationBox"/> property has changed.
		/// </summary>
		event EventHandler<InformationBoxChangedEventArgs> InformationBoxChanged;

		/// <summary>
		/// Occurs when the <see cref="EditBox"/> property has changed.
		/// </summary>
		event EventHandler EditBoxChanged;
	}
}
