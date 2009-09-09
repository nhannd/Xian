using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Externals
{
	[ExtensionPoint]
	public sealed class ExternalFactoryExtensionPoint : ExtensionPoint<IExternalFactory> {}

	public interface IExternalFactory
	{
		string Description { get; }
		IExternal CreateNew();
	}

	public abstract class ExternalFactoryBase<T> : IExternalFactory where T : IExternal, new()
	{
		private readonly string _description;

		protected ExternalFactoryBase(string description)
		{
			_description = description;
		}

		public string Description
		{
			get { return _description; }
		}

		public IExternal CreateNew()
		{
			T t = new T();
			t.Enabled = true;
			t.Label = "New External";
			t.Name = string.Empty;
			t.WindowStyle = WindowStyle.Normal;
			return t;
		}
	}
}