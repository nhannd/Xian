using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Defines a rendering surface.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Unless you are implementing your own renderering surface, you should never
	/// have to interact with this interface.  The two properties on 
	/// <see cref="IRenderingSurface"/> should only ever have to be accessed by the 
	/// Framework, and thus should be treated as internal.
	/// </para>
	/// </remarks>
	public interface IRenderingSurface : IDisposable
	{
		/// <summary>
		/// Gets or sets the window ID.
		/// </summary>
		/// <remarks>
		/// On Windows systems, this is the window handle, or "hwnd".  This
		/// property is set by the Framework; you should never have to
		/// set this property yourself.
		/// </remarks>
		IntPtr WindowID
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the context ID.
		/// </summary>
		/// <remarks>
		/// On Windows systems, this is the device context handle, or "hdc". This
		/// property is set by the Framework; you should never have to
		/// set this property yourself.
		/// </remarks>
		IntPtr ContextID
		{
			get;
			set;
		}
	}
}
