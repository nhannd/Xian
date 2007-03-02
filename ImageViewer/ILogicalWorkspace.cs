using System;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A container for <see cref="IImageSet"/> objects.
	/// </summary>
	public interface ILogicalWorkspace : IDisposable
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

		/// <summary>
		/// Draws the <see cref="ILogicalWorkspace"/>.
		/// </summary>
		void Draw();
	}
}
