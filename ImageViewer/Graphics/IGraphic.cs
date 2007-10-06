using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a graphical object that can be rendered.
	/// </summary>
	public interface IGraphic : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets this <see cref="IGraphic"/> object's parent <see cref="IGraphic"/>.
		/// </summary>
		IGraphic ParentGraphic { get; }

		/// <summary>
		/// Gets this <see cref="IGraphic"/> object's associated 
		/// <see cref="IPresentationImage"/>.
		/// </summary>
		/// <value>The associated <see cref="IPresentationImage"/> or <b>null</b>
		/// if the <see cref="IGraphic"/> is not yet part of the scene graph.</value>
		IPresentationImage ParentPresentationImage { get; }

		/// <summary>
		/// Gets this <see cref="IGraphic"/> object's associated 
		/// <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b>
		/// if the <see cref="IGraphic"/> is not yet associated with
		/// an <see cref="IImageViewer"/>.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IGraphic"/> is visible.
		/// </summary>
		bool Visible { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// After the <see cref="IGraphic.CoordinateSystem"/> has been set and the
		/// desired operations have been performed on the <see cref="IGraphic"/>,
		/// it is proper practice to call <see cref="ResetCoordinateSystem"/>
		/// to restore the previous coordinate system.
		/// </remarks>
		CoordinateSystem CoordinateSystem { get; set; }

		/// <summary>
		/// Gets this <see cref="Graphic"/> object's <see cref="SpatialTransform"/>.
		/// </summary>
		SpatialTransform SpatialTransform { get; }

		/// <summary>
		/// Gets or sets the name of this <see cref="IGraphic"/>.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Performs a hit test on the <see cref="IGraphic"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="IGraphic"/>,
		/// <b>false</b> otherwise.
		/// <remarks>
		/// It is up to the implementation of <see cref="IGraphic"/> to define what a "hit" is.
		/// </remarks>
		/// </returns>
		bool HitTest(Point point);

		/// <summary>
		/// Moves the <see cref="IGraphic"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		void Move(SizeF delta);

		/// <summary>
		/// Resets the <see cref="CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="ResetCoordinateSystem"/> will reset the <see cref="CoordinateSystem"/>
		/// to what it was before the <see cref="IGraphic.CoordinateSystem"/> property
		/// was last set.
		/// </para>
		/// </remarks>
		void ResetCoordinateSystem();
	}
}
