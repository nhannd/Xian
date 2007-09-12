
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// An <see cref="IDataLut"/> that is purely generated, usually by an equation or algorithm.
	/// </summary>
	public interface IGeneratedDataLut : IDataLut
	{
		/// <summary>
		/// Called by the framework to lazily create the data in the lut.  This method can be called repeatedly by
		/// the framework, along with <see cref="Clear"/>.
		/// </summary>
		void Create();
		
		/// <summary>
		/// Called by the framework to release any data held by the lut.  <see cref="Create"/> will be called again
		/// if and when the data is needed.
		/// </summary>
		void Clear();
	}
}
