
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A base implementation for Presentation Lut factories.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class PresentationLutFactoryBase<T> : IPresentationLutFactory
		where T : PresentationLut, new()
	{
		#region IPresentationLutFactory Members

		/// <summary>
		/// Gets a name that should be unique when compared to other <see cref="IPresentationLutFactory"/>s.
		/// </summary>
		/// <remarks>
		/// This name should not be a resource string, as it should be constant for all languages.
		/// </remarks>
		public abstract string Name { get; }

		/// <summary>
		/// Gets a brief description of the factory.
		/// </summary>
		public abstract string Description { get; }

		/// <summary>
		/// Creates an <see cref="IPresentationLut"/>.
		/// </summary>
		public IPresentationLut Create()
		{
			return new T();
		}

		#endregion
	}
}
