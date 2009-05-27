using ClearCanvas.Desktop;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer
{
	public class UndoableOperationCommand<T> : CompositeUndoableCommand where T : class 
	{
		public UndoableOperationCommand(IUndoableOperation<T> operation, T item)
			: this(operation, ToEnumerable(item))
		{
		}

		public UndoableOperationCommand(IUndoableOperation<T> operation, IEnumerable<T> items)
		{
			Execute(operation, items);
		}

		private static IEnumerable<U> ToEnumerable<U>(U item) where U : class
		{
			yield return item;
		}

		private void Execute(IUndoableOperation<T> operation, IEnumerable<T> items)
		{
			foreach (T item in items)
			{
				UndoableCommand command = Apply(operation, item);
				if (command != null)
					Enqueue(command);
			}
		}

		protected virtual UndoableCommand Apply(IUndoableOperation<T> operation, T item)
		{
			IMemorable originator = operation.GetOriginator(item);
			if (originator != null)
			{
				object beginState = originator.CreateMemento();
				if (beginState != null)
				{
					operation.Apply(item);

					object endState = originator.CreateMemento();
					if (!Equals(beginState, endState))
					{
						MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(originator);
						memorableCommand.BeginState = beginState;
						memorableCommand.EndState = endState;

						return memorableCommand;
					}
				}
			}

			return null;
		}
	}
}
