using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class AddGraphicUndoableCommand : UndoableCommand
	{
		private Command _command;

		public AddGraphicUndoableCommand(IGraphic graphic, GraphicCollection parentCollection)
		{
			_command = new AddGraphicCommand(graphic, parentCollection);
		}

		/// <summary>
		/// <see cref="Execute"/>s the insert command.
		/// </summary>
		public override void Execute()
		{
			if (_command is AddGraphicCommand)
			{
				_command.Execute();
				_command = ((AddGraphicCommand)_command).GetUndoCommand();
			}
			else if (_command is InsertGraphicCommand)
			{
				//after the initial add has executed (and has been undone) it's an insert from then on.
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

	internal class AddGraphicCommand : Command
	{
		private GraphicCollection _parentCollection;
		private IGraphic _graphic;

		private RemoveGraphicCommand _undoCommand;

		public AddGraphicCommand(IGraphic graphic, GraphicCollection parentCollection)
		{
			Platform.CheckForNullReference(graphic, "graphic");
			Platform.CheckForNullReference(parentCollection, "parentCollection");

			_parentCollection = parentCollection;
			_graphic = graphic;
		}

		public override void Execute()
		{
			if (_undoCommand != null)
				throw new InvalidOperationException("The command has already been executed.");

			_parentCollection.Add(_graphic);
			_undoCommand = new RemoveGraphicCommand(_graphic);

			_parentCollection = null;
			_graphic = null;
		}

		internal Command GetUndoCommand()
		{
			if (_undoCommand == null)
				throw new InvalidOperationException("The command must be executed first.");

			return _undoCommand;
		}
	}
}