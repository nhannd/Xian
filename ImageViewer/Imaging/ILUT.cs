using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a LUT that can be added to <see cref="LUTCollection"/>.
	/// </summary>
	public interface ILut : IEquatable<ILut>
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
		/// Gets the output value of the lut at a given input index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		int this[int index] { get; }

		/// <summary>
		/// Occurs when the LUT has changed.
		/// </summary>
		event EventHandler LutChanged;
		
		LutCreationParameters GetCreationParametersMemento();
	}
}
