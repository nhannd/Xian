using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Used to apply an undoable operation to an <see cref="IPresentationImage"/>.
	/// </summary>
	public interface IImageOperation
	{
		/// <summary>
		/// Gets the object whose state is to be captured or restored.
		/// </summary>
		/// <param name="image">An <see cref="IPresentationImage"/> that contains
		/// the object whose state is to be captured or restored.</param>
		/// <remarks>
		/// <para>
		/// Typically, operations are applied to some aspect of the presentation image,
		/// such as zoom, pan, window/level, etc. That aspect will usually be 
		/// encapsulated as an object that is owned by the
		/// by <see cref="IPresentationImage"/>.  <see cref="GetOriginator"/> allows
		/// the plugin developer to define what that object is.
		/// </para>
		/// <para>
		/// <see cref="AppliesTo"/> should not return true if <see cref="GetOriginator"/> has returned null.
		/// However, it is valid for <see cref="GetOriginator"/> to return a non-null value and <see cref="AppliesTo"/>
		/// to return false.
		/// </para>
		/// </remarks>
		/// <returns>
		/// The appropriate originator for the input <see cref="IPresentationImage"/>, or null if one doesn't exist.
		/// </returns>
		IMemorable GetOriginator(IPresentationImage image);

		/// <summary>
		/// Gets whether or not the operation is applicable for the input <see cref="IPresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="AppliesTo"/> should not return true if <see cref="GetOriginator"/> has returned null.
		/// </remarks>
		bool AppliesTo(IPresentationImage image);

		/// <summary>
		/// Applies the operation to the input <see cref="IPresentationImage"/>.
		/// </summary>
		void Apply(IPresentationImage image);
	}
}
