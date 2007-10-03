using System;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for <see cref="IImageSet"/> objects.
	/// </summary>
	public interface ILogicalWorkspace : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets a collection of <see cref="IImageSet"/> objects that belong to
		/// this <see cref="ILogicalWorkspace"/>.
		/// </summary>
		ImageSetCollection ImageSets { get; }
	}
}
