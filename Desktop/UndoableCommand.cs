namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Abstract base class for 'undoable' commands.
	/// </summary>
	public abstract class UndoableCommand : Command
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected UndoableCommand()
		{
		}

		/// <summary>
		/// Performs and 'undo' of the <see cref="Command.Execute"/> operation.
		/// </summary>
		public abstract void Unexecute();
	}
}
