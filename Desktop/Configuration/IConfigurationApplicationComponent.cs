using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// Defines an interface for configuration <see cref="ApplicationComponent"/>s.
	/// </summary>
	public interface IConfigurationApplicationComponent : IApplicationComponent
	{
		/// <summary>
		/// Save any settings modified in the hosted component.
		/// </summary>
		void Save();
	}
}