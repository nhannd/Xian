using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop 
{
	public class CommandHistory 
	{
		// Private attributes
		private List<UndoableCommand> _History = new List<UndoableCommand>();
		private int _MaxSize = 0;
		private int _CurrentCommandIndex = -1;
		private int _LastCommandIndex = -1;

		private event EventHandler _CurrentCommandChangedEvent;

		// Constructor
		public CommandHistory(int maxSize)
		{
			Platform.CheckPositive(maxSize, "maxSize");

			_MaxSize = maxSize;
		}

		// Properties
		public int NumCommands 
		{
			get
			{
				return _History.Count;
			}
		}

		public int MaxSize
		{
			get
			{
				return _MaxSize;
			}
		}

		public int CurrentCommandIndex
		{
			get
			{
				return _CurrentCommandIndex;
			}
		}

		public int LastCommandIndex
		{
			get
			{
				return _LastCommandIndex;
			}
		}

		// Event accessors
		public event EventHandler CurrentCommandChanged
		{
			add
			{
				_CurrentCommandChangedEvent += value;
			}
			remove
			{
				_CurrentCommandChangedEvent -= value;
			}
		}

		// Public methods
		public void AddCommand(UndoableCommand command)
		{
			Platform.CheckForNullReference(command, "command");

			if (_CurrentCommandIndex < _LastCommandIndex)
			{
				int numCommandsToRemove = _LastCommandIndex - _CurrentCommandIndex;
				_History.RemoveRange(_CurrentCommandIndex + 1, numCommandsToRemove);
				_LastCommandIndex -= numCommandsToRemove;
			}

			_History.Add(command);

			if (NumCommands > _MaxSize)
			{
				_History.RemoveAt(0);

				if (_CurrentCommandIndex == _LastCommandIndex)
					_CurrentCommandIndex--;

				_LastCommandIndex--;
			}

			_CurrentCommandIndex++;
			_LastCommandIndex++;

			EventsHelper.Fire(_CurrentCommandChangedEvent, this, EventArgs.Empty);
		}

		public void Redo()
		{
			if (NumCommands == 0)
				return;

			if (_CurrentCommandIndex == _LastCommandIndex)
				return;

			_CurrentCommandIndex++;
			UndoableCommand cmd = _History[_CurrentCommandIndex];

			try
			{
				cmd.Execute();
			}
			catch (Exception e)
			{
				Platform.Log(e, LogLevel.Error);
			}

			EventsHelper.Fire(_CurrentCommandChangedEvent, this, EventArgs.Empty);
		}

		public void Undo()
		{
			if (NumCommands == 0)
				return;

			if (_CurrentCommandIndex == -1)
				return;

			UndoableCommand cmd = _History[_CurrentCommandIndex];

			try
			{
				cmd.Unexecute();
			}
			catch (Exception e)
			{
				Platform.Log(e, LogLevel.Error);
			}

			_CurrentCommandIndex--;

			EventsHelper.Fire(_CurrentCommandChangedEvent, this, EventArgs.Empty);
		}
	}
}
