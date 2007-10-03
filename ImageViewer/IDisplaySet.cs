using System;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for <see cref="IPresentationImage"/> objects.
	/// </summary>
	public interface IDisplaySet : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="IDisplaySet"/> is not part of the 
		/// logical workspace yet.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the parent <see cref="IImageSet"/>.
		/// </summary>
		/// <value>The parent <see cref="ImageSet"/> or <b>null</b> if the 
		/// <see cref="IDisplaySet"/> has not been added to an 
		/// <see cref="IImageSet"/> yet.</value>
		IImageSet ParentImageSet { get; }

		/// <summary>
		/// Gets the collection of <see cref="IPresentationImage"/> objects belonging
		/// to this <see cref="IDisplaySet"/>.
		/// </summary>
		PresentationImageCollection PresentationImages { get; }

		/// <summary>
		/// Gets a collection of linked <see cref="IPresentationImage"/> objects.
		/// </summary>
		ReadOnlyCollection<IPresentationImage> LinkedPresentationImages { get; }

		/// <summary>
		/// Gets the <see cref="IImageBox"/> associated with this <see cref="IDisplaySet"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageBox "/> or <b>null</b> if the
		/// <see cref="IDisplaySet"/> is not currently visible.</value>
		IImageBox ImageBox { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IImageBox"/> is
		/// linked.
		/// </summary>
		/// <value><b>true</b> if linked; <b>false</b> otherwise.</value>
		/// <remarks>
		/// Multiple display sets may be linked, allowing tools that can operate on
		/// multiple display sets to operate on all linked display sets simultaneously.  
		/// Note that the concept of linkage is slightly different from selection:
		/// it is possible for an <see cref="IDisplaySet"/> to be 1) selected but not linked
		/// 2) linked but not selected and 3) selected and linked.
		/// </remarks>
		bool Linked { get; set; }

		/// <summary>
		/// Gets or sets the name of the display set.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="IDisplaySet"/> is selected.
		/// </summary>
		bool Selected { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="IDisplaySet"/> is visible.
		/// </summary>
		bool Visible { get; }

		/// <summary>
		/// Gets or sets unique identifier for this <see cref="IDisplaySet"/>.
		/// </summary>
		string Uid { get; }

		// TODO: Change Clone to some other name

		/// <summary>
		/// Creates a clone of the <see cref="IDisplaySet"/>.
		/// </summary>
		/// <returns>The cloned <see cref="IDisplaySet"/>.</returns>
		IDisplaySet Clone();
	}
}
