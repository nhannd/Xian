using System;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for <see cref="IDisplaySet"/> objects.
	/// </summary>
	public interface IImageSet : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="IImageSet"/> is not part of the 
		/// logical workspace yet.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the parent <see cref="ILogicalWorkspace"/>
		/// </summary>
		/// <value>The parent <see cref="ILogicalWorkspace"/> or <b>null</b> if the 
		/// <see cref="IImageSet"/> has not been added to an 
		/// <see cref="ILogicalWorkspace"/> yet.</value>
		ILogicalWorkspace ParentLogicalWorkspace { get; }

		/// <summary>
		/// Gets the collection of <see cref="IDisplaySet"/> objects that belong
		/// to this <see cref="IImageSet"/>.
		/// </summary>
		DisplaySetCollection DisplaySets { get; }

		// TODO: Change to IEnumerable

		/// <summary>
		/// Gets a collection of linked <see cref="IDisplaySet"/> objects.
		/// </summary>
		ReadOnlyCollection<IDisplaySet> LinkedDisplaySets { get; }

		/// <summary>
		/// Gets or sets the name of the image set.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Gets or sets the patient information associated with the image set.
		/// </summary>
		string PatientInfo { get; set; }

		/// <summary>
		/// Gets or sets unique identifier for this <see cref="IImageSet"/>.
		/// </summary>
		string Uid { get; set; }
	}
}
