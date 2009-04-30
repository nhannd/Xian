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
