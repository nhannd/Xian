using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a LUT that can be added to <see cref="LUTCollection"/>.
	/// </summary>
	public interface IComposableLUT : ILUT
	{
		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		int MinInputValue { get; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		int MaxInputValue { get; }

		/// <summary>
		/// Gets or sets the minimum output value.
		/// </summary>
		int MinOutputValue { get; }

		/// <summary>
		/// Gets or sets the maximum output value.
		/// </summary>
		int MaxOutputValue { get; }
	}
}
