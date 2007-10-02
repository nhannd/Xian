
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// The basic definition of a Lut.
	/// </summary>
	public interface ILut
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
		/// <param name="index">the index into the Lut</param>
		/// <returns>the value at the given index</returns>
		int this[int index] { get; }
	}
}
