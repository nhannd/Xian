using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a LUT that can be added to <see cref="LUTCollection"/>.
	/// </summary>
	public interface ILut : IEquatable<LutCreationParameters>, IEquatable<ILut>
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
		
		/// <summary>
		/// Gets a string key that identifies this particular LUT.
		/// </summary>
		/// <remarks>
		/// Implementors of <see cref="ILUT"/> must implement this
		/// method.  The string returned should be a string that uniquely 
		/// identifies the LUT based on the LUT's characteristic parameters so 
		/// that it can be used as a key in a dictionary.
		/// </remarks>
		string GetKey();

		LutCreationParameters GetCreationParametersMemento();
		
		bool TrySetCreationParametersMemento(LutCreationParameters creationParameters);
	}
}
