using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A base definition of an <see cref="IImageOperation"/>.
	/// </summary>
	public abstract class ImageOperation : IImageOperation
	{
		/// <summary>
		/// Default protected constructor.
		/// </summary>
		protected ImageOperation()
		{
		}

		#region IImageOperation Members

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
		/// by <see cref="IPresentationImage"/>.  <see cref="IImageOperation.GetOriginator"/> allows
		/// the plugin developer to define what that object is.
		/// </para>
		/// <para>
		/// <see cref="IImageOperation.AppliesTo"/> should not return true if <see cref="IImageOperation.GetOriginator"/> has returned null.
		/// However, it is valid for <see cref="IImageOperation.GetOriginator"/> to return a non-null value and <see cref="IImageOperation.AppliesTo"/>
		/// to return false.
		/// </para>
		/// </remarks>
		/// <returns>
		/// The appropriate originator for the input <see cref="IPresentationImage"/>, or null if one doesn't exist.
		/// </returns>
		public abstract IMemorable GetOriginator(IPresentationImage image);

		/// <summary>
		/// Gets whether or not the operation is applicable for the input <see cref="IPresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="IImageOperation.AppliesTo"/> should never return true if <see cref="IImageOperation.GetOriginator"/> has returned null.
		/// </remarks>
		/// <returns>
		/// Unless overridden, returns true if <see cref="GetOriginator"/> returns a non-null value, otherwise false.
		/// </returns>
		public virtual bool AppliesTo(IPresentationImage image)
		{
			return GetOriginator(image) != null;
		}

		/// <summary>
		/// Applies the operation to the input <see cref="IPresentationImage"/>.
		/// </summary>
		public abstract void Apply(IPresentationImage image);

		#endregion
	}
}
