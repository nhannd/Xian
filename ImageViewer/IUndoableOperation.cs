using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	//TODO: redo documentation

	/// <summary>
	/// Models an undoable operation applied to an item of type <typeparamref name="T"/>, to
	/// be used in conjuction with the <see cref="UndoableOperationApplicator{T}"/>.
	/// </summary>
	/// <remarks>
	/// <para>This interface may be deprecated in a future release. Consider using the <see cref="CompositeUndoableCommand"/> instead.</para>
	/// <para>
	/// The item type <typeparam name="T"/> need not implement <see cref="IMemorable"/> itself,
	/// but must be able to provide the originator object (from the <see cref="GetOriginator"/> method) for
	/// the operation being performed.</para>
	/// </remarks>
	public interface IUndoableOperation<T> where T : class
	{
		/// <summary>
		/// Gets the object whose state will be captured and or restored before and/or after
		/// the operation is applied (via <see cref="Apply"/>).
		/// </summary>
		IMemorable GetOriginator(T item);

		/// <summary>
		/// Gets whether or not this operation applies to the given <paramref name="item"/>.
		/// </summary>
		bool AppliesTo(T item);

		/// <summary>
		/// Applies the operation to the given <paramref name="item"/>.
		/// </summary>
		void Apply(T item);
	}
}