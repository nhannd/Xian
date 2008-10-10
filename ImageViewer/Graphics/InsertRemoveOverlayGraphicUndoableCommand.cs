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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	//TODO: separate into DeleteGraphicCommand and InsertGraphicCommand.

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
		private readonly IList<GraphicItem> _graphicOps;
		private bool _flagReverse = true;

		private InsertRemoveOverlayGraphicUndoableCommand(IOverlayGraphicsProvider overlayGraphicsProvider, IGraphic graphic, int graphicIndex)
			: base((IPresentationImage)overlayGraphicsProvider)
		{
			Platform.CheckForNullReference(graphic, "graphic");

			GraphicItem item;
			if (!overlayGraphicsProvider.OverlayGraphics.Contains(graphic))
				item = new GraphicItem(graphic, graphicIndex);
			else 
				item = new GraphicItem(graphic);

			_graphicOps = new List<GraphicItem>(1);
			_graphicOps.Add(item);
		}

		private InsertRemoveOverlayGraphicUndoableCommand(IOverlayGraphicsProvider overlayGraphicsProvider, IList<GraphicItem> graphicOps)
			: base((IPresentationImage)overlayGraphicsProvider) {
			Platform.CheckForNullReference(graphicOps, "graphics");
			Platform.CheckPositive(graphicOps.Count, "graphics");

			_graphicOps = graphicOps;
		}

		private GraphicCollection OverlayGraphics
		{
			get { return ((IOverlayGraphicsProvider) base.Drawable).OverlayGraphics; }	
		}

		private void Toggle()
		{
			if(_flagReverse)
			{
				int count = _graphicOps.Count;
				for(int n = 0; n < count; n++)
					_graphicOps[n].Toggle(this.OverlayGraphics);
			}
			else 
			{
				int count = _graphicOps.Count;
				for (int n = count - 1; n >= 0; n-- ) 
					_graphicOps[n].Toggle(this.OverlayGraphics);
			}
			_flagReverse = !_flagReverse;
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

		/// <summary>
		/// Factory method for creating an <see cref="InsertRemoveOverlayGraphicUndoableCommand"/> that,
		/// on undo, will insert a list of <see cref="IGraphic"/>s into an <see cref="IPresentationImage"/>'s overlay graphics
		/// at the corresponding indices in specified list.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The input <see cref="IPresentationImage"/> must implement <see cref="IOverlayGraphicsProvider"/> or an
		/// <see cref="InvalidCastException"/> will be thrown.
		/// </para>
		/// <para>
		/// The input <paramref name="graphics"/> must not exist in the <param name="presentationImage"/>'s overlay graphics or
		/// an <see cref="InvalidOperationException"/> will be thrown.
		/// </para>
		/// <para>
		/// The inputs <param name="graphics"/> and <param name="restoreIndices"/> must have the same length or an <see cref="ArgumentException"/>
		/// will be thrown.
		/// </para>
		/// </remarks>
		public static InsertRemoveOverlayGraphicUndoableCommand GetInsertCommand(IPresentationImage presentationImage, IList<IGraphic> graphics, IList<int> restoreIndices) {
			Platform.CheckForNullReference(presentationImage, "presentationImage");
			Platform.CheckForNullReference(graphics, "graphic");
			Platform.CheckPositive(graphics.Count, "graphics");
			Platform.CheckForNullReference(restoreIndices, "restoreIndices");
			Platform.CheckTrue(graphics.Count == restoreIndices.Count, "graphics and restoreIndices must be the same length");

			IOverlayGraphicsProvider overlayProvider = presentationImage as IOverlayGraphicsProvider;
			Platform.CheckForInvalidCast(overlayProvider, "presentationImage", typeof(IOverlayGraphicsProvider).FullName);

			foreach (IGraphic graphic in graphics) {
				if (overlayProvider.OverlayGraphics.Contains(graphic))
					throw new InvalidOperationException("Cannot create 'insert' command; one or more graphics are already in the collection.");
			}

			int count = overlayProvider.OverlayGraphics.Count;
			List<GraphicItem> list = new List<GraphicItem>(graphics.Count);
			for (int n = 0; n < graphics.Count; n++)
			{
				Platform.CheckIndexRange(restoreIndices[n], 0, count++, overlayProvider.OverlayGraphics);
				list.Add(new GraphicItem(graphics[n], restoreIndices[n]));
			}

			return new InsertRemoveOverlayGraphicUndoableCommand(overlayProvider, list.AsReadOnly());
		}

		/// <summary>
		/// Factory method for creating an <see cref="InsertRemoveOverlayGraphicUndoableCommand"/> that, 
		/// on undo, will remove a list of <see cref="IGraphic"/>s from an <see cref="IPresentationImage"/>'s overlay graphics.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The input <see cref="IPresentationImage"/> must implement <see cref="IOverlayGraphicsProvider"/> or an
		/// <see cref="InvalidCastException"/> will be thrown.
		/// </para>
		/// <para>
		/// The input <paramref name="graphics"/> must exist in the <see cref="IPresentationImage"/>'s overlay graphics or
		/// an <see cref="InvalidOperationException"/> will be thrown.
		/// </para>
		/// </remarks>
		public static InsertRemoveOverlayGraphicUndoableCommand GetRemoveCommand(IPresentationImage presentationImage, IList<IGraphic> graphics) {
			Platform.CheckForNullReference(presentationImage, "presentationImage");
			Platform.CheckForNullReference(graphics, "graphics");
			Platform.CheckPositive(graphics.Count, "graphics");

			IOverlayGraphicsProvider overlayProvider = presentationImage as IOverlayGraphicsProvider;
			Platform.CheckForInvalidCast(overlayProvider, "presentationImage", typeof(IOverlayGraphicsProvider).FullName);

			foreach (IGraphic graphic in graphics)
			{
				if (!overlayProvider.OverlayGraphics.Contains(graphic))
					throw new InvalidOperationException("Cannot create 'remove' command; one or more graphics are not currently in the collection.");
			}

			List<GraphicItem> list = new List<GraphicItem>(graphics.Count);
			for (int n = 0; n < graphics.Count; n++) {
				list.Add(new GraphicItem(graphics[n]));
			}

			return new InsertRemoveOverlayGraphicUndoableCommand(overlayProvider, list.AsReadOnly());
		}

		#region GraphicItem Class

		/// <summary>
		/// Represents an IGraphic to be inserted at a certain index, or removed
		/// </summary>
		private class GraphicItem {
			private readonly IGraphic _graphic;
			private int _index;

			/// <summary>
			/// Creates an item representing deleting the specified graphic.
			/// </summary>
			/// <param name="graphic"></param>
			public GraphicItem(IGraphic graphic)
				: this(graphic, -1) {
			}

			/// <summary>
			/// Creates an item representing inserting the specified graphic at the given index, or remove the graphic if the index is less than 0.
			/// </summary>
			/// <param name="graphic"></param>
			/// <param name="index"></param>
			public GraphicItem(IGraphic graphic, int index)
			{
				_graphic = graphic;
				_index = index;
			}

			public IGraphic Graphic
			{
				get { return _graphic; }
			}

			public int Index
			{
				get { return _index; }
			}

			public void Toggle(GraphicCollection overlayGraphics)
			{
				// use the index as a state toggle to determine if we are inserting it at this index, or removing it (from whatever index the item is actually at)
				if (_index >= 0)
				{
					overlayGraphics.Insert(_index, _graphic);
					_index = -1;
				}
				else
				{
					_index = overlayGraphics.IndexOf(_graphic);
					overlayGraphics.Remove(_graphic);
				}
			}
		}

		#endregion
	}
}