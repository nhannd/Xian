using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Defines a pixel data prefetching strategy.
	/// </summary>
	public interface IPrefetchingStrategy
	{
		/// <summary>
		/// Gets the friendly name of the prefetching strategy.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the friendly description of the prefetching strategy
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Starts prefetching pixel data in the background.
		/// </summary>
		/// <param name="imageViewer"></param>
		/// <remarks>
		/// Use <paramref name="imageViewer"/> to determine how prefetching is done.
		/// </remarks>
		void Start(IImageViewer imageViewer);

		/// <summary>
		/// Stops prefetching of pixel data in the background.
		/// </summary>
		/// <remarks>
		/// Implementers should ensure that all background threads have terminated
		/// before this method returns.
		/// </remarks>
		void Stop();
	}
}
