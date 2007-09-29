
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// An <see cref="IDataLut"/> that is purely generated, usually by an equation or algorithm.
	/// </summary>
	public interface IGeneratedDataLut : IDataLut
	{
		/// <summary>
		/// Called by the framework to release any data held by the lut.  The Lut should be capable
		/// of recreating the data when it is needed.
		/// </summary>
		void Clear();
	}
}
