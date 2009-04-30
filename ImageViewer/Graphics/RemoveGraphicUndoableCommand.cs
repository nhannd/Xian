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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An <see cref="UndoableCommand"/> for removing an <see cref="IGraphic"/> from a <see cref="GraphicCollection"/>.
	/// </summary>
	public class RemoveGraphicUndoableCommand : UndoableCommand
	{
		private Command _command;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// The <paramref name="graphic"/>'s <see cref="IGraphic.ParentGraphic"/> must not be null.
		/// </remarks>
		public RemoveGraphicUndoableCommand(IGraphic graphic)
		{
			_command = new RemoveGraphicCommand(graphic);
		}

		/// <summary>
		/// <see cref="Execute"/>s the remove command.
		/// </summary>
		public override void Execute()
		{
			if (_command is RemoveGraphicCommand)
			{
				_command.Execute();
				_command = ((RemoveGraphicCommand)_command).GetUndoCommand();
			}
		}

		/// <summary>
		/// <see cref="Unexecute"/>s the remove command (e.g. re-inserts the graphic).
		/// </summary>
		public override void Unexecute()
		{
			if (_command is InsertGraphicCommand)
			{
				_command.Execute();
				_command = ((InsertGraphicCommand)_command).GetUndoCommand();
			}
		}
	}
	
	internal class RemoveGraphicCommand : Command
	{
		private IGraphic _graphic;
		private InsertGraphicCommand _undoCommand;

		public RemoveGraphicCommand(IGraphic graphic)
		{
			Platform.CheckForNullReference(graphic, "graphic");

			_graphic = graphic;
			Validate();
		}

		private void Validate()
		{
			CompositeGraphic parentGraphic = _graphic.ParentGraphic as CompositeGraphic;
			if (parentGraphic == null)
				throw new InvalidOperationException("The graphic must have a parent.");
		}

		public InsertGraphicCommand GetUndoCommand()
		{
			if (_undoCommand == null)
				throw new InvalidOperationException("The command must be executed first.");

			return _undoCommand;
		}

		public override void Execute()
		{
			if (_undoCommand != null)
				throw new InvalidOperationException("The command has already been executed.");

			Validate();

			GraphicCollection parentCollection = ((CompositeGraphic)_graphic.ParentGraphic).Graphics;
			int restoreIndex = parentCollection.IndexOf(_graphic);
			parentCollection.Remove(_graphic);

			_undoCommand = new InsertGraphicCommand(_graphic, parentCollection, restoreIndex);
			_graphic = null;
		}
	}
}
