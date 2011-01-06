#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A subclass of <see cref="UndoableCommand"/> for <see cref="IDrawable"/> objects.
	/// </summary>
	/// <remarks>
	/// Often, when an <see cref="UndoableCommand"/> is <see cref="Execute"/>d or
	/// <see cref="Unexecute"/>d, it is necessary to refresh an <see cref="IDrawable"/>
	/// object, such as an <see cref="ITile"/>.  This class automatically calls
	/// <see cref="IDrawable.Draw"/> on the object passed to the constructor
	/// after <see cref="Execute"/> and <see cref="Unexecute"/>.
	/// </remarks>
	public class DrawableUndoableCommand : CompositeUndoableCommand
	{
		private readonly IDrawable _drawable;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="drawable">The object to redraw after <see cref="Execute"/> or <see cref="Unexecute"/>.</param>
		public DrawableUndoableCommand(IDrawable drawable)
		{
			Platform.CheckForNullReference(drawable, "drawable");

			_drawable = drawable;
		}

		/// <summary>
		/// Gets the <see cref="IDrawable"/> object that will be redrawn on <see cref="Unexecute"/>
		/// or <see cref="Execute"/>.
		/// </summary>
		protected IDrawable Drawable
		{
			get { return _drawable; }	
		}

		/// <summary>
		/// Performs a 'redo' and calls <see cref="IDrawable.Draw"/> on <see cref="Drawable"/> afterwards.
		/// </summary>
		public override void Execute()
		{
			base.Execute();
			_drawable.Draw();
		}

		/// <summary>
		/// Performs an 'undo' and calls <see cref="IDrawable.Draw"/> on <see cref="Drawable"/> afterwards.
		/// </summary>
		public override void Unexecute()
		{
			base.Unexecute();
			_drawable.Draw();
		}
	}
}
