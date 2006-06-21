using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Common.Application 
{
	public class CommandHistory 
	{
		// Private attributes
		private List<UndoableCommand> m_History = new List<UndoableCommand>();
		private int m_MaxSize = 0;
		private int m_CurrentCommandIndex = -1;
		private int m_LastCommandIndex = -1;

		private event EventHandler m_CurrentCommandChangedEvent;

		// Constructor
		public CommandHistory(int maxSize)
		{
			Platform.CheckPositive(maxSize, "maxSize");

			m_MaxSize = maxSize;
		}

		// Properties
		public int NumCommands 
		{
			get
			{
				return m_History.Count;
			}
		}

		public int MaxSize
		{
			get
			{
				return m_MaxSize;
			}
		}

		public int CurrentCommandIndex
		{
			get
			{
				return m_CurrentCommandIndex;
			}
		}

		public int LastCommandIndex
		{
			get
			{
				return m_LastCommandIndex;
			}
		}

		// Event accessors
		public event EventHandler CurrentCommandChanged
		{
			add
			{
				m_CurrentCommandChangedEvent += value;
			}
			remove
			{
				m_CurrentCommandChangedEvent -= value;
			}
		}

		// Public methods
		public void AddCommand(UndoableCommand command)
		{
			Platform.CheckForNullReference(command, "command");

			if (m_CurrentCommandIndex < m_LastCommandIndex)
			{
				int numCommandsToRemove = m_LastCommandIndex - m_CurrentCommandIndex;
				m_History.RemoveRange(m_CurrentCommandIndex + 1, numCommandsToRemove);
				m_LastCommandIndex -= numCommandsToRemove;
			}

			m_History.Add(command);

			if (NumCommands > m_MaxSize)
			{
				m_History.RemoveAt(0);

				if (m_CurrentCommandIndex == m_LastCommandIndex)
					m_CurrentCommandIndex--;

				m_LastCommandIndex--;
			}

			m_CurrentCommandIndex++;
			m_LastCommandIndex++;

			EventsHelper.Fire(m_CurrentCommandChangedEvent, this, EventArgs.Empty);
		}

		public void Redo()
		{
			if (NumCommands == 0)
				return;

			if (m_CurrentCommandIndex == m_LastCommandIndex)
				return;

			m_CurrentCommandIndex++;
			UndoableCommand cmd = m_History[m_CurrentCommandIndex];

			try
			{
				cmd.Execute();
			}
			catch (Exception e)
			{
				Platform.Log(e, LogLevel.Error);
			}

			EventsHelper.Fire(m_CurrentCommandChangedEvent, this, EventArgs.Empty);
		}

		public void Undo()
		{
			if (NumCommands == 0)
				return;

			if (m_CurrentCommandIndex == -1)
				return;

			UndoableCommand cmd = m_History[m_CurrentCommandIndex];

			try
			{
				cmd.Unexecute();
			}
			catch (Exception e)
			{
				Platform.Log(e, LogLevel.Error);
			}

			m_CurrentCommandIndex--;

			EventsHelper.Fire(m_CurrentCommandChangedEvent, this, EventArgs.Empty);
		}
	}
}
