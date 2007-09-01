
namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IDataLut : ILut
	{
		/// <summary>
		/// Gets the length of the data lut.
		/// </summary>
		uint Length { get; }

		/// <summary>
		/// Called by the framework to lazily create the data in the lut.  This method can be called repeatedly by
		/// the framework, along with <see cref="Clear"/>.
		/// </summary>
		void Create();
		
		/// <summary>
		/// Called by the framework to release any data held by the lut.  <see cref="CreateLut"/> will be called again
		/// if and when the data is needed.
		/// </summary>
		void Clear();
	}
}
