#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An <see cref="UndoableCommand"/> for inserting an <see cref="IGraphic"/> into a <see cref="GraphicCollection"/>.
	/// </summary>
	public class InsertGraphicUndoableCommand : UndoableCommand
	{
		private Command _command;

		/// <summary>
		/// Constructor.
		/// </summary>
		public InsertGraphicUndoableCommand(IGraphic graphic, GraphicCollection parentCollection, int index)
		{
			_command = new InsertGraphicCommand(graphic, parentCollection, index);
		}

		/// <summary>
		/// <see cref="Execute"/>s the insert command.
		/// </summary>
		public override void Execute()
		{
			if (_command is InsertGraphicCommand)
			{
				_command.Execute();
				_command = ((InsertGraphicCommand)_command).GetUndoCommand();
			}
		}

		/// <summary>
		/// <see cref="Unexecute"/>s the insert command (e.g. removes the graphic).
		/// </summary>
		public override void Unexecute()
		{
			if (_command is RemoveGraphicCommand)
			{
				_command.Execute();
				_command = ((RemoveGraphicCommand)_command).GetUndoCommand();
			}
		}
	}

	internal class InsertGraphicCommand : Command
	{
		private GraphicCollection _parentCollection;
		private IGraphic _graphic;
		private int _index;
		
		private RemoveGraphicCommand _undoCommand;

		public InsertGraphicCommand(IGraphic graphic, GraphicCollection parentCollection, int index)
		{
			Platform.CheckForNullReference(graphic, "graphic");
			Platform.CheckForNullReference(parentCollection, "parentCollection");
			Platform.CheckTrue(index >= 0, "Insert index positive");

			_parentCollection = parentCollection;
			_graphic = graphic;
			_index = index;
		}

		private void Validate()
		{
			if (_graphic.ParentGraphic != null)
				throw new InvalidOperationException("The graphic already has a parent.");

			if (_parentCollection.Contains(_graphic))
				throw new InvalidOperationException("The graphic is already in the collection.");

			Platform.CheckArgumentRange(_index, 0, _parentCollection.Count, "Insert index is out of range.");
		}

		public RemoveGraphicCommand GetUndoCommand()
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

			_parentCollection.Insert(_index, _graphic);

			_undoCommand = new RemoveGraphicCommand(_graphic);

			_parentCollection = null;
			_graphic = null;
			_index = -1;
		}
	}
}
