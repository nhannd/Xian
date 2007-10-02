
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A composed Lut is one that has been created by combining multiple Luts together.
	/// </summary>
	/// <remarks>
	/// The <see cref="Data"/> property should be considered readonly and is only provided
	/// for fast (unsafe) iteration overy the array.  However, it also enforces that <see cref="IComposedLut"/>s
	/// be data Luts, which is important because the overall efficiency of the Lut pipeline is improved 
	/// substantially.
	/// </remarks>
	public interface IComposedLut : ILut
	{
		/// <summary>
		/// Gets the lut's data.  This property should be considered readonly
		/// and is only provided for fast (unsafe) iteration over the array.
		/// </summary>
		int[] Data { get; }
	}
}
