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

using System.Collections.Generic;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	public abstract class DrawableUndoableOperation<T> : UndoableOperation<T> where T : class, IDrawable
	{
		protected DrawableUndoableOperation()
		{
		}

		public static DrawableUndoableCommand Apply(IUndoableOperation<T> operation, T item)
		{
			MemorableUndoableCommand memorableCommand = UndoableOperation<T>.Apply(operation, item);
			if (memorableCommand == null)
				return null;

			//we have to draw the item ourselves b/c the operation has already been applied.
			item.Draw();

			DrawableUndoableCommand drawableCommand = new DrawableUndoableCommand(item);
			drawableCommand.Enqueue(memorableCommand);
			return drawableCommand;
		}

		public static CompositeUndoableCommand Apply(IUndoableOperation<T> operation, IEnumerable<T> items)
		{
			CompositeUndoableCommand returnCommand = new CompositeUndoableCommand();
			foreach (T item in items)
			{
				DrawableUndoableCommand drawableCommand = Apply(operation, item);
				if (drawableCommand != null)
					returnCommand.Enqueue(drawableCommand);
			}

			if (returnCommand.Count > 0)
				return returnCommand;
			else
				return null;
		}
	}
}
