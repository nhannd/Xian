using System;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Provides access to an <see cref="ImageSop"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// If you have subclassed <see cref="PresentationImage"/> and want to expose
	/// an <see cref="ImageSop"/> to <see cref="ImageViewerTool"/> objects, 
	/// do so by implementing this interface in your subclass.
	/// </para>
	/// <para>
	/// In general, avoid accessing members of subclasses of <see cref="PresentationImage"/>
	/// directly.  Prefer instead to use provider interfaces such as this one.  Doing
	/// so prevents <see cref="ImageViewerTool"/> objects from having to know about specific
	/// subclasses of <see cref="PresentationImage"/>, thus allowing them to work with
	/// any type of <see cref="PresentationImage"/> that implements the provider interface.
	/// </para>
	/// </remarks>
	public interface IImageSopProvider
	{
		/// <summary>
		/// Gets an <see cref="ImageSop"/>.
		/// </summary>
		ImageSop ImageSop { get; }
	}
}
