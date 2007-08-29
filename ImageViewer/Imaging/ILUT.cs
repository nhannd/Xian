using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a LUT that can be added to a <see cref="LutCollection"/>
	/// </summary>
	public interface ILut : IMemorable
	{
		/// <summary>
		/// Gets or sets the minimum input value.  This value will be set internally by the framework.
		/// </summary>
		int MinInputValue { get; set; }

		/// <summary>
		/// Gets the maximum input value.  This value will be set internally by the framework.
		/// </summary>
		int MaxInputValue { get; set; }

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
		/// Gets a string key that identifies this particular LUT's characteristics, so that 
		/// an image's <see cref="OutputLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <B>equality</B>, since some Luts can be
		/// dependent upon the actual image to which it belongs.  The method should simply 
		/// be used to determine if a lut in the <see cref="OutputLutPool"/> is the same 
		/// as an existing one.
		/// </remarks>
		string GetKey();
	}
}
