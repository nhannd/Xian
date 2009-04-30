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
using ClearCanvas.Desktop;
using System.Drawing;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for <see cref="IImageBox"/> objects.
	/// </summary>
	public interface IPhysicalWorkspace : IDrawable, IMemorable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the associated <see cref="ILogicalWorkspace"/>.
		/// </summary>
		ILogicalWorkspace LogicalWorkspace { get; }

		/// <summary>
		/// Gets the collection of <see cref="IImageBox"/> objects that belong
		/// to this <see cref="IPhysicalWorkspace"/>.
		/// </summary>
		ImageBoxCollection ImageBoxes { get; }

		/// <summary>
		/// Gets the selected <see cref="IImageBox"/>.
		/// </summary>
		/// <value>The currently selected <see cref="IImageBox"/>, or <b>null</b> if
		/// no <see cref="IImageBox"/> is currently selected.</value>
		IImageBox SelectedImageBox { get; }

		/// <summary>
		/// Gets the number of rows of <see cref="IImageBox"/> objects in the
		/// <see cref="IPhysicalWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="Rows"/> is <i>only</i> valid if <see cref="SetImageBoxGrid"/> has
		/// been called.  Otherwise, the value is meaningless.
		/// </remarks>
		int Rows { get; }

		/// <summary>
		/// Gets the number of columns of <see cref="IImageBox"/> objects in the
		/// <see cref="IPhysicalWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="Columns"/> is <i>only</i> valid if <see cref="SetImageBoxGrid"/> has
		/// been called.  Otherwise, the value is meaningless.
		/// </remarks>
		int Columns { get; }

		/// <summary>
		/// Returns the image box at a specified row and column index.
		/// </summary>
		/// <param name="row">the zero-based row index of the image box to retrieve</param>
		/// <param name="column">the zero-based column index of the image box to retrieve</param>
		/// <returns>the image box at the specified row and column indices</returns>
		/// <remarks>This method is only valid if <see cref="SetImageBoxGrid"/> has been called and/or the 
		/// layout is, in fact, rectangular.</remarks>
		IImageBox this[int row, int column] { get; }

		/// <summary>
		/// Gets or sets whether the workspace is currently enabled.
		/// </summary>
		bool Enabled { get; set; }
		event EventHandler EnabledChanged;

		bool Locked { get; set; }
		event EventHandler LockedChanged;

		/// <summary>
		/// Gets the rectangle that the <see cref="IPhysicalWorkspace"/> occupies
		/// in virtual screen coordinates.
		/// </summary>
		Rectangle ScreenRectangle { get; }

		/// <summary>
		/// Occurs when <see cref="ScreenRectangle"/> changes.
		/// </summary>
		event EventHandler ScreenRectangleChanged;

		/// <summary>
		/// Occurs when all changes to image box collection are complete.
		/// </summary>
		/// <remarks>
		/// <see cref="LayoutCompleted"/> is raised by the Framework when
		/// <see cref="SetImageBoxGrid"/> has been called.  If you are adding/removing
		/// <see cref="IImageBox"/> objects manually, you should raise this event when
		/// you're done by calling <see cref="OnLayoutCompleted"/>.  This event is
		/// consumed by the view to reduce flicker when layouts are changed.  
		/// In that way, it is similar to the WinForms methods <b>SuspendLayout</b>
		/// and <b>ResumeLayout</b>.
		/// </remarks>
		event EventHandler LayoutCompleted;

		/// <summary>
		/// Creates a rectangular <see cref="IImageBox"/> grid.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// <see cref="SetImageBoxGrid"/> is a convenience method that adds
		/// <see cref="IImageBox"/> objects to the <see cref="IPhysicalWorkspace"/>
		/// in a rectangular grid.
		/// </remarks>
		void SetImageBoxGrid(int rows, int columns);

		/// <summary>
		/// Raises the <see cref="LayoutCompleted"/> event.
		/// </summary>
		/// <remarks>
		/// If you are adding/removing <see cref="IImageBox"/> objects manually 
		/// (i.e., instead of using <see cref="SetImageBoxGrid"/>), you should call
		/// <see cref="OnLayoutCompleted"/> to raise the <see cref="LayoutCompleted"/> event.  
		/// This event is consumed by the view to reduce flicker when layouts are changed.  
		/// In that way, it is similar to the WinForms methods <b>SuspendLayout</b>
		/// and <b>ResumeLayout</b>.
		/// </remarks>
		void OnLayoutCompleted();

		/// <summary>
		/// Selects the first <see cref="IImageBox"/> in the image box collection.
		/// </summary>
		/// <remarks>
		/// When <see cref="SetImageBoxGrid"/> has been used to setup the 
		/// <see cref="IPhysicalWorkspace"/>, the first <see cref="IImageBox"/> in the
		/// image box collection will be the top-left <see cref="IImageBox"/>.
		/// </remarks>
		void SelectDefaultImageBox();
	}
}
