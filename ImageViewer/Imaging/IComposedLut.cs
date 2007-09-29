
namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IComposedLut : ILut
	{
		/// <summary>
		/// Gets the lut's data.  This property should be considered readonly
		/// and is only provided for fast (unsafe) iteration over the array.
		/// </summary>
		int[] Data { get; }
	}
}
