using System;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines the final image that is presented to the user in an <see cref="ITile"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// An <see cref="IPresentationImage"/> can be thought of as a “scene” 
	/// composed of many parts, be they images, lines, text, etc.  It is the
	/// image that is presented to the user in a <see cref="Tile"/>.
	/// </para>
	/// <para>
	/// You should not implement <see cref="IPresentationImage"/> directly.
	/// Instead, subclass <see cref="PresentationImage"/>.
	/// </para>
	/// </remarks>
	public interface IPresentationImage : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="IPresentationImage"/> is not part of the 
		/// logical workspace yet.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the parent <see cref="IDisplaySet"/>.
		/// </summary>
		/// <value>The parent <see cref="IDisplaySet"/> or <b>null</b> if the 
		/// <see cref="IPresentationImage"/> has not
		/// been added to the <see cref="IDisplaySet"/> yet.</value>
		IDisplaySet ParentDisplaySet { get; }

		/// <summary>
		/// Gets the associated <see cref="ITile"/>.
		/// </summary>
		/// <value>The <see cref="ITile"/> that currently contains the
		/// <see cref="IPresentationImage"/> or <b>null</b> if the 
		/// <see cref="IPresentationImage"/> is not currently visible.</value>
		ITile Tile { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="IPresentationImage"/>
		/// is linked.
		/// </summary>
		bool Linked { get; set; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="IPresentationImage"/>
		/// is selected.
		/// </summary>
		bool Selected { get; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="IPresentationImage"/>
		/// is visible.
		/// </summary>
		bool Visible { get; }

		/// <summary>
		/// Gets the currently selected <see cref="IGraphic"/>.
		/// </summary>
		/// <value>The currently selected <see cref="IGraphic"/> or <b>null</b>
		/// if no <see cref="IGraphic"/> is currently selected.</value>
		/// <remarks>
		/// It is possible for an <see cref="IGraphic"/> to be selected,
		/// focussed or selected and focussed.
		/// </remarks>
		ISelectableGraphic SelectedGraphic { get; set; }

		/// <summary>
		/// Gets the currently focussed <see cref="IGraphic"/>.
		/// </summary>
		/// <value>The currently selected <see cref="IGraphic"/> or <b>null</b>
		/// if no <see cref="IGraphic"/> is currently focussed.</value>
		/// <remarks>
		/// It is possible for an <see cref="IGraphic"/> to be selected,
		/// focussed or selected and focussed.
		/// </remarks>
		IFocussableGraphic FocussedGraphic { get; set; }

		/// <summary>
		/// Gets or sets unique identifier for this <see cref="IPresentationImage"/>.
		/// </summary>
		string Uid { get; set; }

		/// <summary>
		/// Creates a clone of the <see cref="IPresentationImage"/>.
		/// </summary>
		/// <returns></returns>
		IPresentationImage Clone();
	}
}
