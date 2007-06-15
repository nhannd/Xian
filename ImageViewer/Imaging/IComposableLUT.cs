using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a LUT that can be added to <see cref="LUTCollection"/>.
	/// </summary>
	public interface IComposableLUT : ILUT
	{
		/// <summary>
		/// Gets the minimum input value.
		/// </summary>
		int MinInputValue { get; }

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		int MaxInputValue { get; }

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		int MinOutputValue { get; }

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		int MaxOutputValue { get; }

		/// <summary>
		/// Occurs when the LUT has changed.
		/// </summary>
		event EventHandler LUTChanged;

		/// <summary>
		/// Gets a string key that identifies this particular LUT.
		/// </summary>
		/// <remarks>
		/// Implementors of <see cref="IComposableLUT"/> must implement this
		/// method.  The string returned should be a string that uniquely 
		/// identifies the LUT based on the LUT's characteristic parameters so 
		/// that it can be used as a key in a dictionary.
		/// </remarks>
		string GetKey();
	}
}
