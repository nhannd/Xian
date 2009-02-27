using System.Collections.Generic;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	public abstract class DrawableUndoableOperation<T> : UndoableOperation<T> where T : class, IDrawable
	{
		protected DrawableUndoableOperation()
		{
		}

		public static DrawableUndoableCommand Apply(IUndoableOperation<T> operation, T item)
		{
			MemorableUndoableCommand memorableCommand = UndoableOperation<T>.Apply(operation, item);
			if (memorableCommand == null)
				return null;

			//we have to draw the item ourselves b/c the operation has already been applied.
			item.Draw();

			DrawableUndoableCommand drawableCommand = new DrawableUndoableCommand(item);
			drawableCommand.Enqueue(memorableCommand);
			return drawableCommand;
		}

		public static CompositeUndoableCommand Apply(IUndoableOperation<T> operation, IEnumerable<T> items)
		{
			CompositeUndoableCommand returnCommand = new CompositeUndoableCommand();
			foreach (T item in items)
			{
				DrawableUndoableCommand drawableCommand = Apply(operation, item);
				if (drawableCommand != null)
					returnCommand.Enqueue(drawableCommand);
			}

			if (returnCommand.Count > 0)
				return returnCommand;
			else
				return null;
		}
	}
}
