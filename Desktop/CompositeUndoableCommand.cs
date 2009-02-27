using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// An <see cref="UndoableCommand"/> whose only purpose is to process other <see cref="UndoableCommand"/>s in
	/// a repeatable manner, such that the entire set of commands can be undone/redone.
	/// </summary>
	/// <remarks>
	/// The <see cref="CompositeUndoableCommand"/> doesn't place any explicit restrictions as to whether
	/// or not a <see cref="UndoableCommand"/> has already been executed or unexecuted, but rather it
	/// leaves the details up to the consumer.  Typically, before adding a <see cref="CompositeUndoableCommand"/>
	/// to the <see cref="CommandHistory"/>, it (or it's contained commands) must be Executed first.
	/// </remarks>
	public class CompositeUndoableCommand : UndoableCommand
	{
		private readonly List<UndoableCommand> _commands;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CompositeUndoableCommand()
		{
			_commands = new List<UndoableCommand>();
		}

		/// <summary>
		/// Gets the number of commands in this <see cref="CompositeUndoableCommand"/>.
		/// </summary>
		public int Count
		{
			get { return _commands.Count; }	
		}

		/// <summary>
		/// Adds/Enqueues an <see cref="UndoableCommand"/>.
		/// </summary>
		public void Enqueue(UndoableCommand command)
		{
			_commands.Add(command);
		}

		/// <summary>
		/// <see cref="Execute"/>s each command, from the beginning to the end.
		/// </summary>
		public override void Execute()
		{
			foreach (UndoableCommand command in _commands)
				command.Execute();
		}

		/// <summary>
		/// <see cref="Unexecute"/>s each command, from the end to the beginning.
		/// </summary>
		public override void Unexecute()
		{
			for (int i = _commands.Count - 1; i >= 0; --i)
				_commands[i].Unexecute();
		}
	}
}
