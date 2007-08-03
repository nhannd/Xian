using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop 
{
	public class CommandHistory 
	{
		// Private attributes
		private List<UndoableCommand> _history = new List<UndoableCommand>();
		private int _maxSize = 0;
		private int _currentCommandIndex = -1;
		private int _lastCommandIndex = -1;

		private event EventHandler _currentCommandChangedEvent;

		// Constructor
		public CommandHistory(int maxSize)
		{
			Platform.CheckPositive(maxSize, "maxSize");

			_maxSize = maxSize;
		}

		// Properties
		public int NumCommands 
		{
			get
			{
				return _history.Count;
			}
		}

		public int MaxSize
		{
			get
			{
				return _maxSize;
			}
		}

		public int CurrentCommandIndex
		{
			get
			{
				return _currentCommandIndex;
			}
		}

		public int LastCommandIndex
		{
			get
			{
				return _lastCommandIndex;
			}
		}

		// Event accessors
		public event EventHandler CurrentCommandChanged
		{
			add
			{
				_currentCommandChangedEvent += value;
			}
			remove
			{
				_currentCommandChangedEvent -= value;
			}
		}

		// Public methods
		public void AddCommand(UndoableCommand command)
		{
			Platform.CheckForNullReference(command, "command");

			if (_currentCommandIndex < _lastCommandIndex)
			{
				int numCommandsToRemove = _lastCommandIndex - _currentCommandIndex;
				_history.RemoveRange(_currentCommandIndex + 1, numCommandsToRemove);
				_lastCommandIndex -= numCommandsToRemove;
			}

			_history.Add(command);

			if (NumCommands > _maxSize)
			{
				_history.RemoveAt(0);

				if (_currentCommandIndex == _lastCommandIndex)
					_currentCommandIndex--;

				_lastCommandIndex--;
			}

			_currentCommandIndex++;
			_lastCommandIndex++;

			EventsHelper.Fire(_currentCommandChangedEvent, this, EventArgs.Empty);
		}

		public void Redo()
		{
			if (NumCommands == 0)
				return;

			if (_currentCommandIndex == _lastCommandIndex)
				return;

			_currentCommandIndex++;
			UndoableCommand cmd = _history[_currentCommandIndex];

			try
			{
				cmd.Execute();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			EventsHelper.Fire(_currentCommandChangedEvent, this, EventArgs.Empty);
		}

		public void Undo()
		{
			if (NumCommands == 0)
				return;

			if (_currentCommandIndex == -1)
				return;

			UndoableCommand cmd = _history[_currentCommandIndex];

			try
			{
				cmd.Unexecute();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			_currentCommandIndex--;

			EventsHelper.Fire(_currentCommandChangedEvent, this, EventArgs.Empty);
		}
	}
}
