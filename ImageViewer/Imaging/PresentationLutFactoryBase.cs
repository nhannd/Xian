
namespace ClearCanvas.ImageViewer.Imaging
{
	public abstract class PresentationLutFactoryBase<T> : IPresentationLutFactory
		where T : PresentationLut, new()
	{
		#region IPresentationLutFactory Members

		public abstract string Name { get; }
		public abstract string Description { get; }

		public IPresentationLut Create()
		{
			return new T();
		}

		#endregion
	}
}
