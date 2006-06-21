using System;
using ClearCanvas.Common;

namespace ClearCanvas.Common.Application
{
	public class CommandAddedEventArgs : EventArgs
	{
		private UndoableCommand m_Command;

		public CommandAddedEventArgs(UndoableCommand command)
		{
			Platform.CheckForNullReference(command, "command");

			m_Command = command;
		}

		public UndoableCommand Command { get { return m_Command; } }
	}
}
