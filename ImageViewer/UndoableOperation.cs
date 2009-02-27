using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	public abstract class UndoableOperation<T> : IUndoableOperation<T> where T : class
	{
		protected UndoableOperation()
		{
		}

		#region IUndoableOperation<T> Members

		public abstract IMemorable GetOriginator(T item);

		public virtual bool AppliesTo(T item)
		{
			return GetOriginator(item) != null;
		}

		public abstract void Apply(T item);

		#endregion

		public static MemorableUndoableCommand Apply(IUndoableOperation<T> operation, T item)
		{
			if (!operation.AppliesTo(item))
				return null;

			if (operation.GetOriginator(item) == null)
				return null;

			IMemorable originator = operation.GetOriginator(item);
			object beginState = originator.CreateMemento();
			if (beginState == null)
				return null;

			operation.Apply(item);

			object endState = originator.CreateMemento();
			if (Equals(beginState, endState))
				return null;

			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(originator);
			memorableCommand.BeginState = beginState;
			memorableCommand.EndState = endState;

			return memorableCommand;
		}
	}
}
