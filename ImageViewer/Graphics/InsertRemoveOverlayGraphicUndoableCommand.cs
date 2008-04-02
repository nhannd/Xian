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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A subclass of <see cref="DrawableUndoableCommand"/> that performs
	/// undo/redo operations for insertion and removal of an <see cref="IGraphic"/>
	/// from an <see cref="IPresentationImage"/>'s overlay graphics.
	/// </summary>
	/// <remarks>
	/// The <see cref="IPresentationImage"/> must implement <see cref="IOverlayGraphicsProvider"/>.
	/// </remarks>
	public class InsertRemoveOverlayGraphicUndoableCommand : DrawableUndoableCommand
	{
		private IGraphic _graphic;
		private int _restoreIndex;

		private InsertRemoveOverlayGraphicUndoableCommand(IOverlayGraphicsProvider overlayGraphicsProvider, IGraphic graphic, int graphicIndex)
			: base((IPresentationImage)overlayGraphicsProvider)
		{
			Platform.CheckForNullReference(graphic, "graphic");

			if (!overlayGraphicsProvider.OverlayGraphics.Contains(graphic))
				_graphic = graphic;
			else 
				_graphic = null;

			_restoreIndex = graphicIndex;
		}

		private GraphicCollection OverlayGraphics
		{
			get { return ((IOverlayGraphicsProvider) base.Drawable).OverlayGraphics; }	
		}

		private void Insert()
		{
			OverlayGraphics.Insert(_restoreIndex, _graphic);
			_graphic = null;
		}

		private void Remove()
		{
			_graphic = OverlayGraphics[_restoreIndex];
			OverlayGraphics.RemoveAt(_restoreIndex);
		}

		private void Toggle()
		{
			if (_graphic != null)
				Insert();
			else
				Remove();
		}

		/// <summary>
		/// Performs a 'redo' operation, either inserting or removing the
		/// <see cref="IGraphic"/> from <see cref="IOverlayGraphicsProvider.OverlayGraphics"/> depending
		/// on which type of operation (insert or remove) is being 'redone'.
		/// </summary>
		public override void Execute()
		{
			Toggle();
			base.Execute();
		}

		/// <summary>
		/// Performs an 'undo' operation, either inserting or removing the
		/// <see cref="IGraphic"/> from <see cref="IOverlayGraphicsProvider.OverlayGraphics"/> depending
		/// on which type of operation (insert or remove) is being 'undone'.
		/// </summary>
		public override void Unexecute()
		{
			Toggle();
			base.Unexecute();
		}

		/// <summary>
		/// Factory method for creating an <see cref="InsertRemoveOverlayGraphicUndoableCommand"/> that,
		/// on undo, will insert an <see cref="IGraphic"/> into an <see cref="IPresentationImage"/>'s overlay graphics.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The input <see cref="IPresentationImage"/> must implement <see cref="IOverlayGraphicsProvider"/> or an
		/// <see cref="InvalidCastException"/> will be thrown.
		/// </para>
		/// <para>
		/// The input <paramref name="graphic"/> must not exist in the <see cref="IPresentationImage"/>'s overlay graphics or
		/// an <see cref="InvalidOperationException"/> will be thrown.
		/// </para>
		/// </remarks>
		public static InsertRemoveOverlayGraphicUndoableCommand GetInsertCommand(IPresentationImage presentationImage, IGraphic graphic, int restoreIndex)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");
			Platform.CheckForNullReference(graphic, "graphic");

			IOverlayGraphicsProvider overlayProvider = presentationImage as IOverlayGraphicsProvider;
			Platform.CheckForInvalidCast(overlayProvider, "presentationImage", typeof(IOverlayGraphicsProvider).FullName);

			if (overlayProvider.OverlayGraphics.Contains(graphic))
				throw new InvalidOperationException("Cannot create 'insert' command; the graphic is already in the collection.");

			Platform.CheckIndexRange(restoreIndex, 0, overlayProvider.OverlayGraphics.Count, overlayProvider.OverlayGraphics);

			return new InsertRemoveOverlayGraphicUndoableCommand(overlayProvider, graphic, restoreIndex);
		}

		/// <summary>
		/// Factory method for creating an <see cref="InsertRemoveOverlayGraphicUndoableCommand"/> that, 
		/// on undo, will remove an <see cref="IGraphic"/> from an <see cref="IPresentationImage"/>'s overlay graphics.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The input <see cref="IPresentationImage"/> must implement <see cref="IOverlayGraphicsProvider"/> or an
		/// <see cref="InvalidCastException"/> will be thrown.
		/// </para>
		/// <para>
		/// The input <paramref name="graphic"/> must exist in the <see cref="IPresentationImage"/>'s overlay graphics or
		/// an <see cref="InvalidOperationException"/> will be thrown.
		/// </para>
		/// </remarks>
		public static InsertRemoveOverlayGraphicUndoableCommand GetRemoveCommand(IPresentationImage presentationImage, IGraphic graphic)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");
			Platform.CheckForNullReference(graphic, "graphic");

			IOverlayGraphicsProvider overlayProvider = presentationImage as IOverlayGraphicsProvider;
			Platform.CheckForInvalidCast(overlayProvider, "presentationImage", typeof(IOverlayGraphicsProvider).FullName);

			if (!overlayProvider.OverlayGraphics.Contains(graphic))
				throw new InvalidOperationException("Cannot create 'remove' command; the graphic is not currently in the collection.");

			return new InsertRemoveOverlayGraphicUndoableCommand(overlayProvider, graphic, overlayProvider.OverlayGraphics.IndexOf(graphic));
		}
	}
}