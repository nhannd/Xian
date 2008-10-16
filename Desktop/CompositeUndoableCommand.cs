using System;
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// An <see cref="UndoableCommand"/> whose only purpose is to process other <see cref="UndoableCommand"/>s in
	/// a repeatable manner, such that the entire set of commands can be undone/redone.
	/// </summary>
	public class CompositeUndoableCommand : UndoableCommand
	{
		private readonly Stack<UndoableCommand> _unexecuteStack;
		private readonly Queue<UndoableCommand> _executeQueue;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CompositeUndoableCommand()
		{
			_unexecuteStack = new Stack<UndoableCommand>();
			_executeQueue = new Queue<UndoableCommand>();
		}

		/// <summary>
		/// Adds an <see cref="UndoableCommand"/> to the queue of commands to be processed.
		/// </summary>
		public void Enqueue(UndoableCommand command)
		{
			if (_unexecuteStack.Count > 0)
				throw new InvalidOperationException("Command must be unexecuted before the command queue can be modified.");

			_executeQueue.Enqueue(command);
		}

		/// <summary>
		/// <see cref="Execute"/>s each command in the queue.
		/// </summary>
		public override void Execute()
		{
			if (_unexecuteStack.Count > 0)
				return; //do nothing, the command has already been executed.

			while (_executeQueue.Count > 0)
			{
				UndoableCommand command = _executeQueue.Dequeue();
				command.Execute();
				_unexecuteStack.Push(command);
			}

			base.Execute();
		}

		/// <summary>
		/// <see cref="Unexecute"/>s each command in the queue, in reverse order.
		/// </summary>
		public override void Unexecute()
		{
			if (_executeQueue.Count > 0)
				return; //do nothing, the command has already been unexecuted.

			while (_unexecuteStack.Count > 0)
			{
				UndoableCommand command = _unexecuteStack.Pop();
				command.Unexecute();
				_executeQueue.Enqueue(command);
			}

			base.Unexecute();
		}
	}
}
