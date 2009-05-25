using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	/// <summary>
	/// Interface for <see cref="IPresentationImage"/>s that support a <see cref="PresentationStates.PresentationState"/>.
	/// </summary>
	public interface IPresentationStateProvider
	{
		/// <summary>
		/// Gets or sets the <see cref="PresentationStates.PresentationState"/> of the image.
		/// </summary>
		PresentationState PresentationState { get; set; }
	}

	/// <summary>
	/// The base presentation state class from which all specific presentation state implementations derive.
	/// </summary>
	/// <remarks>
	/// The objects that constitute the presentation state of a given <see cref="IPresentationImage"/> are defined by the specific implementations.
	/// </remarks>
	[Cloneable(true)]
	public abstract class PresentationState
	{
		protected PresentationState() {}

		/// <summary>
		/// Serializes the presentation state of the image to the current state object.
		/// </summary>
		/// <param name="image">The image whose presentation state is to be serialized.</param>
		public virtual void Serialize(IPresentationImage image)
		{
			this.Serialize(ToEnumerable(image));
		}

		/// <summary>
		/// Serializes the presentation state of the given images to the current state object.
		/// </summary>
		/// <param name="images">The images whose presentation states are to be serialized.</param>
		public abstract void Serialize(IEnumerable<IPresentationImage> images);

		/// <summary>
		/// Deserializes the presentation state from the current state object into the given image.
		/// </summary>
		/// <param name="image">The image to which the presentation state is to be deserialized.</param>
		public virtual void Deserialize(IPresentationImage image)
		{
			this.Deserialize(ToEnumerable(image));
		}

		/// <summary>
		/// Deserializes the presentation state from the current state object into the given images.
		/// </summary>
		/// <param name="images">The images to which the presentation state is to be deserialized.</param>
		public abstract void Deserialize(IEnumerable<IPresentationImage> images);

		/// <summary>
		/// Clears the presentation state of the given image.
		/// </summary>
		/// <remarks>
		/// Whether all presentation state concepts defined by the implementation are cleared, or only the
		/// objects actually defined by this particular state object are cleared, is up to the implementation.
		/// </remarks>
		/// <param name="image">The image whose presentation state is to be cleared.</param>
		public virtual void Clear(IPresentationImage image)
		{
			this.Clear(ToEnumerable(image));
		}

		/// <summary>
		/// Clears the presentation states of the given images.
		/// </summary>
		/// <remarks>
		/// Whether all presentation state concepts defined by the implementation are cleared, or only the
		/// objects actually defined by this particular state object are cleared, is up to the implementation.
		/// </remarks>
		/// <param name="image">The images whose presentation states are to be cleared.</param>
		public abstract void Clear(IEnumerable<IPresentationImage> image);

		private static IEnumerable<IPresentationImage> ToEnumerable(IPresentationImage image)
		{
			yield return image;
		}
	}
}