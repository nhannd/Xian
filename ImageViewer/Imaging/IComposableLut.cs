using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a Lut that can be added to a <see cref="LutCollection"/>
	/// </summary>
	public interface IComposableLut : ILut, IMemorable
	{
		/// <summary>
		/// Gets or sets the minimum input value.  This value will be set internally by the framework.
		/// </summary>
		new int MinInputValue { get; set; }

		/// <summary>
		/// Gets the maximum input value.  This value will be set internally by the framework.
		/// </summary>
		new int MaxInputValue { get; set; }

		/// <summary>
		/// Fired when the LUT has changed in some way.
		/// </summary>
		event EventHandler LutChanged;
		
		/// <summary>
		/// Gets a string key that identifies this particular LUT's characteristics, so that 
		/// an image's <see cref="IComposedLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some Luts can be
		/// dependent upon the actual image to which it belongs.  The method should simply 
		/// be used to determine if a lut in the <see cref="ComposedLutPool"/> is the same 
		/// as an existing one.
		/// </remarks>
		string GetKey();

		/// <summary>
		/// Gets an abbreviated description of the Lut.
		/// </summary>
		string GetDescription();
	}
}
