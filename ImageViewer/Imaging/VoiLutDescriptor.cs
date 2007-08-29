
using ClearCanvas.Common;
namespace ClearCanvas.ImageViewer.Imaging
{
	public sealed class VoiLutDescriptor
	{
		private readonly string _name;
		private readonly string _description;

		private VoiLutDescriptor(string name, string description)
		{
			_name = name;
			_description = description;
		}

		private VoiLutDescriptor()
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

		public static VoiLutDescriptor FromFactory(IVoiLutFactory factory)
		{
			Platform.CheckForNullReference(factory, "factory");
			return new VoiLutDescriptor(factory.Name, factory.Description);
		}
	}
}
