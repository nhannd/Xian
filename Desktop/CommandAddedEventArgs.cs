using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	public class CommandAddedEventArgs : EventArgs
	{
		private UndoableCommand _Command;

		public CommandAddedEventArgs(UndoableCommand command)
		{
			Platform.CheckForNullReference(command, "command");

			_Command = command;
		}

		public UndoableCommand Command { get { return _Command; } }
	}
}
