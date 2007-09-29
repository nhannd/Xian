using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Provides a description of an <see cref="IColorMap"/>.
	/// </summary>
	public sealed class ColorMapDescriptor
	{
		private readonly string _name;
		private readonly string _description;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">the name of the factory</param>
		/// <param name="description">the factory description</param>
		private ColorMapDescriptor(string name, string description)
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
		/// Creates a <see cref="ColorMapDescriptor"/> given an input <see cref="IColorMapFactory"/>.
		/// </summary>
		/// <param name="factory">the factory</param>
		public static ColorMapDescriptor FromFactory(IColorMapFactory factory)
		{
			Platform.CheckForNullReference(factory, "factory");
			return new ColorMapDescriptor(factory.Name, factory.Description);
		}
	}
}
