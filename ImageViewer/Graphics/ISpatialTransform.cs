using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a 2D spatial transformation.
	/// </summary>
	public interface ISpatialTransform : IMemorable
	{
		/// <summary>
		/// Gets or sets a value indicating whether the object if flipped horizontally
		/// (i.e., mirrored on the y-axis)
		/// </summary>
		bool FlipY { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the object if flipped vertically
		/// (i.e., mirrored on the x-axis)
		/// </summary>
		bool FlipX { get; set; }

		/// <summary>
		/// Gets or sets the rotation.
		/// </summary>
		int RotationXY { get; set; }

		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		float Scale { get; set; }
		
		/// <summary>
		/// Gets the scale in the x-direction.
		/// </summary>
		float ScaleX { get; }

		/// <summary>
		/// Gets the scale in the y-direction.
		/// </summary>
		float ScaleY { get; }

		/// <summary>
		/// Gets or sets the translation in the x-direction.
		/// </summary>
		float TranslationX { get; set; }

		/// <summary>
		/// Gets or sets the translation in the y-direction.
		/// </summary>
		float TranslationY { get; set; }
	}
}
