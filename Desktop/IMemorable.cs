namespace ClearCanvas.Desktop
{
	// TODO: Make sure that all implementations of CreateMemento and
	// SetMemento are virtual, unless the class is sealed.

	/// <summary>
	/// Allows object state to be captured and restored.
	/// </summary>
	/// <remarks>
	/// <see cref="IMemorable"/> can be implemented by classes that require support
	/// for the <i>Memento</i> design pattern--<see cref="IMemorable"/> acts as the
	/// <i>Originator</i>.  Typically, the <see cref="IMemorable"/>
	/// and <see cref="IMemento"/> interfaces are used in conjunction with
	/// <see cref="UndoableCommand"/> to provide undo/redo support.
	/// </remarks>
	public interface IMemorable
	{
		/// <summary>
		/// Captures the state of an object.
		/// </summary>
		/// <returns>An <see cref="IMemento"/>.</returns>
		/// <remarks>
		/// The implementation of <see cref="CreateMemento"/> should return an
		/// <see cref="IMemento"/> containing enough state information so that
		/// when <see cref="SetMemento"/> is called, the object can be restored
		/// to the original state.
		/// </remarks>
		IMemento CreateMemento();

		/// <summary>
		/// Restores the state of an object.
		/// </summary>
		/// <param name="memento">The <see cref="IMemento"/> object that was
		/// originally created with <see cref="CreateMemento"/>.</param>
		/// <remarks>
		/// The implementation of <see cref="SetMemento"/> should return the 
		/// object to the original state captured by <see cref="CreateMemento"/>.
		/// </remarks>
		void SetMemento(IMemento memento);
	}
}
