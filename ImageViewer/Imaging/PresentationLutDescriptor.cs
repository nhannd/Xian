using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Provides a description of an <see cref="IPresentationLut"/>.
	/// </summary>
	public sealed class PresentationLutDescriptor
	{
		private readonly string _name;
		private readonly string _description;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">the name of the factory</param>
		/// <param name="description">the factory description</param>
		private PresentationLutDescriptor(string name, string description)
		{
			_name = name;
			_description = description;
		}

		/// <summary>
		/// Gets the name of the factory.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets a brief description of the factory.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}

		/// <summary>
		/// Creates a <see cref="PresentationLutDescriptor"/> given an input <see cref="IPresentationLutFactory"/>.
		/// </summary>
		/// <param name="factory">the factory</param>
		public static PresentationLutDescriptor FromFactory(IPresentationLutFactory factory)
		{
			Platform.CheckForNullReference(factory, "factory");
			return new PresentationLutDescriptor(factory.Name, factory.Description);
		}
	}
}
