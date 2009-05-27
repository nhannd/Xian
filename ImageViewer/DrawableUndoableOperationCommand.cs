using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	public class DrawableUndoableOperationCommand<T> : UndoableOperationCommand<T> where T : class, IDrawable
	{
		public DrawableUndoableOperationCommand(IUndoableOperation<T> operation, T item)
			: base(operation, item)
		{
		}

		public DrawableUndoableOperationCommand(IUndoableOperation<T> operation, IEnumerable<T> items)
			: base(operation, items)
		{
		}

		public sealed override void Execute()
		{
			base.Execute();
		}

		public sealed override void Unexecute()
		{
			base.Unexecute();
		}

		protected override UndoableCommand Apply(IUndoableOperation<T> operation, T item)
		{
			UndoableCommand command = base.Apply(operation, item);

			if (command != null)
			{
				item.Draw();

				DrawableUndoableCommand drawableCommand = new DrawableUndoableCommand(item);
				drawableCommand.Enqueue(command);
				command = drawableCommand;
			}

			return command;
		}
	}
}
