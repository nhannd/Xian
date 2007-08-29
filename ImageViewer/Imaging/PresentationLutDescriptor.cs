using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public sealed class PresentationLutDescriptor
	{
		private readonly string _name;
		private readonly string _description;

		private PresentationLutDescriptor(string name, string description)
		{
			_name = name;
			_description = description;
		}

		private PresentationLutDescriptor()
		{
		}

		public string Name
		{
			get { return _name; }
		}

		public string Description
		{
			get { return _description; }
		}

		public static PresentationLutDescriptor FromFactory(IPresentationLutFactory factory)
		{
			Platform.CheckForNullReference(factory, "factory");
			return new PresentationLutDescriptor(factory.Name, factory.Description);
		}
	}
}
