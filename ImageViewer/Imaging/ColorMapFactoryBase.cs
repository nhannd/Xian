
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A base implementation for Color Map factories.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ColorMapFactoryBase<T> : IColorMapFactory
		where T : ColorMap, new()
	{
		#region IColorMapFactory Members

		/// <summary>
		/// Gets a name that should be unique when compared to other <see cref="IColorMapFactory"/>s.
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
		/// Creates an <see cref="IColorMap"/>.
		/// </summary>
		public IColorMap Create()
		{
			return new T();
		}

		#endregion
	}
}
