using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for a single <see cref="IPresentationImage"/>.
	/// </summary>
	public interface ITile : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="ITile"/> is not part of the 
		/// physical workspace yet.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the parent <see cref="IImageBox"/>
		/// </summary>
		/// <value>The parent <see cref="IImageBox"/> or <b>null</b> if the 
		/// <see cref="ITile"/> has not
		/// been added to the <see cref="IImageBox"/> yet.</value>
		IImageBox ParentImageBox { get; }

		/// <summary>
		/// Gets the <see cref="IPresentationImage"/> associated with this
		/// <see cref="ITile"/>.
		/// </summary>
		IPresentationImage PresentationImage { get; }

		/// <summary>
		/// Gets this <see cref="ITile"/>'s normalized rectangle.
		/// </summary>
		/// <remarks>
		/// Normalized coordinates specify the top-left corner,
		/// width and height of the <see cref="ITile"/> as a 
		/// fraction of the image box.  For example, if the
		/// <see cref="NormalizedRectangle"/> is (left=0.25, top=0.0, width=0.5, height=0.5) 
		/// and the image box has dimensions of (width=1000, height=800), the 
		/// <see cref="ITile"/> rectangle would be (left=250, top=0, width=500, height=400)
		/// </remarks>
		RectangleF NormalizedRectangle { get; }

		/// <summary>
		/// Gets this <see cref="ITile"/>'s client rectangle.
		/// </summary>
		Rectangle ClientRectangle { get; }

		/// <summary>
		/// Gets the presentation image index.
		/// </summary>
		int PresentationImageIndex { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ITile"/> is
		/// selected.
		/// </summary>
		/// <remarks>
		/// <see cref="ITile"/> selection is mutually exclusive.  That is,
		/// only one <see cref="ITile"/> is ever selected at a given time.  
		/// </remarks>
		bool Selected { get; }

		/// <summary>
		/// Selects the <see cref="ITile"/>.
		/// </summary>
		/// <remarks>
		/// Selecting a <see cref="ITile"/> also selects the containing <see cref="IImageBox"/>
		/// and deselects any other currently seleccted <see cref="ITile"/> 
		/// and <see cref="IImageBox"/>.
		/// </remarks>
		void Select();
	}
}
