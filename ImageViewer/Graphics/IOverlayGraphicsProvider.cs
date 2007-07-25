using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides access to a <see cref="GraphicCollection"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// If you have subclassed <see cref="PresentationImage"/> and want to expose
	/// a <see cref="GraphicCollection"/> to <see cref="ImageViewerTool"/> objects, 
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
	public interface IOverlayGraphicsProvider
	{
		/// <summary>
		/// Gets a <see cref="GraphicCollection"/>.
		/// </summary>
		GraphicCollection OverlayGraphics { get; }
	}
}
